USE [HunterIndustriesAPI]
GO

/* API Management */

/****** Object:  Table [dbo].[APIUser]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[APIUser](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](255) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_APIUser] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[APIUser] ADD DEFAULT ((0)) FOR [IsDeleted]
GO

/****** Object:  Table [dbo].[Application]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Application](
	[ApplicationID] [int] IDENTITY(1,1) NOT NULL,
	[PhraseID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[ApplicationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[AuditHistory]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditHistory](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[EndpointID] [int] NOT NULL,
	[MethodID] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[IPAddress] [varchar](15) NOT NULL,
	[DateOccured] [datetime] NOT NULL,
	[Parameters] [varchar](max) NULL,
 CONSTRAINT [PK_AuditHistory] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Authorisation]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Authorisation](
	[PhraseID] [int] IDENTITY(1,1) NOT NULL,
	[Phrase] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Authorisation] PRIMARY KEY CLUSTERED 
(
	[PhraseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Change]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Change](
	[ChangeID] [int] IDENTITY(1,1) NOT NULL,
	[EndpointID] [int] NOT NULL,
	[AuditID] [int] NOT NULL,
	[Field] [varchar](50) NOT NULL,
	[OldValue] [varchar](255) NOT NULL,
	[NewValue] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Change] PRIMARY KEY CLUSTERED 
(
	[ChangeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Endpoint]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Endpoint](
	[EndpointID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Endpoint] PRIMARY KEY CLUSTERED 
(
	[EndpointID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ErrorLog]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[ErrorID] [int] IDENTITY(1,1) NOT NULL,
	[DateOccured] [datetime] NOT NULL,
	[IPAddress] [varchar](15) NOT NULL,
	[Summary] [varchar](255) NOT NULL,
	[Message] [varchar](max) NOT NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[ErrorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[LoginAttempt]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoginAttempt](
	[AttemptID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[PhraseID] [int] NULL,
	[AuditID] [int] NOT NULL,
	[DateOccured] [datetime] NOT NULL,
	[IsSuccessful] [bit] NOT NULL,
 CONSTRAINT [PK_LoginAttempt] PRIMARY KEY CLUSTERED 
(
	[AttemptID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Method]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Method](
	[MethodID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](6) NOT NULL,
 CONSTRAINT [PK_Methods] PRIMARY KEY CLUSTERED 
(
	[MethodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
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

/****** Object:  Table [dbo].[StatusCode]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusCode](
	[StatusID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](100) NOT NULL,
 CONSTRAINT [PK_StatusCode] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

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

/* Assistant API */

/****** Object:  Table [dbo].[AssistantInformation]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssistantInformation](
	[AssistantID] [int] IDENTITY(1,1) NOT NULL,
	[LocationID] [int] NOT NULL,
	[DeletionStatusID] [int] NOT NULL,
	[VersionID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IDNumber] [varchar](10) NOT NULL,
 CONSTRAINT [PK_AssistantInformation] PRIMARY KEY CLUSTERED 
(
	[AssistantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Deletion]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Deletion](
	[StatusID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](5) NOT NULL,
 CONSTRAINT [PK_Deletion] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Location]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Location](
	[LocationID] [int] IDENTITY(1,1) NOT NULL,
	[HostName] [varchar](100) NOT NULL,
	[IPAddress] [varchar](15) NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[LocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[User]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Version]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Version](
	[VersionID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](7) NULL,
 CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED 
(
	[VersionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/* Server Status API */

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

/* Constraints */

ALTER TABLE [dbo].[Application]  WITH CHECK ADD  CONSTRAINT [FK_Applications_Authorisation] FOREIGN KEY([PhraseID])
REFERENCES [dbo].[Authorisation] ([PhraseID])
GO
ALTER TABLE [dbo].[Application] CHECK CONSTRAINT [FK_Applications_Authorisation]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_Deletion] FOREIGN KEY([DeletionStatusID])
REFERENCES [dbo].[Deletion] ([StatusID])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_Deletion]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_Location] FOREIGN KEY([LocationID])
REFERENCES [dbo].[Location] ([LocationID])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_Location]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_User]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_Version] FOREIGN KEY([VersionID])
REFERENCES [dbo].[Version] ([VersionID])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_Version]
GO
ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_Endpoint] FOREIGN KEY([EndpointID])
REFERENCES [dbo].[Endpoint] ([EndpointID])
GO
ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_Endpoint]
GO
ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_Methods] FOREIGN KEY([MethodID])
REFERENCES [dbo].[Method] ([MethodID])
GO
ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_Methods]
GO
ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_StatusCode] FOREIGN KEY([StatusID])
REFERENCES [dbo].[StatusCode] ([StatusID])
GO
ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_StatusCode]
GO
ALTER TABLE [dbo].[Change]  WITH CHECK ADD  CONSTRAINT [FK_Change_Audit] FOREIGN KEY([AuditID])
REFERENCES [dbo].[AuditHistory] ([AuditID])
GO
ALTER TABLE [dbo].[Change] CHECK CONSTRAINT [FK_Change_Audit]
GO
ALTER TABLE [dbo].[Change]  WITH CHECK ADD  CONSTRAINT [FK_Change_Endpoint] FOREIGN KEY([EndpointID])
REFERENCES [dbo].[Endpoint] ([EndpointID])
GO
ALTER TABLE [dbo].[Change] CHECK CONSTRAINT [FK_Change_Endpoint]
GO
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
ALTER TABLE [dbo].[LoginAttempt]  WITH CHECK ADD  CONSTRAINT [FK_LoginAttempt_AuditHistory] FOREIGN KEY([AuditID])
REFERENCES [dbo].[AuditHistory] ([AuditID])
GO
ALTER TABLE [dbo].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_AuditHistory]
GO
ALTER TABLE [dbo].[LoginAttempt]  WITH CHECK ADD  CONSTRAINT [FK_LoginAttempt_Authorisation] FOREIGN KEY([PhraseID])
REFERENCES [dbo].[Authorisation] ([PhraseID])
GO
ALTER TABLE [dbo].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_Authorisation]
GO
ALTER TABLE [dbo].[LoginAttempt]  WITH CHECK ADD  CONSTRAINT [FK_LoginAttempt_APIUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[APIUser] ([UserID])
GO
ALTER TABLE [dbo].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_APIUser]
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

/* Inserts */

INSERT [dbo].[Component] ([Name]) VALUES ('PC Status')
GO
INSERT [dbo].[Component] ([Name]) VALUES ('Hamachi Status')
GO
INSERT [dbo].[Component] ([Name]) VALUES ('Server Status')
GO
INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Online')
GO
INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Offline')
GO
INSERT [dbo].[ComponentStatus] ([Value]) VALUES ('Unknown')
GO
INSERT [dbo].[Deletion] ([Value]) VALUES ('True')
GO
INSERT [dbo].[Deletion] ([Value]) VALUES ('False')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/auth/token')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/audithistory')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/assistant/config')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/assistant/version')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/assistant/deletion')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/assistant/location')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/user')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/usersettings')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverinformation')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverevent')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serveralert')
GO
INSERT [dbo].[Game] ([Name]) VALUES ('Minecraft')
GO
INSERT [dbo].[Machine] ([HostName]) VALUES ('Hunter-NAS')
GO
INSERT [dbo].[Method] ([Value]) VALUES ('GET')
GO
INSERT [dbo].[Method] ([Value]) VALUES ('POST')
GO
INSERT [dbo].[Method] ([Value]) VALUES ('PATCH')
GO
INSERT [dbo].[Method] ([Value]) VALUES ('DELETE')
GO
INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Reported')
GO
INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Investigating')
GO
INSERT [dbo].[ServerAlertStatus] ([Value]) VALUES ('Resolved')
GO
INSERT [dbo].[StatusCode] ([Value]) VALUES ('200 OK')
GO
INSERT [dbo].[StatusCode] ([Value]) VALUES ('201 Created')
GO
INSERT [dbo].[StatusCode] ([Value]) VALUES ('400 Bad Request')
GO
INSERT [dbo].[StatusCode] ([Value]) VALUES ('401 Unauthorized')
GO
INSERT [dbo].[StatusCode] ([Value]) VALUES ('403 Forbidden')
GO
INSERT [dbo].[StatusCode] ([Value]) VALUES ('404 Not Found')
GO
INSERT [dbo].[StatusCode] ([Value]) VALUES ('500 Internal Server Error')
GO
INSERT [dbo].[Version] ([Value]) VALUES ('0.0.0')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('User')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('Assistant API')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('Book Reader API')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('Control Panel API')
GO
INSERT [dbo].[Scope] ([Value]) VALUES ('Server Status API')
GO
