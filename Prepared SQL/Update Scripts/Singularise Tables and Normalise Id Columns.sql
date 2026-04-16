/*
    Renames every user table in [dbo] to its singular form (e.g. ServerAlerts -> ServerAlert)
    and renames every column whose name ends in uppercase "ID" to "Id" (e.g. UserID -> UserId).

    - Uses sp_rename, which preserves foreign keys, indexes, and constraints.
    - Idempotent: re-running is a no-op once everything is already compliant.
    - Set @WhatIf = 1 to print the statements without executing them.
*/

USE [HunterIndustriesAPI];
GO

SET NOCOUNT ON;

DECLARE @WhatIf bit = 1;   -- flip to 0 to actually apply changes

DECLARE @sql       nvarchar(max);
DECLARE @oldName   sysname;
DECLARE @newName   sysname;
DECLARE @schema    sysname;
DECLARE @tableName sysname;

------------------------------------------------------------
-- 1. Singularise table names
------------------------------------------------------------
DECLARE table_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT s.name, t.name
FROM   sys.tables   t
JOIN   sys.schemas  s ON s.schema_id = t.schema_id
WHERE  t.is_ms_shipped = 0
  AND  s.name = 'dbo'
  -- crude plural detector: ends in 's' but not 'ss', 'us', 'is'
  AND  t.name LIKE '%s'
  AND  t.name NOT LIKE '%ss'
  AND  t.name NOT LIKE '%us'
  AND  t.name NOT LIKE '%is'
  AND  t.name NOT IN ('Status');   -- extend this exception list as needed

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @schema, @oldName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Basic English singularisation rules
    IF @oldName LIKE '%ies'                                    -- Companies -> Company
        SET @newName = LEFT(@oldName, LEN(@oldName) - 3) + 'y';
    ELSE IF @oldName LIKE '%ches' OR @oldName LIKE '%shes'
         OR @oldName LIKE '%xes'  OR @oldName LIKE '%zes'      -- Boxes -> Box
        SET @newName = LEFT(@oldName, LEN(@oldName) - 2);
    ELSE                                                       -- ServerAlerts -> ServerAlert
        SET @newName = LEFT(@oldName, LEN(@oldName) - 1);

    IF @newName <> @oldName
       AND NOT EXISTS (SELECT 1 FROM sys.tables t2
                       JOIN sys.schemas s2 ON s2.schema_id = t2.schema_id
                       WHERE s2.name = @schema AND t2.name = @newName)
    BEGIN
        SET @sql = N'EXEC sp_rename '
                 + QUOTENAME(@schema + '.' + @oldName, '''')
                 + N', ' + QUOTENAME(@newName, '''') + N';';

        PRINT @sql;
        IF @WhatIf = 0 EXEC sp_executesql @sql;
    END

    FETCH NEXT FROM table_cursor INTO @schema, @oldName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

------------------------------------------------------------
-- 2. Normalise columns ending in 'ID' to 'Id'
------------------------------------------------------------
DECLARE column_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT s.name, t.name, c.name
FROM   sys.columns  c
JOIN   sys.tables   t ON t.object_id = c.object_id
JOIN   sys.schemas  s ON s.schema_id = t.schema_id
WHERE  t.is_ms_shipped = 0
  AND  s.name = 'dbo'
  AND  c.name COLLATE Latin1_General_CS_AS LIKE '%ID'
  AND  c.name COLLATE Latin1_General_CS_AS NOT LIKE '%Id';

OPEN column_cursor;
FETCH NEXT FROM column_cursor INTO @schema, @tableName, @oldName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @newName = LEFT(@oldName, LEN(@oldName) - 2) + 'Id';

    SET @sql = N'EXEC sp_rename '
             + QUOTENAME(@schema + '.' + @tableName + '.' + @oldName, '''')
             + N', ' + QUOTENAME(@newName, '''')
             + N', ''COLUMN'';';

    PRINT @sql;
    IF @WhatIf = 0 EXEC sp_executesql @sql;

    FETCH NEXT FROM column_cursor INTO @schema, @tableName, @oldName;
END

CLOSE column_cursor;
DEALLOCATE column_cursor;
GO

INSERT INTO VersionHistory(ReleaseVersion, DateUpdated)
VALUES ('2.0.0', GETUTCDATE())

PRINT('Added VersionHistory Record')