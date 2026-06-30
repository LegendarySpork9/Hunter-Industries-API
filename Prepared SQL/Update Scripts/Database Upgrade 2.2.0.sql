USE [HunterIndustriesAPI]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioFilter' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioFilter](
		[PortfolioFilterId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](50) NOT NULL,
		[Values] [varchar](max) NOT NULL,
 	 CONSTRAINT [PK_PortfolioFilter] PRIMARY KEY CLUSTERED 
	(
		[PortfolioFilterId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	PRINT('PortfolioFilter Table Added')
END
ELSE
	PRINT('PortfolioFilter Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioItemType' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioItemType](
		[PortfolioItemTypeId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](50) NOT NULL,
 	 CONSTRAINT [PK_PortfolioItemType] PRIMARY KEY CLUSTERED 
	(
		[PortfolioItemTypeId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('PortfolioItemType Table Added')
END
ELSE
	PRINT('PortfolioItemType Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Framework' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Framework](
		[FrameworkId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](50) NOT NULL,
 	 CONSTRAINT [PK_Framework] PRIMARY KEY CLUSTERED 
	(
		[FrameworkId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Framework Table Added')
END
ELSE
	PRINT('Framework Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Language' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Language](
		[LanguageId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](50) NOT NULL,
 	 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
	(
		[LanguageId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Language Table Added')
END
ELSE
	PRINT('Language Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Environment' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Environment](
		[EnvironmentId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](50) NOT NULL,
 	 CONSTRAINT [PK_Environment] PRIMARY KEY CLUSTERED 
	(
		[EnvironmentId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Environment Table Added')
END
ELSE
	PRINT('Environment Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LLMCompany' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[LLMCompany](
		[LLMCompanyId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](20) NOT NULL,
 	 CONSTRAINT [PK_LLMCompany] PRIMARY KEY CLUSTERED 
	(
		[LLMCompanyId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('LLMCompany Table Added')
END
ELSE
	PRINT('LLMCompany Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioItem' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioItem](
		[PortfolioItemId] [int] IDENTITY(1,1) NOT NULL,
		[TypeId] [int] NOT NULL,
		[LLMModelId] [int] NULL,
		[Name] [varchar](255) NOT NULL,
		[Summary] [varchar](255) NOT NULL,
		[Description] [varchar](max) NOT NULL,
		[IconURL] [varchar](255) NOT NULL,
		[ReleaseNotes] [varchar](max) NOT NULL,
		[GitHubLink] [varchar](255) NOT NULL,
		[DemoLink] [varchar](255) NULL,
		[UnitTestCoverage] [decimal](5, 2) NULL,
		[LLMUsageNotes] [varchar](255) NULL,
		[DateCreated] [datetime] NOT NULL,
		[DateUpdated] [datetime] NOT NULL,
	 CONSTRAINT [PK_PortfolioItem] PRIMARY KEY CLUSTERED 
	(
		[PortfolioItemId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	PRINT('PortfolioItem Table Added')
END
ELSE
	PRINT('PortfolioItem Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LLMModel' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[LLMModel](
		[LLMModelId] [int] IDENTITY(1,1) NOT NULL,
		[LLMCompanyId] [int] NOT NULL,
		[Name] [varchar](50) NOT NULL,
	 CONSTRAINT [PK_LLMModel] PRIMARY KEY CLUSTERED 
	(
		[LLMModelId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('LLMModel Table Added')
END
ELSE
	PRINT('LLMModel Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioItemImage' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioItemImage](
		[PortfolioItemId] [int] NOT NULL,
		[MediaId] [int] NOT NULL
	) ON [PRIMARY]

	PRINT('PortfolioItemImage Table Added')
END
ELSE
	PRINT('PortfolioItemImage Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioItemBuildHistory' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioItemBuildHistory](
		[PortfolioItemBuildHistoryId] [int] IDENTITY(1,1) NOT NULL,
		[PortfolioItemId] [int] NOT NULL,
		[Version] [varchar](10) NOT NULL,
		[ReleaseDate] [datetime] NOT NULL,
	 CONSTRAINT [PK_PortfolioItemBuildHistory] PRIMARY KEY CLUSTERED 
	(
		[PortfolioItemBuildHistoryId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('PortfolioItemImage Table Added')
END
ELSE
	PRINT('PortfolioItemBuildHistory Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioItemFramework' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioItemFramework](
		[PortfolioItemId] [int] NOT NULL,
		[FrameworkId] [int] NOT NULL
	) ON [PRIMARY]

	PRINT('PortfolioItemImage Table Added')
END
ELSE
	PRINT('PortfolioItemFramework Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioItemLanguage' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioItemLanguage](
		[PortfolioItemId] [int] NOT NULL,
		[LanguageId] [int] NOT NULL
	) ON [PRIMARY]

	PRINT('PortfolioItemImage Table Added')
END
ELSE
	PRINT('PortfolioItemLanguage Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortfolioItemEnvironment' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[PortfolioItemEnvironment](
		[PortfolioItemId] [int] NOT NULL,
		[EnvironmentId] [int] NOT NULL
	) ON [PRIMARY]

	PRINT('PortfolioItemImage Table Added')
END
ELSE
	PRINT('PortfolioItemEnvironment Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItem_LLMModel')
BEGIN
	ALTER TABLE [dbo].[PortfolioItem]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItem_LLMModel] FOREIGN KEY([LLMModelId])
	REFERENCES [dbo].[LLMModel] ([LLMModelId])
	
	ALTER TABLE [dbo].[PortfolioItem] CHECK CONSTRAINT [FK_PortfolioItem_LLMModel]

	PRINT('Added Foreign Key to LLMModelId Field in PortfolioItem Table')
END
ELSE
	PRINT('FK_PortfolioItem_LLMModel Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItem_PortfolioItemType')
BEGIN
	ALTER TABLE [dbo].[PortfolioItem]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItem_PortfolioItemType] FOREIGN KEY([TypeId])
	REFERENCES [dbo].[PortfolioItemType] ([PortfolioItemTypeId])
	
	ALTER TABLE [dbo].[PortfolioItem] CHECK CONSTRAINT [FK_PortfolioItem_PortfolioItemType]

	PRINT('Added Foreign Key to PortfolioItemTypeId Field in PortfolioItem Table')
END
ELSE
	PRINT('FK_PortfolioItem_PortfolioItemType Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LLMModel_LLMCompany')
BEGIN
	ALTER TABLE [dbo].[LLMModel]  WITH CHECK ADD  CONSTRAINT [FK_LLMModel_LLMCompany] FOREIGN KEY([LLMCompanyId])
	REFERENCES [dbo].[LLMCompany] ([LLMCompanyId])
	
	ALTER TABLE [dbo].[LLMModel] CHECK CONSTRAINT [FK_LLMModel_LLMCompany]

	PRINT('Added Foreign Key to LLMCompanyId Field in LLMModel Table')
END
ELSE
	PRINT('FK_LLMModel_LLMCompany Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemImage_Media')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemImage]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemImage_Media] FOREIGN KEY([MediaId])
	REFERENCES [dbo].[Media] ([MediaId])
	
	ALTER TABLE [dbo].[PortfolioItemImage] CHECK CONSTRAINT [FK_PortfolioItemImage_Media]

	PRINT('Added Foreign Key to MediaId Field in PortfolioItemImage Table')
END
ELSE
	PRINT('FK_PortfolioItemImage_Media Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemImage_PortfolioItem')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemImage]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemImage_PortfolioItem] FOREIGN KEY([PortfolioItemId])
	REFERENCES [dbo].[PortfolioItem] ([PortfolioItemId])
	
	ALTER TABLE [dbo].[PortfolioItemImage] CHECK CONSTRAINT [FK_PortfolioItemImage_PortfolioItem]

	PRINT('Added Foreign Key to PortfolioItemId Field in PortfolioItemImage Table')
END
ELSE
	PRINT('FK_PortfolioItemImage_PortfolioItem Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemBuildHistory_PortfolioItem')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemBuildHistory]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemBuildHistory_PortfolioItem] FOREIGN KEY([PortfolioItemId])
	REFERENCES [dbo].[PortfolioItem] ([PortfolioItemId])
	
	ALTER TABLE [dbo].[PortfolioItemBuildHistory] CHECK CONSTRAINT [FK_PortfolioItemBuildHistory_PortfolioItem]

	PRINT('Added Foreign Key to PortfolioItemId Field in PortfolioItemBuildHistory Table')
END
ELSE
	PRINT('FK_PortfolioItemBuildHistory_PortfolioItem Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemFramework_Framework')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemFramework]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemFramework_Framework] FOREIGN KEY([FrameworkId])
	REFERENCES [dbo].[Framework] ([FrameworkId])
	
	ALTER TABLE [dbo].[PortfolioItemFramework] CHECK CONSTRAINT [FK_PortfolioItemFramework_Framework]

	PRINT('Added Foreign Key to FrameworkId Field in PortfolioItemFramework Table')
END
ELSE
	PRINT('FK_PortfolioItemFramework_Framework Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemFramework_PortfolioItem')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemFramework]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemFramework_PortfolioItem] FOREIGN KEY([PortfolioItemId])
	REFERENCES [dbo].[PortfolioItem] ([PortfolioItemId])
	
	ALTER TABLE [dbo].[PortfolioItemFramework] CHECK CONSTRAINT [FK_PortfolioItemFramework_PortfolioItem]

	PRINT('Added Foreign Key to PortfolioItemId Field in PortfolioItemFramework Table')
END
ELSE
	PRINT('FK_PortfolioItemFramework_PortfolioItem Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemLanguage_Language')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemLanguage]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemLanguage_Language] FOREIGN KEY([LanguageId])
	REFERENCES [dbo].[Language] ([LanguageId])
	
	ALTER TABLE [dbo].[PortfolioItemLanguage] CHECK CONSTRAINT [FK_PortfolioItemLanguage_Language]

	PRINT('Added Foreign Key to LanguageId Field in PortfolioItemLanguage Table')
END
ELSE
	PRINT('FK_PortfolioItemLanguage_Language Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemLanguage_PortfolioItem')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemLanguage]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemLanguage_PortfolioItem] FOREIGN KEY([PortfolioItemId])
	REFERENCES [dbo].[PortfolioItem] ([PortfolioItemId])
	
	ALTER TABLE [dbo].[PortfolioItemLanguage] CHECK CONSTRAINT [FK_PortfolioItemLanguage_PortfolioItem]

	PRINT('Added Foreign Key to PortfolioItemId Field in PortfolioItemLanguage Table')
END
ELSE
	PRINT('FK_PortfolioItemLanguage_PortfolioItem Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemEnvironment_Environment')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemEnvironment_Environment] FOREIGN KEY([EnvironmentId])
	REFERENCES [dbo].[Environment] ([EnvironmentId])
	
	ALTER TABLE [dbo].[PortfolioItemEnvironment] CHECK CONSTRAINT [FK_PortfolioItemEnvironment_Environment]

	PRINT('Added Foreign Key to EnvironmentId Field in PortfolioItemEnvironment Table')
END
ELSE
	PRINT('FK_PortfolioItemEnvironment_Environment Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PortfolioItemEnvironment_PortfolioItem')
BEGIN
	ALTER TABLE [dbo].[PortfolioItemEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_PortfolioItemEnvironment_PortfolioItem] FOREIGN KEY([PortfolioItemId])
	REFERENCES [dbo].[PortfolioItem] ([PortfolioItemId])
	
	ALTER TABLE [dbo].[PortfolioItemEnvironment] CHECK CONSTRAINT [FK_PortfolioItemEnvironment_PortfolioItem]

	PRINT('Added Foreign Key to PortfolioItemId Field in PortfolioItemEnvironment Table')
END
ELSE
	PRINT('FK_PortfolioItemEnvironment_PortfolioItem Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[EndpointVersion] WHERE [Value] = 'v2.2')
BEGIN
	INSERT INTO EndpointVersion([Value]) VALUES ('v2.2')

	PRINT('Added Endpoint Version to EndpointVersion Table')
END
ELSE
	PRINT('v2.2 Endpoint Version Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = '/portfolio')
BEGIN
	INSERT INTO [Endpoint]([Value]) VALUES ('/portfolio')

	PRINT('Added Portfolio Endpoint')
END
ELSE
	PRINT('Portfolio Endpoint Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = '/portfolio/filter')
BEGIN
	INSERT INTO [Endpoint]([Value]) VALUES ('/portfolio')

	PRINT('Added Portfolio Endpoint')
END
ELSE
	PRINT('Portfolio Endpoint Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Scope] WHERE [Value] = 'Portfolio API')
BEGIN
	INSERT INTO [Scope]([Value]) VALUES ('Portfolio API')

	PRINT('Added Portfolio Scope')
END
ELSE
	PRINT('Portfolio Scope Already Exists')
GO

IF NOT EXISTS (SELECT * FROM VersionHistory WHERE ReleaseVersion = '2.2.0')
	INSERT INTO VersionHistory(ReleaseVersion, ScriptName, DateUpdated)
	VALUES ('2.2.0', 'Database Upgrade 2.2.0', GETUTCDATE())

	PRINT('Added VersionHistory Record')
GO
