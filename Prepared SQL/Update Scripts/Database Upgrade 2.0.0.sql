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

INSERT INTO VersionHistory(ReleaseVersion, DateUpdated)
VALUES ('2.0.0', GETUTCDATE())

PRINT('Added VersionHistory Record')