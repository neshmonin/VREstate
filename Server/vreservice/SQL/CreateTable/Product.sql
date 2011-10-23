-- create table Product start --

CREATE TABLE [dbo].[aProducts](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Name] [nvarchar](MAX)  NULL ,
	[Category] [nvarchar](MAX)  NULL ,
	[Discontinued] [bit]  NOT NULL ,
)

-- create table Product end --
