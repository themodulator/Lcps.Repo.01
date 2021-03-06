USE [Lcps.v2015]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ExternalData].[NWUsers]') AND type in (N'U'))
BEGIN
CREATE TABLE [ExternalData].[NWUsers](
	[NWUserKey] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_NWUsers_NWUserKey]  DEFAULT (newid()),
	[SocSecNbrFormatted] [nvarchar](50) NULL,
	[EntityID] [nvarchar](50) NULL,
	[LN] [nvarchar](40) NULL,
	[FN] [nvarchar](20) NULL,
	[MN] [nvarchar](50) NULL,
	[BirthDateStandard] [nvarchar](50) NULL,
	[Gender] [nvarchar](1) NULL,
	[SchPerDir] [nvarchar](50) NULL,
	[EmpType] [nvarchar](50) NULL,
	[JobTitle] [nvarchar](50) NULL,
	[UserNameNW] [nvarchar](50) NULL,
	[EmailLcps] [nvarchar](255) NULL,
	[Password] [nvarchar](50) NULL,
	[Year] [nvarchar](4) NULL,
	[UserStatus] [nvarchar](2) NULL,
 CONSTRAINT [PK_NWUsers] PRIMARY KEY CLUSTERED 
(
	[NWUserKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ExternalData].[Personnel]') AND type in (N'U'))
BEGIN
CREATE TABLE [ExternalData].[Personnel](
	[PersonnelKey] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Personnel_PersonnelKey]  DEFAULT (newid()),
	[SSN] [nvarchar](12) NULL,
	[Last Name] [nvarchar](30) NULL,
	[First Name] [nvarchar](20) NULL,
	[Middle Name] [nvarchar](50) NULL,
	[DOB] [datetime] NULL,
	[Sex] [nvarchar](1) NULL,
	[Location1] [nvarchar](20) NULL,
	[Position] [nvarchar](40) NULL,
	[Assignment] [nvarchar](60) NULL,
	[PartTime] [nvarchar](1) NULL,
	[Active] [bit] NOT NULL,
	[Employment Date] [datetime] NULL,
 CONSTRAINT [PK_Personnel] PRIMARY KEY CLUSTERED 
(
	[PersonnelKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
