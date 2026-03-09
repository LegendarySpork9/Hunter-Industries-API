USE [HunterIndustriesAPI]
GO
/****** Object:  Table [dbo].[Scope]    Script Date: 10/05/2025 20:22:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Scope](
	[ScopeID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Scope] PRIMARY KEY CLUSTERED 
(
	[ScopeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT [dbo].[Scope] ([Value]) VALUES ('User')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('Assistant API')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('Book Reader API')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('Control Panel API')
GO

PRINT('Scope Table Added')

/****** Object:  Table [dbo].[UserScope]    Script Date: 10/05/2025 20:22:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserScope](
	[UserScopeID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ScopeID] [int] NOT NULL,
 CONSTRAINT [PK_UserScope] PRIMARY KEY CLUSTERED 
(
	[UserScopeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserScope]  WITH CHECK ADD  CONSTRAINT [FK_UserScope_Scope] FOREIGN KEY([ScopeID])
REFERENCES [dbo].[Scope] ([ScopeID])
GO
ALTER TABLE [dbo].[UserScope] CHECK CONSTRAINT [FK_UserScope_Scope]
GO
ALTER TABLE [dbo].[UserScope]  WITH CHECK ADD  CONSTRAINT [FK_UserScope_APIUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[APIUser] ([UserID])
GO
ALTER TABLE [dbo].[UserScope] CHECK CONSTRAINT [FK_UserScope_APIUser]
GO

PRINT('UserScope Table Added')

ALTER TABLE APIUser ADD [IsDeleted] [bit] NOT NULL DEFAULT 0
GO

PRINT('Added IsDeleted Field to APIUser Table')

ALTER TABLE Method ALTER COLUMN [Value] [varchar](6)
GO

INSERT INTO Method ([Value]) VALUES ('DELETE')
GO

PRINT('Added "Delete" Value to Method Table')

/****** Object:  Table [dbo].[Component]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Component](
	[ComponentID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](14) NOT NULL,
 CONSTRAINT [PK_Component] PRIMARY KEY CLUSTERED 
(
	[ComponentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT [dbo].[Component] ([Name]) VALUES ('PC Status')
GO
INSERT [dbo].[Component] ([Name]) VALUES ('Hamachi Status')
GO
INSERT [dbo].[Component] ([Name]) VALUES ('Server Status')
GO

PRINT('Component Table Added')

/****** Object:  Table [dbo].[ComponentInformation]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO

PRINT('ComponentInformation Table Added')

/****** Object:  Table [dbo].[ComponentStatus]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ComponentStatus](
	[ComponentStatusID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](7) NOT NULL,
 CONSTRAINT [PK_ComponentStatus] PRIMARY KEY CLUSTERED 
(
	[ComponentStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Online')
GO
INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Offline')
GO
INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Unknown')
GO

PRINT('ComponentStatus Table Added')

/****** Object:  Table [dbo].[Game]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Game](
	[GameID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED 
(
	[GameID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

PRINT('Game Table Added')

/****** Object:  Table [dbo].[Machine]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Machine](
	[MachineID] [int] IDENTITY(1,1) NOT NULL,
	[HostName] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Machine] PRIMARY KEY CLUSTERED 
(
	[MachineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

PRINT('Machine Table Added')

/****** Object:  Table [dbo].[ServerAlerts]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO

PRINT('ServerAlerts Table Added')

/****** Object:  Table [dbo].[ServerAlertStatus]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServerAlertStatus](
	[AlertStatusID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](13) NOT NULL,
 CONSTRAINT [PK_ServerAlertStatus] PRIMARY KEY CLUSTERED 
(
	[AlertStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Reported')
GO
INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Investigating')
GO
INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Resolved')
GO

PRINT('ServerAlertsStatus Table Added')

/****** Object:  Table [dbo].[ServerInformation]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO

ALTER TABLE [dbo].[ServerInformation] ADD  CONSTRAINT [DF_ServerInformation_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO

PRINT('ServerInformation Table Added')

/****** Object:  Table [dbo].[UserSettings]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO

PRINT('UserSettings Table Added')

ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_Component] FOREIGN KEY([ComponentID])
REFERENCES [dbo].[Component] ([ComponentID])
GO
ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_Component]
GO
ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_ComponentStatus] FOREIGN KEY([ComponentStatusID])
REFERENCES [dbo].[ComponentStatus] ([ComponentStatusID])
GO
ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_ComponentStatus]
GO
ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_ServerInformation] FOREIGN KEY([ServerInformationID])
REFERENCES [dbo].[ServerInformation] ([ServerInformationID])
GO
ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_ServerInformation]
GO
ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_Component] FOREIGN KEY([ComponentID])
REFERENCES [dbo].[Component] ([ComponentID])
GO
ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_Component]
GO
ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_ComponentStatus] FOREIGN KEY([ComponentStatusID])
REFERENCES [dbo].[ComponentStatus] ([ComponentStatusID])
GO
ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_ComponentStatus]
GO
ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_ServerAlertStatus] FOREIGN KEY([AlertStatusID])
REFERENCES [dbo].[ServerAlertStatus] ([AlertStatusID])
GO
ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_ServerAlertStatus]
GO
ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_ServerInformation] FOREIGN KEY([ServerInformationID])
REFERENCES [dbo].[ServerInformation] ([ServerInformationID])
GO
ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_ServerInformation]
GO
ALTER TABLE [dbo].[ServerAlerts]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlerts_UserSettings] FOREIGN KEY([UserSettingsID])
REFERENCES [dbo].[UserSettings] ([UserSettingsID])
GO
ALTER TABLE [dbo].[ServerAlerts] CHECK CONSTRAINT [FK_ServerAlerts_UserSettings]
GO
ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Game] FOREIGN KEY([GameID])
REFERENCES [dbo].[Game] ([GameID])
GO
ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Game]
GO
ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Machine] FOREIGN KEY([MachineID])
REFERENCES [dbo].[Machine] ([MachineID])
GO
ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Machine]
GO
ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_APIUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[APIUser] ([UserID])
GO
ALTER TABLE [dbo].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_APIUser]
GO
ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_Application] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[Application] ([ApplicationID])
GO
ALTER TABLE [dbo].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_Application]
GO

PRINT('Foreign Keys Added')

INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/usersettings')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverinformation')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverevent')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serveralert')
GO

PRINT('Added New Endpoints')

INSERT [dbo].[Scope] ([Value]) VALUES ('Server Status API')
GO

PRINT('Added New Scope')

INSERT INTO VersionHistory(ReleaseVersion, DateUpdated)
VALUES ('1.1.0', GETUTCDATE())

PRINT('Added VersionHistory Record')