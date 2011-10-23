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
