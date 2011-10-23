-- create table Salesperson start --

CREATE TABLE [dbo].[aSalespersons](
	[SalespersonID] [int] PRIMARY KEY NOT NULL,
	-- VRTours is populated by another table --
	[ContactInfoID] [int] NULL,
)

-- create table Salesperson end --
