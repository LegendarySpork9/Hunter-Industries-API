USE [HunterIndustriesAPI]
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'WebhookURL')
	ALTER TABLE ServerInformation ADD [WebhookURL] [varchar](200) NULL
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'Name')
   AND EXISTS (SELECT * FROM ServerInformation WHERE [WebhookURL] IS NULL)
BEGIN
	UPDATE ServerInformation SET [WebhookURL] = 'Replace Me'

	ALTER TABLE ServerInformation ALTER COLUMN [WebhookURL] [varchar](200) NOT NULL

	PRINT('ServerInformation WebhookURL Column Added')
END
ELSE
	PRINT('ServerInformation WebhookURL Column Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'RecipientId')
	ALTER TABLE ServerInformation ADD [RecipientId] [bigint] NULL
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'RecipientId')
   AND EXISTS (SELECT * FROM ServerInformation WHERE [RecipientId] IS NULL)
BEGIN
	UPDATE ServerInformation SET [RecipientId] = 0

	ALTER TABLE ServerInformation ALTER COLUMN [RecipientId] [bigint] NOT NULL

	PRINT('ServerInformation RecipientId Column Added')
END
ELSE
	PRINT('ServerInformation RecipientId Column Already Exists')
GO

IF NOT EXISTS (SELECT * FROM VersionHistory WHERE ReleaseVersion = '2.3.0')
	INSERT INTO VersionHistory(ReleaseVersion, ScriptName, DateUpdated)
	VALUES ('2.3.0', 'Database Upgrade 2.3.0', GETUTCDATE())

	PRINT('Added VersionHistory Record')
GO