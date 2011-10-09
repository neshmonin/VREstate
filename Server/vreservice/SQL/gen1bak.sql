/****** Object:  Table [dbo].[ContactInfo]    Script Date: 10/24/2010 20:38:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContactInfo](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[Emails] [nvarchar](max) NOT NULL,
	[PhoneNumbers] [nvarchar](max) NOT NULL,
	[PersonalTitle] [tinyint] NULL,
	[FirstName] [nvarchar](128) NULL,
	[LastName] [nvarchar](128) NULL,
	[MiddleName] [nvarchar](128) NULL,
	[AddressLine1] [nvarchar](128) NULL,
	[AddressLine2] [nvarchar](128) NULL,
	[City] [nvarchar](64) NULL,
	[StateProvince] [nvarchar](8) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[Country] [nvarchar](128) NULL,
 CONSTRAINT [PK_ContactInfo] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EstateDevelopers]    Script Date: 10/24/2010 20:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EstateDevelopers](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[VREConfiguration] [tinyint] NOT NULL,
 CONSTRAINT [PK__aVrEstateDevelop__1DE57479] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OptionTypes]    Script Date: 10/24/2010 20:38:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OptionTypes](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[Desription] [nvarchar](256) NULL,
 CONSTRAINT [PK_Materials] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbSettings]    Script Date: 10/24/2010 20:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbSettings](
	[Id] [int] NOT NULL,
	[BitValue] [bit] NULL,
	[IntValue] [bigint] NULL,
	[FloatValue] [float] NULL,
	[TimeValue] [datetime] NULL,
	[TextValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_DatabaseSettings_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10/24/2010 20:38:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UserRole] [tinyint] NOT NULL,
	[EstateDeveloperID] [int] NULL,
	[ContactInfoID] [int] NOT NULL,
 CONSTRAINT [PK__Users__014935CB] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Buildings]    Script Date: 10/24/2010 20:38:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Buildings](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[AddressLine1] [nvarchar](128) NULL,
	[AddressLine2] [nvarchar](128) NULL,
	[City] [nvarchar](64) NULL,
	[StateProvince] [nvarchar](8) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[Country] [nvarchar](128) NULL,
	[OpeningDate] [datetime] NULL,
	[Status] [tinyint] NOT NULL,
	[Model] [nvarchar](max) NULL,
	[EstateDeveloperID] [int] NOT NULL,
 CONSTRAINT [PK_Buildings] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Options]    Script Date: 10/24/2010 20:38:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Options](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CutOffDay] [datetime] NULL,
	[ProviderID] [int] NOT NULL,
	[BuildingID] [int] NOT NULL,
	[TypeID] [int] NOT NULL,
 CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suites]    Script Date: 10/24/2010 20:38:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suites](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[BuildingID] [int] NOT NULL,
	[PhysicalLevelNumber] [int] NOT NULL,
	[FloorNumber] [int] NOT NULL,
	[SuiteNumber] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[Model] [nvarchar](max) NULL,
 CONSTRAINT [PK_Suites] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Credentials]    Script Date: 10/24/2010 20:38:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Credentials](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[Login] [nvarchar](64) NOT NULL,
	[UserId] [int] NOT NULL,
	[Password] [varbinary](64) NOT NULL,
	[Salt] [varbinary](64) NOT NULL,
	[HashType] [varchar](32) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
 CONSTRAINT [PK_Credentials_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [CK_Credentials] UNIQUE NONCLUSTERED 
(
	[Type] ASC,
	[Login] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 10/24/2010 20:38:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[SuiteID] [int] NOT NULL,
	[ByuerID] [int] NOT NULL,
	[CurrentBalance] [float] NOT NULL,
	[CurrencyCents] [bigint] NOT NULL,
 CONSTRAINT [PK__aAccounts__0EA330E9] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VRTours]    Script Date: 10/24/2010 20:38:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VRTours](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[StartedOn] [datetime] NOT NULL,
	[SuiteID] [int] NOT NULL,
	[BuyerID] [int] NOT NULL,
	[SalespersonID] [int] NOT NULL,
	[TourRecording] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_VRTours] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 10/24/2010 20:38:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[AccointID] [int] NOT NULL,
	[Amount] [float] NOT NULL,
	[OptionPaidID] [int] NULL,
	[DealDateTime] [datetime] NOT NULL,
	[PaidDateTime] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK__aTransactions__1BFD2C07] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OptionTransactionMM]    Script Date: 10/24/2010 20:38:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OptionTransactionMM](
	[OptionID] [int] NOT NULL,
	[TransactionID] [int] NOT NULL,
 CONSTRAINT [PK_OptionTransactionMM] PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC,
	[TransactionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prices]    Script Date: 10/24/2010 20:38:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prices](
	[AutoID] [int] IDENTITY(1,1) NOT NULL,
	[OriginalCurrencyCents] [bigint] NOT NULL,
	[PricePerUnitForSubcontractor] [float] NOT NULL,
	[PricePerUnitForBuyer] [float] NOT NULL,
	[UnitName] [nvarchar](max) NULL,
	[NumberOfUnits] [float] NOT NULL,
	[StartingDate] [datetime] NOT NULL,
	[OptionID] [int] NOT NULL,
 CONSTRAINT [PK__aPrices__1273C1CD] PRIMARY KEY CLUSTERED 
(
	[AutoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OptionSuiteMM]    Script Date: 10/24/2010 20:38:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OptionSuiteMM](
	[OptionID] [int] NOT NULL,
	[SuiteID] [int] NOT NULL,
 CONSTRAINT [PK_OptionSuiteMM] PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC,
	[SuiteID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_Accounts_Suites]    Script Date: 10/24/2010 20:38:14 ******/
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_Suites] FOREIGN KEY([SuiteID])
REFERENCES [dbo].[Suites] ([AutoID])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Suites]
GO
/****** Object:  ForeignKey [FK_Accounts_Users]    Script Date: 10/24/2010 20:38:14 ******/
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_Users] FOREIGN KEY([ByuerID])
REFERENCES [dbo].[Users] ([AutoID])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Users]
GO
/****** Object:  ForeignKey [FK_Buildings_EstateDevelopers]    Script Date: 10/24/2010 20:38:18 ******/
ALTER TABLE [dbo].[Buildings]  WITH CHECK ADD  CONSTRAINT [FK_Buildings_EstateDevelopers] FOREIGN KEY([EstateDeveloperID])
REFERENCES [dbo].[EstateDevelopers] ([AutoID])
GO
ALTER TABLE [dbo].[Buildings] CHECK CONSTRAINT [FK_Buildings_EstateDevelopers]
GO
/****** Object:  ForeignKey [FK_Credentials_Users]    Script Date: 10/24/2010 20:38:24 ******/
ALTER TABLE [dbo].[Credentials]  WITH CHECK ADD  CONSTRAINT [FK_Credentials_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([AutoID])
GO
ALTER TABLE [dbo].[Credentials] CHECK CONSTRAINT [FK_Credentials_Users]
GO
/****** Object:  ForeignKey [FK_Options_Buildings]    Script Date: 10/24/2010 20:38:30 ******/
ALTER TABLE [dbo].[Options]  WITH CHECK ADD  CONSTRAINT [FK_Options_Buildings] FOREIGN KEY([BuildingID])
REFERENCES [dbo].[Buildings] ([AutoID])
GO
ALTER TABLE [dbo].[Options] CHECK CONSTRAINT [FK_Options_Buildings]
GO
/****** Object:  ForeignKey [FK_Options_Types]    Script Date: 10/24/2010 20:38:30 ******/
ALTER TABLE [dbo].[Options]  WITH CHECK ADD  CONSTRAINT [FK_Options_Types] FOREIGN KEY([TypeID])
REFERENCES [dbo].[OptionTypes] ([AutoID])
GO
ALTER TABLE [dbo].[Options] CHECK CONSTRAINT [FK_Options_Types]
GO
/****** Object:  ForeignKey [FK_Options_Users]    Script Date: 10/24/2010 20:38:30 ******/
ALTER TABLE [dbo].[Options]  WITH CHECK ADD  CONSTRAINT [FK_Options_Users] FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Users] ([AutoID])
GO
ALTER TABLE [dbo].[Options] CHECK CONSTRAINT [FK_Options_Users]
GO
/****** Object:  ForeignKey [FK_OptionSuiteMM_Options]    Script Date: 10/24/2010 20:38:31 ******/
ALTER TABLE [dbo].[OptionSuiteMM]  WITH CHECK ADD  CONSTRAINT [FK_OptionSuiteMM_Options] FOREIGN KEY([OptionID])
REFERENCES [dbo].[Options] ([AutoID])
GO
ALTER TABLE [dbo].[OptionSuiteMM] CHECK CONSTRAINT [FK_OptionSuiteMM_Options]
GO
/****** Object:  ForeignKey [FK_OptionSuiteMM_Suites]    Script Date: 10/24/2010 20:38:31 ******/
ALTER TABLE [dbo].[OptionSuiteMM]  WITH CHECK ADD  CONSTRAINT [FK_OptionSuiteMM_Suites] FOREIGN KEY([SuiteID])
REFERENCES [dbo].[Suites] ([AutoID])
GO
ALTER TABLE [dbo].[OptionSuiteMM] CHECK CONSTRAINT [FK_OptionSuiteMM_Suites]
GO
/****** Object:  ForeignKey [FK_OptionTransactionMM_Options]    Script Date: 10/24/2010 20:38:32 ******/
ALTER TABLE [dbo].[OptionTransactionMM]  WITH CHECK ADD  CONSTRAINT [FK_OptionTransactionMM_Options] FOREIGN KEY([OptionID])
REFERENCES [dbo].[Options] ([AutoID])
GO
ALTER TABLE [dbo].[OptionTransactionMM] CHECK CONSTRAINT [FK_OptionTransactionMM_Options]
GO
/****** Object:  ForeignKey [FK_OptionTransactionMM_Transactions]    Script Date: 10/24/2010 20:38:32 ******/
ALTER TABLE [dbo].[OptionTransactionMM]  WITH CHECK ADD  CONSTRAINT [FK_OptionTransactionMM_Transactions] FOREIGN KEY([TransactionID])
REFERENCES [dbo].[Transactions] ([AutoID])
GO
ALTER TABLE [dbo].[OptionTransactionMM] CHECK CONSTRAINT [FK_OptionTransactionMM_Transactions]
GO
/****** Object:  ForeignKey [FK_Prices_Options]    Script Date: 10/24/2010 20:38:35 ******/
ALTER TABLE [dbo].[Prices]  WITH CHECK ADD  CONSTRAINT [FK_Prices_Options] FOREIGN KEY([OptionID])
REFERENCES [dbo].[Options] ([AutoID])
GO
ALTER TABLE [dbo].[Prices] CHECK CONSTRAINT [FK_Prices_Options]
GO
/****** Object:  ForeignKey [FK_Suites_Buildings]    Script Date: 10/24/2010 20:38:38 ******/
ALTER TABLE [dbo].[Suites]  WITH CHECK ADD  CONSTRAINT [FK_Suites_Buildings] FOREIGN KEY([BuildingID])
REFERENCES [dbo].[Buildings] ([AutoID])
GO
ALTER TABLE [dbo].[Suites] CHECK CONSTRAINT [FK_Suites_Buildings]
GO
/****** Object:  ForeignKey [FK_Transactions_Accounts]    Script Date: 10/24/2010 20:38:40 ******/
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Accounts] FOREIGN KEY([AccointID])
REFERENCES [dbo].[Accounts] ([AutoID])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_Accounts]
GO
/****** Object:  ForeignKey [FK_Users_ContactInfo]    Script Date: 10/24/2010 20:38:42 ******/
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_ContactInfo] FOREIGN KEY([ContactInfoID])
REFERENCES [dbo].[ContactInfo] ([AutoID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_ContactInfo]
GO
/****** Object:  ForeignKey [FK_Users_EstateDevelopers]    Script Date: 10/24/2010 20:38:43 ******/
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_EstateDevelopers] FOREIGN KEY([EstateDeveloperID])
REFERENCES [dbo].[EstateDevelopers] ([AutoID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_EstateDevelopers]
GO
/****** Object:  ForeignKey [FK_VRTours_Suites]    Script Date: 10/24/2010 20:38:44 ******/
ALTER TABLE [dbo].[VRTours]  WITH CHECK ADD  CONSTRAINT [FK_VRTours_Suites] FOREIGN KEY([SuiteID])
REFERENCES [dbo].[Suites] ([AutoID])
GO
ALTER TABLE [dbo].[VRTours] CHECK CONSTRAINT [FK_VRTours_Suites]
GO
/****** Object:  ForeignKey [FK_VRTours_Users]    Script Date: 10/24/2010 20:38:44 ******/
ALTER TABLE [dbo].[VRTours]  WITH CHECK ADD  CONSTRAINT [FK_VRTours_Users] FOREIGN KEY([BuyerID])
REFERENCES [dbo].[Users] ([AutoID])
GO
ALTER TABLE [dbo].[VRTours] CHECK CONSTRAINT [FK_VRTours_Users]
GO
/****** Object:  ForeignKey [FK_VRTours_Users1]    Script Date: 10/24/2010 20:38:45 ******/
ALTER TABLE [dbo].[VRTours]  WITH CHECK ADD  CONSTRAINT [FK_VRTours_Users1] FOREIGN KEY([SalespersonID])
REFERENCES [dbo].[Users] ([AutoID])
GO
ALTER TABLE [dbo].[VRTours] CHECK CONSTRAINT [FK_VRTours_Users1]
GO
