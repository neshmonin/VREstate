Use Master

-- Drop/Create DB
Drop Database VR -- Drop the Database
CREATE DATABASE VR-- Create a clean DB


--- Setup the users
Use VR
EXEC sp_grantdbaccess 'vr', 'vr'
EXEC sp_addrolemember 'db_owner', 'vr'



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



-- create table AvailableLanguage start --
CREATE TABLE [dbo].[aProducts](
	[Id]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Name] [nvarchar](MAX)  NULL ,
	[Category] [nvarchar](MAX)  NULL ,
	[Discontinued] [bit]  NOT NULL ,
)

CREATE TABLE [dbo].[aAccounts](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[SuiteID] [int]  NOT NULL ,
	-- Transactions is populated by another table --
	[CurrentBalance] [float]  NOT NULL ,
	[CustomerID] [int]  NOT NULL ,
	[CurrencyCents] [bigint]  NOT NULL ,
)

-- create table Account end --
-- create table Building start --

CREATE TABLE [dbo].[aBuildings](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[LocationID] [int] NULL,
	[OpenningDate] [DateTime]  NOT NULL ,
	[Status] [tinyint]  NOT NULL ,
	-- Suites is populated by another table --
	[Model] [nvarchar](MAX)  NULL ,
)

-- create table Building end --
-- create table Buyer start --

CREATE TABLE [dbo].[aBuyers](
	[UserID] [int] PRIMARY KEY NOT NULL,
	-- Accounts is populated by another table --
	-- VRTours is populated by another table --
	[VRContactInfoID] [int] NULL,
)

-- create table Buyer end --
-- create table ContactInfo start --

CREATE TABLE [dbo].[aContactInfos](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Emails] [nvarchar](MAX)  NULL ,
	-- PhoneNumbers is populated by another table --
	[AddressID] [int] NULL,
	[PersonalInfoID] [int] NULL,
)

-- create table ContactInfo end --
-- create table Credetials start --

CREATE TABLE [dbo].[aCredetialss](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Nickname] [nvarchar](MAX)  NULL ,
	[Password] [nvarchar](MAX)  NULL ,
)

-- create table Credetials end --
-- create table Option start --

CREATE TABLE [dbo].[aOptions](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Description] [nvarchar](MAX)  NULL ,
	-- Prices is populated by another table --
	[ProviderID] [int] NULL,
	[CutoffDay] [DateTime]  NOT NULL ,
)

-- create table Option end --
-- create table PersonalInfo start --

CREATE TABLE [dbo].[aPersonalInfos](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[FirstName] [nvarchar](MAX)  NULL ,
	[LastName] [nvarchar](MAX)  NULL ,
	[MidName] [nvarchar](MAX)  NULL ,
	[PersonalTitle] [tinyint]  NOT NULL ,
)

-- create table PersonalInfo end --
-- create table PhoneNumber start --

CREATE TABLE [dbo].[aPhoneNumbers](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Number] [nvarchar](MAX)  NULL ,
	[PhoneExtension] [nvarchar](MAX)  NULL ,
)

-- create table PhoneNumber end --
-- create table PostalAddress start --

CREATE TABLE [dbo].[aPostalAddresss](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Line1] [nvarchar](MAX)  NULL ,
	[Line2] [nvarchar](MAX)  NULL ,
	[City] [nvarchar](MAX)  NULL ,
	[State_province] [nvarchar](MAX)  NULL ,
	[Zipcode] [nvarchar](MAX)  NULL ,
	[Country] [nvarchar](MAX)  NULL ,
)

-- create table PostalAddress end --
-- create table Price start --

CREATE TABLE [dbo].[aPrices](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[OriginalCurrencyCents] [bigint]  NOT NULL ,
	[PricePerUnitForSubcontractor] [float]  NOT NULL ,
	[PricePerUnitForBuyer] [float]  NOT NULL ,
	[UnitName] [nvarchar](MAX)  NULL ,
	[NumberOfUnits] [float]  NOT NULL ,
	[StartingDate] [DateTime]  NOT NULL ,
)

-- create table Price end --
-- create table Salesperson start --

CREATE TABLE [dbo].[aSalespersons](
	[SalespersonID] [int] PRIMARY KEY NOT NULL,
	-- VRTours is populated by another table --
	[ContactInfoID] [int] NULL,
)

-- create table Salesperson end --
-- create table Subcontractor start --

CREATE TABLE [dbo].[aSubcontractors](
	[SubcontractorID] [int] PRIMARY KEY NOT NULL,
	[ContactInfoID] [int] NULL,
)

-- create table Subcontractor end --
-- create table Suite start --

CREATE TABLE [dbo].[aSuites](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[PhysicalLevelNumber] [int]  NOT NULL ,
	[FloorNumber] [int]  NOT NULL ,
	[SuiteNumber] [int]  NOT NULL ,
	[Status] [tinyint]  NOT NULL ,
	-- Options is populated by another table --
	[Model] [nvarchar](MAX)  NULL ,
	-- VRTours is populated by another table --
	[AccountID] [int]  NOT NULL ,
)

-- create table Suite end --
-- create table SuperAdmin start --

CREATE TABLE [dbo].[aSuperAdmins](
	[SuperAdminID] [int] PRIMARY KEY NOT NULL,
	[Test] [nvarchar](MAX)  NULL ,
)

-- create table SuperAdmin end --
-- create table Transaction start --

CREATE TABLE [dbo].[aTransactions](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Amount] [float]  NOT NULL ,
	[OptionPaidID] [int] NULL,
	[DealDateTime] [DateTime]  NOT NULL ,
	[PaidDateTime] [DateTime]  NOT NULL ,
	[Status] [tinyint]  NOT NULL ,
	[AccountID] [int] NULL,
)

-- create table Transaction end --
-- create table User start --

CREATE TABLE [dbo].[aUsers](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[CredentialsID] [int] NULL,
	[VRRole] [tinyint]  NOT NULL ,
)

-- create table User end --
-- create table VrEstateDeveloper start --

CREATE TABLE [dbo].[aVrEstateDevelopers](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	-- Buildings is populated by another table --
	-- Subcontractors is populated by another table --
	-- Buyers is populated by another table --
	-- Salespersons is populated by another table --
	[VRConfiguration] [tinyint]  NOT NULL ,
)

-- create table VrEstateDeveloper end --
-- create table VrEstateSuperServer start --

CREATE TABLE [dbo].[aVrEstateSuperServers](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	-- Developers is populated by another table --
	-- Users is populated by another table --
	[SuperAdminID] [int] NULL,
	-- Subcontractors is populated by another table --
	-- Buyers is populated by another table --
	-- Salespersons is populated by another table --
)

-- create table VrEstateSuperServer end --
-- create table VRTour start --

CREATE TABLE [dbo].[aVRTours](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[StartedOn] [DateTime]  NOT NULL ,
	[SuiteID] [int]  NOT NULL ,
	[BuyerID] [int]  NOT NULL ,
	[SalespersonID] [int]  NOT NULL ,
	[TourRecording] [nvarchar](MAX)  NULL ,
)

-- create table VRTour end --
-- create constraints Account start --



-- create constraints Account end --
-- create constraints Building start --


ALTER TABLE [dbo].[aBuildings]	ADD FOREIGN KEY ( [LocationID] )
	REFERENCES [dbo].[aPostalAddresss] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Building end --
-- create constraints Buyer start --


ALTER TABLE [dbo].[aBuyers]	ADD FOREIGN KEY ( [VRContactInfoID] )
	REFERENCES [dbo].[aContactInfos] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Buyer end --
-- create constraints ContactInfo start --


ALTER TABLE [dbo].[aContactInfos]	ADD FOREIGN KEY ( [AddressID] )
	REFERENCES [dbo].[aPostalAddresss] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 

ALTER TABLE [dbo].[aContactInfos]	ADD FOREIGN KEY ( [PersonalInfoID] )
	REFERENCES [dbo].[aPersonalInfos] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints ContactInfo end --
-- create constraints Credetials start --



-- create constraints Credetials end --
-- create constraints Option start --


ALTER TABLE [dbo].[aOptions]	ADD FOREIGN KEY ( [ProviderID] )
	REFERENCES [dbo].[aSubcontractors] ([SubcontractorID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Option end --
-- create constraints PersonalInfo start --



-- create constraints PersonalInfo end --
-- create constraints PhoneNumber start --



-- create constraints PhoneNumber end --
-- create constraints PostalAddress start --



-- create constraints PostalAddress end --
-- create constraints Price start --



-- create constraints Price end --
-- create constraints Salesperson start --


ALTER TABLE [dbo].[aSalespersons]	ADD FOREIGN KEY ( [ContactInfoID] )
	REFERENCES [dbo].[aContactInfos] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Salesperson end --
-- create constraints Subcontractor start --


ALTER TABLE [dbo].[aSubcontractors]	ADD FOREIGN KEY ( [ContactInfoID] )
	REFERENCES [dbo].[aContactInfos] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Subcontractor end --
-- create constraints Suite start --



-- create constraints Suite end --
-- create constraints SuperAdmin start --



-- create constraints SuperAdmin end --
-- create constraints Transaction start --


ALTER TABLE [dbo].[aTransactions]	ADD FOREIGN KEY ( [OptionPaidID] )
	REFERENCES [dbo].[aOptions] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 

ALTER TABLE [dbo].[aTransactions]	ADD FOREIGN KEY ( [AccountID] )
	REFERENCES [dbo].[aAccounts] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 
-- create constraints Transaction end --
-- create constraints User start --


ALTER TABLE [dbo].[aUsers]	ADD FOREIGN KEY ( [CredentialsID] )
	REFERENCES [dbo].[aCredetialss] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints User end --
-- create constraints VrEstateDeveloper start --



-- create constraints VrEstateDeveloper end --
-- create constraints VrEstateSuperServer start --


ALTER TABLE [dbo].[aVrEstateSuperServers]	ADD FOREIGN KEY ( [SuperAdminID] )
	REFERENCES [dbo].[aSuperAdmins] ([SuperAdminID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 







-- Initialize database --

INSERT INTO [DbSettings] ([Id], [IntValue]) VALUES (1, 1)
INSERT INTO [DbSettings] ([Id], [TimeValue]) VALUES (2, GETDATE())