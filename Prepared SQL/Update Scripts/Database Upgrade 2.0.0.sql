USE [HunterIndustriesAPI]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VersionHistory' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[VersionHistory](
		[VersionId] [int] IDENTITY(1,1) NOT NULL,
		[ReleaseVersion] [varchar](11) NOT NULL,
		[DateUpdated] [datetime] NOT NULL,
	 CONSTRAINT [PK_VersionHistory] PRIMARY KEY CLUSTERED
	(
		[VersionId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('VersionHistory Table Added')
END
ELSE
	PRINT('VersionHistory Table Already Exists')
GO

UPDATE [Endpoint] SET [Value] = REPLACE([Value], 'hunter-industries.co.uk/api', 'api.hunter-industries.co.uk')

PRINT('Changed URL Values')
GO

UPDATE Component SET [Name] = REPLACE([Name], ' Status', '')

PRINT('Removed "Status" from Component Names')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'Name')
	ALTER TABLE ServerInformation ADD [Name] [varchar](255) NULL
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'Name')
   AND EXISTS (SELECT * FROM ServerInformation WHERE [Name] IS NULL)
BEGIN
	UPDATE ServerInformation SET [Name] = 'Replace Me'

	ALTER TABLE ServerInformation ALTER COLUMN [Name] [varchar](255) NOT NULL

	PRINT('Added Name Field to ServerInformation Table')
END
ELSE
	PRINT('Name Field Already Exists on ServerInformation Table')
GO

UPDATE [Endpoint] SET [Value] = REPLACE([Value], 'https://api.hunter-industries.co.uk', '')

PRINT('Removed "https://api.hunter-industries.co.uk" from Endpoint Values')
GO

ALTER TABLE [Endpoint] ALTER COLUMN [Value] [varchar](50) NOT NULL

PRINT('Changed Endpoint Value Field to varchar(50)')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EndpointVersion' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[EndpointVersion](
		[EndpointVersionId] [int] IDENTITY(1,1) NOT NULL,
		[Value] [varchar](10) NOT NULL,
	 CONSTRAINT [PK_EndpointVersion] PRIMARY KEY CLUSTERED
	(
		[EndpointVersionId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('EndpointVersion Table Added')
END
ELSE
	PRINT('EndpointVersion Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[EndpointVersion] WHERE [Value] = 'v1.0')
	INSERT INTO EndpointVersion([Value]) VALUES ('v1.0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[EndpointVersion] WHERE [Value] = 'v1.1')
	INSERT INTO EndpointVersion([Value]) VALUES ('v1.1')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditHistory') AND name = 'EndpointVersionId')
	ALTER TABLE AuditHistory ADD EndpointVersionId [int] NULL
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditHistory') AND name = 'EndpointVersionId')
   AND EXISTS (SELECT * FROM AuditHistory WHERE EndpointVersionId IS NULL)
BEGIN
	UPDATE AuditHistory SET EndpointVersionId = CASE
	WHEN EndpointId = 1 THEN 1
	WHEN EndpointId = 2 THEN 1
	WHEN EndpointId = 3 THEN 1
	WHEN EndpointId = 4 THEN 1
	WHEN EndpointId = 5 THEN 1
	WHEN EndpointId = 6 THEN 1
	WHEN EndpointId = 7 AND MethodId in (1,2) THEN 1
	WHEN EndpointId = 7 AND MethodId in (3,4) THEN 2
	WHEN EndpointId = 8 THEN 2
	WHEN EndpointId = 9 THEN 2
	WHEN EndpointId = 10 THEN 2
	WHEN EndpointId = 11 THEN 2
	END

	ALTER TABLE AuditHistory ALTER COLUMN EndpointVersionId [int] NOT NULL

	PRINT('Added EndpointVersionId Field to AuditHistory Table')
END
ELSE
	PRINT('EndpointVersionId Field Already Exists on AuditHistory Table')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AuditHistory_EndpointVersion')
BEGIN
	ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_EndpointVersion] FOREIGN KEY([EndpointVersionId])
	REFERENCES [dbo].[EndpointVersion] ([EndpointVersionId])

	ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_EndpointVersion]

	PRINT('Added Foreign Key to EndpointVersionId Field')
END
ELSE
	PRINT('FK_AuditHistory_EndpointVersion Already Exists')
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Change_Endpoint')
BEGIN
	ALTER TABLE [dbo].[Change] DROP CONSTRAINT [FK_Change_Endpoint]

	PRINT('Dropped FK_Change_Endpoint Constraint')
END
ELSE
	PRINT('FK_Change_Endpoint Already Removed')
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Change') AND name = 'EndpointId')
BEGIN
	ALTER TABLE [Change] DROP COLUMN [EndpointId]

	PRINT('Removed EndpointId from Change Table')
END
ELSE
	PRINT('EndpointId Already Removed from Change Table')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[EndpointVersion] WHERE [Value] = 'v2.0')
BEGIN
	INSERT INTO EndpointVersion([Value]) VALUES ('v2.0')

	PRINT('Added Endpoint Version to EndpointVersion Table')
END
ELSE
	PRINT('v2.0 Endpoint Version Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = '/errorlog')
BEGIN
	INSERT INTO [Endpoint]([Value]) VALUES ('/errorlog')

	PRINT('Added Error Log Endpoint')
END
ELSE
	PRINT('Error Log Endpoint Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditHistory') AND name = 'UserId')
BEGIN
	ALTER TABLE AuditHistory ADD UserId [int] NULL

	PRINT('Added UserId Field to AuditHistory Table')
END
ELSE
	PRINT('UserId Field Already Exists on AuditHistory Table')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AuditHistory_APIUser')
BEGIN
	ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_APIUser] FOREIGN KEY([UserId])
	REFERENCES [dbo].[APIUser] ([UserId])

	ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_APIUser]

	PRINT('Added Foreign Key to UserId Field')
END
ELSE
	PRINT('FK_AuditHistory_APIUser Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditHistory') AND name = 'ApplicationId')
BEGIN
	ALTER TABLE AuditHistory ADD ApplicationId [int] NULL

	PRINT('Added ApplicationId Field to AuditHistory Table')
END
ELSE
	PRINT('ApplicationId Field Already Exists on AuditHistory Table')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AuditHistory_Application')
BEGIN
	ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_Application] FOREIGN KEY([ApplicationId])
	REFERENCES [dbo].[Application] ([ApplicationId])

	ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_Application]

	PRINT('Added Foreign Key to ApplicationId Field')
END
ELSE
	PRINT('FK_AuditHistory_Application Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = '/configuration')
BEGIN
	INSERT INTO [Endpoint]([Value]) VALUES ('/configuration')

	PRINT('Added Configuration Endpoint')
END
ELSE
	PRINT('Configuration Endpoint Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApplicationSetting' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[ApplicationSetting](
		[ApplicationSettingId] [int] IDENTITY(1,1) NOT NULL,
		[ApplicationId] [int] NOT NULL,
		[Name] [varchar](255) NOT NULL,
		[Type] [varchar](20) NOT NULL,
		[Required] [bit] NOT NULL,
		[IsDeleted] [bit] NOT NULL
	 CONSTRAINT [PK_ApplicationSetting] PRIMARY KEY CLUSTERED
	(
		[ApplicationSettingId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('ApplicationSetting Table Added')
END
ELSE
	PRINT('ApplicationSetting Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ApplicationSetting_Required')
BEGIN
	ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_Required]  DEFAULT ((0)) FOR [Required]
END
GO

IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ApplicationSetting_IsDeleted')
BEGIN
	ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ApplicationSetting_Application')
BEGIN
	ALTER TABLE [dbo].[ApplicationSetting]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationSetting_Application] FOREIGN KEY([ApplicationId])
	REFERENCES [dbo].[Application] ([ApplicationId])

	ALTER TABLE [dbo].[ApplicationSetting] CHECK CONSTRAINT [FK_ApplicationSetting_Application]
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Application') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE [Application] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

	PRINT('Added IsDeleted Field to Application Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on Application Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Authorisation') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE [Authorisation] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

	PRINT('Added IsDeleted Field to Authorisation Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on Authorisation Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Component') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE [Component] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

	PRINT('Added IsDeleted Field to Component Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on Component Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Connection') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE [Connection] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

	PRINT('Added IsDeleted Field to Connection Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on Connection Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Downtime') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE [Downtime] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

	PRINT('Added IsDeleted Field to Downtime Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on Downtime Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Game') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE [Game] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

	PRINT('Added IsDeleted Field to Game Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on Game Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Machine') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE [Machine] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

	PRINT('Added IsDeleted Field to Machine Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on Machine Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Downtime') AND name = 'Duration')
	ALTER TABLE Downtime ADD [Duration] [int] NULL
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Downtime') AND name = 'Duration')
   AND EXISTS (SELECT * FROM Downtime WHERE [Duration] IS NULL)
BEGIN
	UPDATE Downtime SET Duration = 0

	ALTER TABLE Downtime ALTER COLUMN [Duration] [int] NOT NULL

	PRINT('Added Duration Field to Downtime Table')
END
ELSE
	PRINT('Duration Field Already Exists on Downtime Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'EventInterval')
BEGIN
	ALTER TABLE [ServerInformation] ADD [EventInterval] [int] NOT NULL DEFAULT(300)

	PRINT('Added EventInterval Field to ServerInformation Table')
END
ELSE
	PRINT('EventInterval Field Already Exists on ServerInformation Table')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = '/statistic')
BEGIN
	INSERT INTO [Endpoint]([Value]) VALUES ('/statistic')

	PRINT('Added Statistic Endpoint')
END
ELSE
	PRINT('Statistic Endpoint Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.VersionHistory') AND name = 'ScriptName')
BEGIN
	ALTER TABLE [VersionHistory] ADD [ScriptName] [varchar](255) NOT NULL

	PRINT('Added ScriptName Field to VersionHistory Table')
END
ELSE
	PRINT('ScriptName Field Already Exists on VersionHistory Table')
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerAlert') AND name = 'UserSettingsId')
BEGIN
	EXEC sp_rename 'ServerAlert.UserSettingsId', 'UserSettingId', 'COLUMN'

	PRINT('Renamed ServerAlert.UserSettingsId to ServerAlert.UserSettingId')
END
ELSE
	PRINT('ServerAlert.UserSettingsId Already Renamed')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[StatusCode] WHERE [Value] = '204 No Content')
BEGIN
	BEGIN TRANSACTION;

	BEGIN TRY

	    ALTER TABLE [dbo].[AuditHistory] NOCHECK CONSTRAINT [FK_AuditHistory_StatusCode];

	    DELETE FROM [dbo].[StatusCode]
	    WHERE  [StatusId] >= 3;

	    SET IDENTITY_INSERT [dbo].[StatusCode] ON;

	    INSERT INTO [dbo].[StatusCode] ([StatusId], [Value]) VALUES (3, '204 No Content');
	    INSERT INTO [dbo].[StatusCode] ([StatusId], [Value]) VALUES (4, '400 Bad Request');
	    INSERT INTO [dbo].[StatusCode] ([StatusId], [Value]) VALUES (5, '401 Unauthorized');
	    INSERT INTO [dbo].[StatusCode] ([StatusId], [Value]) VALUES (6, '403 Forbidden');
	    INSERT INTO [dbo].[StatusCode] ([StatusId], [Value]) VALUES (7, '404 Not Found');
	    INSERT INTO [dbo].[StatusCode] ([StatusId], [Value]) VALUES (8, '500 Internal Server Error');

	    SET IDENTITY_INSERT [dbo].[StatusCode] OFF;

	    DBCC CHECKIDENT ('[dbo].[StatusCode]', RESEED, 8);

	    ALTER TABLE [dbo].[AuditHistory] WITH CHECK CHECK CONSTRAINT [FK_AuditHistory_StatusCode];

	    COMMIT TRANSACTION;

	END TRY
	BEGIN CATCH
	    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

	    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AuditHistory_StatusCode' AND is_disabled = 1)
	        ALTER TABLE [dbo].[AuditHistory] WITH CHECK CHECK CONSTRAINT [FK_AuditHistory_StatusCode];

	    THROW;
	END CATCH;

	PRINT('Added 204 No Content to StatusCode Table')
END
ELSE
	PRINT('StatusCode Table Already Updated')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditHistory') AND name = 'RequestBody')
BEGIN
	ALTER TABLE AuditHistory ADD [RequestBody] [varchar](max) NULL

	PRINT('Added RequestBody Field to AuditHistory Table')
END
ELSE
	PRINT('RequestBody Field Already Exists on AuditHistory Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditHistory') AND name = 'ResponseBody')
BEGIN
	ALTER TABLE AuditHistory ADD [ResponseBody] [varchar](max) NULL

	PRINT('Added ResponseBody Field to AuditHistory Table')
END
ELSE
	PRINT('ResponseBody Field Already Exists on AuditHistory Table')
GO

PRINT('Added VersionHistory Record')

IF NOT EXISTS (SELECT * FROM VersionHistory WHERE ReleaseVersion = '2.0.0')
	INSERT INTO VersionHistory(ReleaseVersion, ScriptName, DateUpdated)
	VALUES ('2.0.0', 'Database Upgrade 2.0.0', GETUTCDATE())
GO
