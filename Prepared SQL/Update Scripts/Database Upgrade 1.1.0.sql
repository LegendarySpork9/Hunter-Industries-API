USE [HunterIndustriesAPI]
GO

/****** Object:  Table [dbo].[Scope]    Script Date: 10/05/2025 20:22:36 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Scope' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[Scope](
		[ScopeID] [int] IDENTITY(1,1) NOT NULL,
		[Value] [varchar](255) NOT NULL,
	 CONSTRAINT [PK_Scope] PRIMARY KEY CLUSTERED
	(
		[ScopeID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Scope Table Added')
END
ELSE
	PRINT('Scope Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Scope] WHERE [Value] = 'User')
	INSERT [dbo].[Scope] ([Value]) VALUES ('User')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Scope] WHERE [Value] = 'Assistant API')
	INSERT [dbo].[Scope] ([Value]) VALUES ('Assistant API')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Scope] WHERE [Value] = 'Book Reader API')
	INSERT [dbo].[Scope] ([Value]) VALUES ('Book Reader API')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Scope] WHERE [Value] = 'Control Panel API')
	INSERT [dbo].[Scope] ([Value]) VALUES ('Control Panel API')
GO

/****** Object:  Table [dbo].[UserScope]    Script Date: 10/05/2025 20:22:36 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserScope' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[UserScope](
		[UserScopeID] [int] IDENTITY(1,1) NOT NULL,
		[UserID] [int] NOT NULL,
		[ScopeID] [int] NOT NULL,
	 CONSTRAINT [PK_UserScope] PRIMARY KEY CLUSTERED
	(
		[UserScopeID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('UserScope Table Added')
END
ELSE
	PRINT('UserScope Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserScope_Scope')
BEGIN
	ALTER TABLE [dbo].[UserScope]  WITH CHECK ADD  CONSTRAINT [FK_UserScope_Scope] FOREIGN KEY([ScopeID])
	REFERENCES [dbo].[Scope] ([ScopeID])

	ALTER TABLE [dbo].[UserScope] CHECK CONSTRAINT [FK_UserScope_Scope]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserScope_APIUser')
BEGIN
	ALTER TABLE [dbo].[UserScope]  WITH CHECK ADD  CONSTRAINT [FK_UserScope_APIUser] FOREIGN KEY([UserID])
	REFERENCES [dbo].[APIUser] ([UserID])

	ALTER TABLE [dbo].[UserScope] CHECK CONSTRAINT [FK_UserScope_APIUser]
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.APIUser') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE APIUser ADD [IsDeleted] [bit] NOT NULL DEFAULT 0

	PRINT('Added IsDeleted Field to APIUser Table')
END
ELSE
	PRINT('IsDeleted Field Already Exists on APIUser Table')
GO

ALTER TABLE Method ALTER COLUMN [Value] [varchar](6)
GO

IF NOT EXISTS (SELECT * FROM Method WHERE [Value] = 'DELETE')
BEGIN
	INSERT INTO Method ([Value]) VALUES ('DELETE')

	PRINT('Added "Delete" Value to Method Table')
END
ELSE
	PRINT('"Delete" Value Already Exists in Method Table')
GO

/****** Object:  Table [dbo].[Component]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Component' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[Component](
		[ComponentID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](14) NOT NULL,
	 CONSTRAINT [PK_Component] PRIMARY KEY CLUSTERED
	(
		[ComponentID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Component Table Added')
END
ELSE
	PRINT('Component Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Component] WHERE [Name] = 'PC Status')
	INSERT [dbo].[Component] ([Name]) VALUES ('PC Status')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Component] WHERE [Name] = 'Hamachi Status')
	INSERT [dbo].[Component] ([Name]) VALUES ('Hamachi Status')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Component] WHERE [Name] = 'Server Status')
	INSERT [dbo].[Component] ([Name]) VALUES ('Server Status')
GO

/****** Object:  Table [dbo].[ComponentStatus]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComponentStatus' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[ComponentStatus](
		[ComponentStatusID] [int] IDENTITY(1,1) NOT NULL,
		[Value] [varchar](7) NOT NULL,
	 CONSTRAINT [PK_ComponentStatus] PRIMARY KEY CLUSTERED
	(
		[ComponentStatusID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('ComponentStatus Table Added')
END
ELSE
	PRINT('ComponentStatus Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ComponentStatus] WHERE [Value] = 'Online')
	INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Online')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[ComponentStatus] WHERE [Value] = 'Offline')
	INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Offline')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[ComponentStatus] WHERE [Value] = 'Unknown')
	INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Unknown')
GO

/****** Object:  Table [dbo].[Game]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Game' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[Game](
		[GameID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](255) NOT NULL,
	 CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED
	(
		[GameID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Game Table Added')
END
ELSE
	PRINT('Game Table Already Exists')
GO

/****** Object:  Table [dbo].[Machine]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Machine' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[Machine](
		[MachineID] [int] IDENTITY(1,1) NOT NULL,
		[HostName] [varchar](255) NOT NULL,
	 CONSTRAINT [PK_Machine] PRIMARY KEY CLUSTERED
	(
		[MachineID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Machine Table Added')
END
ELSE
	PRINT('Machine Table Already Exists')
GO

/****** Object:  Table [dbo].[ServerAlertStatus]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ServerAlertStatus' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[ServerAlertStatus](
		[AlertStatusID] [int] IDENTITY(1,1) NOT NULL,
		[Value] [varchar](13) NOT NULL,
	 CONSTRAINT [PK_ServerAlertStatus] PRIMARY KEY CLUSTERED
	(
		[AlertStatusID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('ServerAlertStatus Table Added')
END
ELSE
	PRINT('ServerAlertStatus Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ServerAlertStatus] WHERE [Value] = 'Reported')
	INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Reported')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[ServerAlertStatus] WHERE [Value] = 'Investigating')
	INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Investigating')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[ServerAlertStatus] WHERE [Value] = 'Resolved')
	INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Resolved')
GO

/****** Object:  Table [dbo].[ServerInformation]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ServerInformation' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[ServerInformation](
		[ServerInformationID] [int] IDENTITY(1,1) NOT NULL,
		[MachineID] [int] NOT NULL,
		[GameID] [int] NOT NULL,
		[GameVersion] [varchar](20) NOT NULL,
		[IPAddress] [varchar](13) NOT NULL,
		[IsActive] [bit] NOT NULL,
	 CONSTRAINT [PK_ServerInformation] PRIMARY KEY CLUSTERED
	(
		[ServerInformationID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('ServerInformation Table Added')
END
ELSE
	PRINT('ServerInformation Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ServerInformation_IsActive')
BEGIN
	ALTER TABLE [dbo].[ServerInformation] ADD  CONSTRAINT [DF_ServerInformation_IsActive]  DEFAULT ((0)) FOR [IsActive]

	PRINT('ServerInformation IsActive Default Added')
END
GO

/****** Object:  Table [dbo].[ComponentInformation]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComponentInformation' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[ComponentInformation](
		[ComponentInformationID] [int] IDENTITY(1,1) NOT NULL,
		[ServerInformationID] [int] NOT NULL,
		[ComponentID] [int] NOT NULL,
		[ComponentStatusID] [int] NOT NULL,
		[DateOccured] [datetime] NOT NULL,
	 CONSTRAINT [PK_ComponentInformation] PRIMARY KEY CLUSTERED
	(
		[ComponentInformationID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('ComponentInformation Table Added')
END
ELSE
	PRINT('ComponentInformation Table Already Exists')
GO

/****** Object:  Table [dbo].[UserSettings]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserSettings' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[UserSettings](
		[UserSettingsID] [int] IDENTITY(1,1) NOT NULL,
		[UserID] [int] NOT NULL,
		[ApplicationID] [int] NOT NULL,
		[Name] [varchar](255) NOT NULL,
		[Value] [varchar](255) NOT NULL,
	 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED
	(
		[UserSettingsID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('UserSettings Table Added')
END
ELSE
	PRINT('UserSettings Table Already Exists')
GO

/****** Object:  Table [dbo].[ServerAlerts]    Script Date: 17/05/2025 12:29:22 ******/
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ServerAlerts' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[ServerAlerts](
		[ServerAlertsID] [int] IDENTITY(1,1) NOT NULL,
		[ServerInformationID] [int] NOT NULL,
		[UserSettingsID] [int] NOT NULL,
		[ComponentID] [int] NOT NULL,
		[ComponentStatusID] [int] NOT NULL,
		[AlertStatusID] [int] NOT NULL,
		[DateOccured] [datetime] NOT NULL,
	 CONSTRAINT [PK_ServerAlerts] PRIMARY KEY CLUSTERED
	(
		[ServerAlertsID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('ServerAlerts Table Added')
END
ELSE
	PRINT('ServerAlerts Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ComponentInformation_Component')
BEGIN
	ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_Component] FOREIGN KEY([ComponentID])
	REFERENCES [dbo].[Component] ([ComponentID])

	ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_Component]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ComponentInformation_ComponentStatus')
BEGIN
	ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_ComponentStatus] FOREIGN KEY([ComponentStatusID])
	REFERENCES [dbo].[ComponentStatus] ([ComponentStatusID])

	ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_ComponentStatus]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ComponentInformation_ServerInformation')
BEGIN
	ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_ServerInformation] FOREIGN KEY([ServerInformationID])
	REFERENCES [dbo].[ServerInformation] ([ServerInformationID])

	ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_ServerInformation]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerAlerts_Component')
BEGIN
	ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_Component] FOREIGN KEY([ComponentID])
	REFERENCES [dbo].[Component] ([ComponentID])

	ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_Component]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerAlerts_ComponentStatus')
BEGIN
	ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_ComponentStatus] FOREIGN KEY([ComponentStatusID])
	REFERENCES [dbo].[ComponentStatus] ([ComponentStatusID])

	ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_ComponentStatus]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerAlerts_ServerAlertStatus')
BEGIN
	ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_ServerAlertStatus] FOREIGN KEY([AlertStatusID])
	REFERENCES [dbo].[ServerAlertStatus] ([AlertStatusID])

	ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_ServerAlertStatus]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerAlerts_ServerInformation')
BEGIN
	ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_ServerInformation] FOREIGN KEY([ServerInformationID])
	REFERENCES [dbo].[ServerInformation] ([ServerInformationID])

	ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_ServerInformation]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerAlerts_UserSettings')
BEGIN
	ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_UserSettings] FOREIGN KEY([UserSettingsID])
	REFERENCES [dbo].[UserSettings] ([UserSettingsID])

	ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_UserSettings]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerInformation_Game')
BEGIN
	ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Game] FOREIGN KEY([GameID])
	REFERENCES [dbo].[Game] ([GameID])

	ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Game]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerInformation_Machine')
BEGIN
	ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Machine] FOREIGN KEY([MachineID])
	REFERENCES [dbo].[Machine] ([MachineID])

	ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Machine]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserSettings_APIUser')
BEGIN
	ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_APIUser] FOREIGN KEY([UserID])
	REFERENCES [dbo].[APIUser] ([UserID])

	ALTER TABLE [dbo].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_APIUser]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserSettings_Application')
BEGIN
	ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_Application] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[Application] ([ApplicationID])

	ALTER TABLE [dbo].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_Application]
END
GO

PRINT('Foreign Keys Added')

IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = 'https://hunter-industries.co.uk/api/usersettings')
	INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/usersettings')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = 'https://hunter-industries.co.uk/api/serverstatus/serverinformation')
	INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverinformation')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = 'https://hunter-industries.co.uk/api/serverstatus/serverevent')
	INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverevent')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[Endpoint] WHERE [Value] = 'https://hunter-industries.co.uk/api/serverstatus/serveralert')
	INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serveralert')
GO

PRINT('Added New Endpoints')

IF NOT EXISTS (SELECT * FROM [dbo].[Scope] WHERE [Value] = 'Server Status API')
	INSERT [dbo].[Scope] ([Value]) VALUES ('Server Status API')
GO

PRINT('Added New Scope')
