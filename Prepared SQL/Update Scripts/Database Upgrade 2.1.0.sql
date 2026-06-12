USE [HunterIndustriesAPI]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Domain' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Domain](
		[DomainId] [int] IDENTITY(1,1) NOT NULL,
		[Host] [varchar](255) NOT NULL,
		[IsDeleted] [bit] NOT NULL,
 	CONSTRAINT [PK_Domain] PRIMARY KEY CLUSTERED 
	(
		[DomainId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Domain Table Added')
END
ELSE
	PRINT('Domain Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MediaType' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[MediaType](
		[MediaTypeId] [int] IDENTITY(1,1) NOT NULL,
		[Extension] [varchar](10) NOT NULL,
		[MimeType] [varchar](100) NOT NULL,
	 CONSTRAINT [PK_MediaType] PRIMARY KEY CLUSTERED 
	(
		[MediaTypeId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('MediaType Table Added')
END
ELSE
	PRINT('MediaType Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Media' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Media](
		[MediaId] [int] IDENTITY(1,1) NOT NULL,
		[MediaTypeId] [int] NOT NULL,
		[DomainId] [int] NOT NULL,
		[ApplicationId] [int] NOT NULL,
		[Name] [varchar](255) NOT NULL,
		[Size] [bigint] NOT NULL,
		[Path] [varchar](400) NULL,
		[DateUploaded] [datetime] NOT NULL,
		[DateUpdated] [datetime] NOT NULL,
		[IsDeleted] [bit] NOT NULL,
	 CONSTRAINT [PK_Media] PRIMARY KEY CLUSTERED 
	(
		[MediaId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('MediaType Table Added')
END
ELSE
	PRINT('MediaType Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Media_Application')
BEGIN
	ALTER TABLE [dbo].[Media]  WITH CHECK ADD  CONSTRAINT [FK_Media_Application] FOREIGN KEY([ApplicationId])
	REFERENCES [dbo].[Application] ([ApplicationId])

	ALTER TABLE [dbo].[Media] CHECK CONSTRAINT [FK_Media_Application]

	PRINT('Added Media Foreign Key to ApplicationId Field')
END
ELSE
	PRINT('FK_Media_Application Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Media_Domain')
BEGIN
	ALTER TABLE [dbo].[Media]  WITH CHECK ADD  CONSTRAINT [FK_Media_Domain] FOREIGN KEY([DomainId])
	REFERENCES [dbo].[Domain] ([DomainId])

	ALTER TABLE [dbo].[Media] CHECK CONSTRAINT [FK_Media_Domain]

	PRINT('Added Foreign Key to DomainId Field')
END
ELSE
	PRINT('FK_Media_Domain Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Media_MediaType')
BEGIN
	ALTER TABLE [dbo].[Media]  WITH CHECK ADD  CONSTRAINT [FK_Media_MediaType] FOREIGN KEY([MediaTypeId])
	REFERENCES [dbo].[MediaType] ([MediaTypeId])

	ALTER TABLE [dbo].[Media] CHECK CONSTRAINT [FK_Media_MediaType]

	PRINT('Added Foreign Key to MediaTypeId Field')
END
ELSE
	PRINT('FK_Media_MediaType Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[EndpointVersion] WHERE [Value] = 'v2.1')
BEGIN
	INSERT INTO EndpointVersion([Value]) VALUES ('v2.1')

	PRINT('Added Endpoint Version to EndpointVersion Table')
END
ELSE
	PRINT('v2.1 Endpoint Version Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = '/media')
BEGIN
	INSERT INTO [Endpoint]([Value]) VALUES ('/media')

	PRINT('Added Media Endpoint')
END
ELSE
	PRINT('Media Endpoint Already Exists')
GO

IF NOT EXISTS (SELECT * FROM VersionHistory WHERE ReleaseVersion = '2.1.0')
	INSERT INTO VersionHistory(ReleaseVersion, ScriptName, DateUpdated)
	VALUES ('2.1.0', 'Database Upgrade 2.1.0', GETUTCDATE())

	PRINT('Added VersionHistory Record')
GO
