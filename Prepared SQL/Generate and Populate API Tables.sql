USE [HunterIndustriesAPI]
GO

/* API Management */

/****** Object:  Table [dbo].[APIUser]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[APIUser](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](255) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_APIUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
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
	[ApplicationId] [int] IDENTITY(1,1) NOT NULL,
	[PhraseId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[AuditHistory]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditHistory](
	[AuditId] [int] IDENTITY(1,1) NOT NULL,
	[EndpointId] [int] NOT NULL,
	[MethodId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[IPAddress] [varchar](15) NOT NULL,
	[DateOccured] [datetime] NOT NULL,
	[Parameters] [varchar](max) NULL,
 CONSTRAINT [PK_AuditHistory] PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Authorisation]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Authorisation](
	[PhraseId] [int] IDENTITY(1,1) NOT NULL,
	[Phrase] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Authorisation] PRIMARY KEY CLUSTERED 
(
	[PhraseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Change]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Change](
	[ChangeId] [int] IDENTITY(1,1) NOT NULL,
	[EndpointId] [int] NOT NULL,
	[AuditId] [int] NOT NULL,
	[Field] [varchar](50) NOT NULL,
	[OldValue] [varchar](255) NOT NULL,
	[NewValue] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Change] PRIMARY KEY CLUSTERED 
(
	[ChangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Endpoint]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Endpoint](
	[EndpointId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Endpoint] PRIMARY KEY CLUSTERED 
(
	[EndpointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ErrorLog]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[ErrorId] [int] IDENTITY(1,1) NOT NULL,
	[DateOccured] [datetime] NOT NULL,
	[IPAddress] [varchar](15) NOT NULL,
	[Summary] [varchar](255) NOT NULL,
	[Message] [varchar](max) NOT NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[LoginAttempt]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoginAttempt](
	[AttemptId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[PhraseId] [int] NULL,
	[AuditId] [int] NOT NULL,
	[DateOccured] [datetime] NOT NULL,
	[IsSuccessful] [bit] NOT NULL,
 CONSTRAINT [PK_LoginAttempt] PRIMARY KEY CLUSTERED 
(
	[AttemptId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Method]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Method](
	[MethodId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](6) NOT NULL,
 CONSTRAINT [PK_Methods] PRIMARY KEY CLUSTERED 
(
	[MethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Scope]    Script Date: 10/05/2025 20:22:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Scope](
	[ScopeId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Scope] PRIMARY KEY CLUSTERED 
(
	[ScopeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[StatusCode]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusCode](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](100) NOT NULL,
 CONSTRAINT [PK_StatusCode] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[UserScope]    Script Date: 10/05/2025 20:22:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserScope](
	[UserScopeId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ScopeId] [int] NOT NULL,
 CONSTRAINT [PK_UserScope] PRIMARY KEY CLUSTERED 
(
	[UserScopeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[UserSetting]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSetting](
	[UserSettingId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_UserSetting] PRIMARY KEY CLUSTERED 
(
	[UserSettingId] ASC
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
	[AssistantId] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [int] NOT NULL,
	[DeletionStatusId] [int] NOT NULL,
	[VersionId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IDNumber] [varchar](10) NOT NULL,
 CONSTRAINT [PK_AssistantInformation] PRIMARY KEY CLUSTERED 
(
	[AssistantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Deletion]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Deletion](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](5) NOT NULL,
 CONSTRAINT [PK_Deletion] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Location]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Location](
	[LocationId] [int] IDENTITY(1,1) NOT NULL,
	[HostName] [varchar](100) NOT NULL,
	[IPAddress] [varchar](15) NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[User]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Version]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Version](
	[VersionId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](7) NULL,
 CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED 
(
	[VersionId] ASC
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
	[ComponentId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](17) NOT NULL,
 CONSTRAINT [PK_Component] PRIMARY KEY CLUSTERED 
(
	[ComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ComponentInformation]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ComponentInformation](
	[ComponentInformationId] [int] IDENTITY(1,1) NOT NULL,
	[ServerInformationId] [int] NOT NULL,
	[ComponentId] [int] NOT NULL,
	[ComponentStatusId] [int] NOT NULL,
	[DateOccured] [datetime] NOT NULL,
 CONSTRAINT [PK_ComponentInformation] PRIMARY KEY CLUSTERED 
(
	[ComponentInformationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ComponentStatus]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ComponentStatus](
	[ComponentStatusId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](7) NOT NULL,
 CONSTRAINT [PK_ComponentStatus] PRIMARY KEY CLUSTERED 
(
	[ComponentStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Connection]    Script Date: 14/08/2025 20:59:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Connection](
	[ConnectionId] [int] IDENTITY(1,1) NOT NULL,
	[IPAddress] [varchar](50) NOT NULL,
	[Port] [int] NOT NULL,
 CONSTRAINT [PK_Connection] PRIMARY KEY CLUSTERED 
(
	[ConnectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Downtime]    Script Date: 14/08/2025 20:59:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Downtime](
	[DowntimeId] [int] IDENTITY(1,1) NOT NULL,
	[Time] [varchar](8) NOT NULL,
 CONSTRAINT [PK_Downtime] PRIMARY KEY CLUSTERED 
(
	[DowntimeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Game]    Script Date: 14/08/2025 21:00:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Game](
	[GameId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Version] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Machine]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Machine](
	[MachineId] [int] IDENTITY(1,1) NOT NULL,
	[HostName] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Machine] PRIMARY KEY CLUSTERED 
(
	[MachineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ServerAlert]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServerAlert](
	[ServerAlertId] [int] IDENTITY(1,1) NOT NULL,
	[ServerInformationId] [int] NOT NULL,
	[UserSettingsId] [int] NOT NULL,
	[ComponentId] [int] NOT NULL,
	[ComponentStatusId] [int] NOT NULL,
	[AlertStatusId] [int] NOT NULL,
	[DateOccured] [datetime] NOT NULL,
 CONSTRAINT [PK_ServerAlert] PRIMARY KEY CLUSTERED 
(
	[ServerAlertId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ServerAlertStatus]    Script Date: 17/05/2025 12:29:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServerAlertStatus](
	[AlertStatusId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](13) NOT NULL,
 CONSTRAINT [PK_ServerAlertStatus] PRIMARY KEY CLUSTERED 
(
	[AlertStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ServerInformation]    Script Date: 14/08/2025 21:00:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServerInformation](
	[ServerInformationId] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[ConnectionId] [int] NOT NULL,
	[DowntimeId] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ServerInformation] PRIMARY KEY CLUSTERED 
(
	[ServerInformationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServerInformation] ADD  CONSTRAINT [DF_ServerInformation_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO

/* Constraints */

ALTER TABLE [dbo].[Application]  WITH CHECK ADD  CONSTRAINT [FK_Applications_Authorisation] FOREIGN KEY([PhraseId])
REFERENCES [dbo].[Authorisation] ([PhraseId])
GO
ALTER TABLE [dbo].[Application] CHECK CONSTRAINT [FK_Applications_Authorisation]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_Deletion] FOREIGN KEY([DeletionStatusId])
REFERENCES [dbo].[Deletion] ([StatusId])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_Deletion]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Location] ([LocationId])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_Location]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_User]
GO
ALTER TABLE [dbo].[AssistantInformation]  WITH CHECK ADD  CONSTRAINT [FK_AssistantInformation_Version] FOREIGN KEY([VersionId])
REFERENCES [dbo].[Version] ([VersionId])
GO
ALTER TABLE [dbo].[AssistantInformation] CHECK CONSTRAINT [FK_AssistantInformation_Version]
GO
ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_Endpoint] FOREIGN KEY([EndpointId])
REFERENCES [dbo].[Endpoint] ([EndpointId])
GO
ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_Endpoint]
GO
ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_Methods] FOREIGN KEY([MethodId])
REFERENCES [dbo].[Method] ([MethodId])
GO
ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_Methods]
GO
ALTER TABLE [dbo].[AuditHistory]  WITH CHECK ADD  CONSTRAINT [FK_AuditHistory_StatusCode] FOREIGN KEY([StatusId])
REFERENCES [dbo].[StatusCode] ([StatusId])
GO
ALTER TABLE [dbo].[AuditHistory] CHECK CONSTRAINT [FK_AuditHistory_StatusCode]
GO
ALTER TABLE [dbo].[Change]  WITH CHECK ADD  CONSTRAINT [FK_Change_Audit] FOREIGN KEY([AuditId])
REFERENCES [dbo].[AuditHistory] ([AuditId])
GO
ALTER TABLE [dbo].[Change] CHECK CONSTRAINT [FK_Change_Audit]
GO
ALTER TABLE [dbo].[Change]  WITH CHECK ADD  CONSTRAINT [FK_Change_Endpoint] FOREIGN KEY([EndpointId])
REFERENCES [dbo].[Endpoint] ([EndpointId])
GO
ALTER TABLE [dbo].[Change] CHECK CONSTRAINT [FK_Change_Endpoint]
GO
ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_Component] FOREIGN KEY([ComponentId])
REFERENCES [dbo].[Component] ([ComponentId])
GO
ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_Component]
GO
ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_ComponentStatus] FOREIGN KEY([ComponentStatusId])
REFERENCES [dbo].[ComponentStatus] ([ComponentStatusId])
GO
ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_ComponentStatus]
GO
ALTER TABLE [dbo].[ComponentInformation]  WITH CHECK ADD  CONSTRAINT [FK_ComponentInformation_ServerInformation] FOREIGN KEY([ServerInformationId])
REFERENCES [dbo].[ServerInformation] ([ServerInformationId])
GO
ALTER TABLE [dbo].[ComponentInformation] CHECK CONSTRAINT [FK_ComponentInformation_ServerInformation]
GO
ALTER TABLE [dbo].[LoginAttempt]  WITH CHECK ADD  CONSTRAINT [FK_LoginAttempt_AuditHistory] FOREIGN KEY([AuditId])
REFERENCES [dbo].[AuditHistory] ([AuditId])
GO
ALTER TABLE [dbo].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_AuditHistory]
GO
ALTER TABLE [dbo].[LoginAttempt]  WITH CHECK ADD  CONSTRAINT [FK_LoginAttempt_Authorisation] FOREIGN KEY([PhraseId])
REFERENCES [dbo].[Authorisation] ([PhraseId])
GO
ALTER TABLE [dbo].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_Authorisation]
GO
ALTER TABLE [dbo].[LoginAttempt]  WITH CHECK ADD  CONSTRAINT [FK_LoginAttempt_APIUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[APIUser] ([UserId])
GO
ALTER TABLE [dbo].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_APIUser]
GO
ALTER TABLE [dbo].[ServerAlert]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlert_Component] FOREIGN KEY([ComponentId])
REFERENCES [dbo].[Component] ([ComponentId])
GO
ALTER TABLE [dbo].[ServerAlert] CHECK CONSTRAINT [FK_ServerAlert_Component]
GO
ALTER TABLE [dbo].[ServerAlert]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlert_ComponentStatus] FOREIGN KEY([ComponentStatusId])
REFERENCES [dbo].[ComponentStatus] ([ComponentStatusId])
GO
ALTER TABLE [dbo].[ServerAlert] CHECK CONSTRAINT [FK_ServerAlert_ComponentStatus]
GO
ALTER TABLE [dbo].[ServerAlert]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlert_ServerAlertStatus] FOREIGN KEY([AlertStatusId])
REFERENCES [dbo].[ServerAlertStatus] ([AlertStatusId])
GO
ALTER TABLE [dbo].[ServerAlert] CHECK CONSTRAINT [FK_ServerAlert_ServerAlertStatus]
GO
ALTER TABLE [dbo].[ServerAlert]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlert_ServerInformation] FOREIGN KEY([ServerInformationId])
REFERENCES [dbo].[ServerInformation] ([ServerInformationId])
GO
ALTER TABLE [dbo].[ServerAlert] CHECK CONSTRAINT [FK_ServerAlert_ServerInformation]
GO
ALTER TABLE [dbo].[ServerAlert]  WITH CHECK ADD  CONSTRAINT [FK_ServerAlert_UserSetting] FOREIGN KEY([UserSettingsId])
REFERENCES [dbo].[UserSetting] ([UserSettingsId])
GO
ALTER TABLE [dbo].[ServerAlert] CHECK CONSTRAINT [FK_ServerAlert_UserSetting]
GO
ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Connection] FOREIGN KEY([ConnectionId])
REFERENCES [dbo].[Connection] ([ConnectionId])
GO
ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Connection]
GO
ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Downtime] FOREIGN KEY([DowntimeId])
REFERENCES [dbo].[Downtime] ([DowntimeId])
GO
ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Downtime]
GO
ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Game] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([GameId])
GO
ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Game]
GO
ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Machine] FOREIGN KEY([MachineId])
REFERENCES [dbo].[Machine] ([MachineId])
GO
ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Machine]
GO
ALTER TABLE [dbo].[UserScope]  WITH CHECK ADD  CONSTRAINT [FK_UserScope_Scope] FOREIGN KEY([ScopeId])
REFERENCES [dbo].[Scope] ([ScopeId])
GO
ALTER TABLE [dbo].[UserScope] CHECK CONSTRAINT [FK_UserScope_Scope]
GO
ALTER TABLE [dbo].[UserScope]  WITH CHECK ADD  CONSTRAINT [FK_UserScope_APIUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[APIUser] ([UserId])
GO
ALTER TABLE [dbo].[UserScope] CHECK CONSTRAINT [FK_UserScope_APIUser]
GO
ALTER TABLE [dbo].[UserSetting]  WITH CHECK ADD  CONSTRAINT [FK_UserSetting_APIUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[APIUser] ([UserId])
GO
ALTER TABLE [dbo].[UserSetting] CHECK CONSTRAINT [FK_UserSetting_APIUser]
GO
ALTER TABLE [dbo].[UserSetting]  WITH CHECK ADD  CONSTRAINT [FK_UserSetting_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO
ALTER TABLE [dbo].[UserSetting] CHECK CONSTRAINT [FK_UserSetting_Application]
GO

/* Inserts */

INSERT [dbo].[Component] ([Name]) VALUES ('PC Status')
GO
INSERT [dbo].[Component] ([Name]) VALUES ('Server Status')
GO
INSERT [dbo].[Component] ([Name]) VALUES ('Connection Status')
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
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/UserSetting')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverinformation')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serverevent')
GO
INSERT [dbo].[Endpoint] ([Value]) VALUES ('https://hunter-industries.co.uk/api/serverstatus/serveralert')
GO
INSERT [dbo].[Game] ([Name], [Version]) VALUES ('Minecraft', '1.7.10')
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
