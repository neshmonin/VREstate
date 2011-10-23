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
