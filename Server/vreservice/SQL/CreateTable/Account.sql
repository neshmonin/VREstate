-- create table Account start --

CREATE TABLE [dbo].[aAccounts](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[SuiteID] [int]  NOT NULL ,
	-- Transactions is populated by another table --
	[CurrentBalance] [float]  NOT NULL ,
	[CustomerID] [int]  NOT NULL ,
	[CurrencyCents] [bigint]  NOT NULL ,
	[ParentID] [int]  NOT NULL ,
)

-- create table Account end --
