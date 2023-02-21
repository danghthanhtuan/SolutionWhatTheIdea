USE [ProductIntroduce]
GO

/****** Object:  Table [dbo].[Partner]    Script Date: 2/21/2023 9:13:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Partner](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[UrlLogo] [varchar](500) NULL,
	[Code] [varchar](50) NULL,
	[Description] [nvarchar](550) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUser] [varchar](50) NULL,
	[CreatedUser] [varchar](50) NULL,
	[Status] [int] NULL
) ON [PRIMARY]
GO


