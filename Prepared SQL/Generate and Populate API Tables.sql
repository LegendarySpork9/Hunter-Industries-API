USE [HunterIndustriesAPI]
GO
/****** Object:  Table [dbo].[APIUser]    Script Date: 18/12/2024 21:19:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[APIUser](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](255) NOT NULL,
	[Password] [varchar](255) NOT NULL,
 CONSTRAINT [PK_APIUser] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
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
	[Value] [varchar](5) NOT NULL,
 CONSTRAINT [PK_Methods] PRIMARY KEY CLUSTERED 
(
	[MethodID] ASC
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
ALTER TABLE [dbo].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_User]
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
INSERT [dbo].[Method] ([Value]) VALUES ('GET')
GO
INSERT [dbo].[Method] ([Value]) VALUES ('POST')
GO
INSERT [dbo].[Method] ([Value]) VALUES ('PATCH')
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
