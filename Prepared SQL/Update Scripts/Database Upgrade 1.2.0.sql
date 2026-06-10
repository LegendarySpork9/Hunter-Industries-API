USE [HunterIndustriesAPI]
GO

ALTER TABLE Component ALTER COLUMN [Name] [varchar](17) NOT NULL

PRINT('Changed Name Field to varchar(17) in Component Table')
GO

IF EXISTS (SELECT * FROM [dbo].[Component] WHERE [Name] = 'Hamachi Status')
BEGIN
	UPDATE Component SET [Name] = 'Connection Status'
	WHERE [Name] = 'Hamachi Status'

	PRINT('Changed "Hamachi Status" to "Connection Status" in Component Table')
END
ELSE
	PRINT('"Connection Status" Already Set in Component Table')
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'IPAddress')
BEGIN
	ALTER TABLE ServerInformation ALTER COLUMN [IPAddress] [varchar](50) NOT NULL

	PRINT('Changed IPAddress Field to varchar(50) in ServerInformation Table')
END
ELSE
	PRINT('IPAddress Field Does Not Exist on ServerInformation Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Game') AND name = 'Version')
BEGIN
	ALTER TABLE Game ADD [Version] [varchar](20) NOT NULL

	PRINT('Added Version Field to Game Table')
END
ELSE
	PRINT('Version Field Already Exists on Game Table')
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'GameVersion')
BEGIN
	ALTER TABLE ServerInformation DROP COLUMN [GameVersion]

	PRINT('Removed GameVersion Field from ServerInformation Table')
END
ELSE
	PRINT('GameVersion Field Already Removed from ServerInformation Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'Port')
	AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'ConnectionID')
BEGIN
	ALTER TABLE ServerInformation ADD [Port] [int] NOT NULL

	PRINT('Added Port Field to ServerInformation Table')
END
ELSE
	PRINT('Port Field Already Handled on ServerInformation Table')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Connection' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Connection](
		[ConnectionID] [int] IDENTITY(1,1) NOT NULL,
		[IPAddress] [varchar](50) NOT NULL,
		[Port] [int] NOT NULL,
	 CONSTRAINT [PK_Connection] PRIMARY KEY CLUSTERED
	(
		[ConnectionID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Connection Table Added')
END
ELSE
	PRINT('Connection Table Already Exists')
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'IPAddress')
BEGIN
	ALTER TABLE ServerInformation DROP COLUMN [IPAddress]

	PRINT('Removed IPAddress Field from ServerInformation Table')
END
ELSE
	PRINT('IPAddress Field Already Removed from ServerInformation Table')
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'Port')
BEGIN
	ALTER TABLE ServerInformation DROP COLUMN [Port]

	PRINT('Removed Port Field from ServerInformation Table')
END
ELSE
	PRINT('Port Field Already Removed from ServerInformation Table')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'ConnectionID')
BEGIN
	ALTER TABLE ServerInformation ADD [ConnectionID] [int] NOT NULL

	PRINT('Added ConnectionID Field to ServerInformation Table')
END
ELSE
	PRINT('ConnectionID Field Already Exists on ServerInformation Table')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerInformation_Connection')
BEGIN
	ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Connection] FOREIGN KEY([ConnectionID])
	REFERENCES [dbo].[Connection] ([ConnectionID])

	ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Connection]

	PRINT('Foreign Keys Added')
END
ELSE
	PRINT('FK_ServerInformation_Connection Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Downtime' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [dbo].[Downtime](
		[DowntimeID] [int] IDENTITY(1,1) NOT NULL,
		[Time] [varchar](8) NOT NULL,
	 CONSTRAINT [PK_Downtime] PRIMARY KEY CLUSTERED
	(
		[DowntimeID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT('Downtime Table Added')
END
ELSE
	PRINT('Downtime Table Already Exists')
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServerInformation') AND name = 'DowntimeID')
BEGIN
	ALTER TABLE ServerInformation ADD [DowntimeID] [int] NULL

	PRINT('Added DowntimeID Field to ServerInformation Table')
END
ELSE
	PRINT('DowntimeID Field Already Exists on ServerInformation Table')
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ServerInformation_Downtime')
BEGIN
	ALTER TABLE [dbo].[ServerInformation]  WITH CHECK ADD  CONSTRAINT [FK_ServerInformation_Downtime] FOREIGN KEY([DowntimeID])
	REFERENCES [dbo].[Downtime] ([DowntimeID])

	ALTER TABLE [dbo].[ServerInformation] CHECK CONSTRAINT [FK_ServerInformation_Downtime]

	PRINT('Downtime Table Foreign Keys Added')
END
ELSE
	PRINT('FK_ServerInformation_Downtime Already Exists')
GO
