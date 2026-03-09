USE [HunterIndustriesAPI]
GO

ALTER TABLE Component ALTER COLUMN [Name] [varchar](17) NOT NULL

PRINT('Changed Name Field to varchar(27) in Component Table')

UPDATE Component SET [Name] = 'Connection Status'
WHERE [Name] = 'Hamachi Status'

PRINT('Changed "Hamachi Status" to "Connection Status" in Component Table')

ALTER TABLE ServerInformation ALTER COLUMN [IPAddress] [varchar](50) NOT NULL

PRINT('Changed IPAddress Field to varchar(50) in ServerInformation Table')

ALTER TABLE Game ADD [Version] [varchar](20) NOT NULL

PRINT('Added Version Field to Game Table')

ALTER TABLE ServerInformation DROP COLUMN [GameVersion]

PRINT('Removed GameVersion Field from ServerInformation Table')

ALTER TABLE ServerInformation ADD [Port] [int] NOT NULL

PRINT('Added Port Field to ServerInformation Table')

CREATE TABLE [dbo].[Connection](
	[ConnectionID] [int] IDENTITY(1,1) NOT NULL,
	[IPAddress] [varchar](50) NOT NULL,
	[Port] [int] NOT NULL,
 CONSTRAINT [PK_Connection] PRIMARY KEY CLUSTERED 
(
	[ConnectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

PRINT('Connection Table Added')

ALTER TABLE ServerInformation DROP COLUMN [IPAddress]

PRINT('Removed IPAddress Field from ServerInformation Table')

ALTER TABLE ServerInformation DROP COLUMN [Port]

PRINT('Removed Port Field from ServerInformation Table')

ALTER TABLE ServerInformation ADD [ConnectionID] [int] NOT NULL

PRINT('Added ConnectionID Field to ServerInformation Table')

ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Connection] FOREIGN KEY([ConnectionID])
REFERENCES [dbo].[Connection] ([ConnectionID])
GO

ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Connection]
GO

PRINT('Foreign Keys Added')

CREATE TABLE [dbo].[Downtime](
	[DowntimeID] [int] IDENTITY(1,1) NOT NULL,
	[Time] [varchar](8) NOT NULL,
 CONSTRAINT [PK_Downtime] PRIMARY KEY CLUSTERED 
(
	[DowntimeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

PRINT('Downtime Table Added')

ALTER TABLE ServerInformation ADD [DowntimeID] [int] NULL

ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Downtime] FOREIGN KEY([DowntimeID])
REFERENCES [dbo].[Downtime] ([DowntimeID])
GO

ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Downtime]
GO

PRINT('Downtime Table Foreign Keys Added')

INSERT INTO VersionHistory(ReleaseVersion, DateUpdated)
VALUES ('1.2.0', GETUTCDATE())

PRINT('Added VersionHistory Record')