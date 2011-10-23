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
