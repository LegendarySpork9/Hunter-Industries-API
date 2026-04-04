USE [HunterIndustriesAPI]
GO

CREATE TABLE [dbo].[VersionHistory](
	[VersionId] [int] IDENTITY(1,1) NOT NULL,
	[ReleaseVersion] [varchar](11) NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_VersionHistory] PRIMARY KEY CLUSTERED 
(
	[VersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

PRINT('VersionHistory Table Added')

UPDATE [Endpoint] SET [Value] = REPLACE([Value], 'hunter-industries.co.uk/api', 'api.hunter-industries.co.uk')

PRINT('Changed URL Values')

UPDATE Component SET [Name] = REPLACE([Name], ' Status', '')

PRINT('Removed "Status" from Component Names')

ALTER TABLE ServerInformation ADD [Name] [varchar](255) NULL
GO

UPDATE ServerInformation SET [Name] = 'Replace Me'

ALTER TABLE ServerInformation ALTER COLUMN [Name] [varchar](255) NOT NULL

PRINT('Added Name Field to ServerInformation Table')

UPDATE [Endpoint] SET [Value] = REPLACE([Value], 'https://api.hunter-industries.co.uk', '')

PRINT('Removed "https://api.hunter-industries.co.uk" from Endpoint Values')

ALTER TABLE [Endpoint] ALTER COLUMN [Value] [varchar](50) NOT NULL

PRINT('Removed "https://api.hunter-industries.co.uk" from Endpoint Values')

CREATE TABLE [dbo].[EndpointVersion](
	[EndpointVersionId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](10) NOT NULL,
 CONSTRAINT [PK_EndpointVersion] PRIMARY KEY CLUSTERED 
(
	[EndpointVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO EndpointVersion([Value])
VALUES ('v1.0'),
('v1.1')

PRINT('EndpointVersion Table Added')

ALTER TABLE AuditHistory ADD EndpointVersionId [int] NULL
GO

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

ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_EndpointVersion] FOREIGN KEY([EndpointVersionId])
REFERENCES [dbo].[EndpointVersion] ([EndpointVersionId])
GO

ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_EndpointVersion]
GO

PRINT('Added Foreign Key to EndpointVersionId Field')

ALTER TABLE [dbo].[Change] DROP CONSTRAINT [FK_Change_Endpoint]
GO

ALTER TABLE [Change] DROP COLUMN [EndpointId]

PRINT('Removed EndpointId from Change Table')

INSERT INTO EndpointVersion([Value])
VALUES ('v2.0')

PRINT('Added Endpoint Version to EndpointVersion Table')

INSERT INTO [Endpoint]([Value])
VALUES ('/errorlog')

PRINT('Added Error Log Endpoint')

ALTER TABLE AuditHistory ADD UserId [int] NULL
GO

PRINT('Added UserId Field to AuditHistory Table')

ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_APIUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[APIUser] ([UserId])
GO

ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_APIUser]
GO

PRINT('Added Foreign Key to UserId Field')

ALTER TABLE AuditHistory ADD ApplicationId [int] NULL
GO

PRINT('Added ApplicationId Field to AuditHistory Table')

ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO

ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_Application]
GO

PRINT('Added Foreign Key to ApplicationId Field')

INSERT INTO [Endpoint]([Value])
VALUES ('/configuration')

PRINT('Added Configuration Endpoint')

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
GO

ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_Required]  DEFAULT ((0)) FOR [Required]
GO

ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[ApplicationSetting]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationSetting_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO

ALTER TABLE [dbo].[ApplicationSetting] CHECK CONSTRAINT [FK_ApplicationSetting_Application]
GO

PRINT('ApplicationSetting Table Added')

ALTER TABLE [Application] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

PRINT('Added IsDeleted Field to Application Table')

ALTER TABLE [Authorisation] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

PRINT('Added IsDeleted Field to Authorisation Table')

ALTER TABLE [Component] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

PRINT('Added IsDeleted Field to Component Table')

ALTER TABLE [Connection] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

PRINT('Added IsDeleted Field to Connection Table')

ALTER TABLE [Downtime] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

PRINT('Added IsDeleted Field to Downtime Table')

ALTER TABLE [Game] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

PRINT('Added IsDeleted Field to Game Table')

ALTER TABLE [Machine] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)

PRINT('Added IsDeleted Field to Machine Table')

ALTER TABLE Downtime ADD [Duration] [int] NULL
GO

UPDATE Downtime SET Duration = 0

ALTER TABLE Downtime ALTER COLUMN [Duration] [int] NOT NULL

PRINT('Added Duration Field to Downtime Table')

ALTER TABLE [ServerInformation] ADD [EventInterval] [int] NOT NULL DEFAULT(300)

PRINT('Added EventInterval Field to ServerInformation Table')

INSERT INTO VersionHistory(ReleaseVersion, DateUpdated)
VALUES ('2.0.0', GETUTCDATE())

PRINT('Added VersionHistory Record')