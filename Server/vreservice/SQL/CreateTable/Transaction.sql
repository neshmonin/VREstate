-- create table Transaction start --

CREATE TABLE [dbo].[aTransactions](
	[AutoID]  [int] IDENTITY PRIMARY KEY  NOT NULL,
	[Amount] [float]  NOT NULL ,
	[OptionPaidID] [int] NULL,
	[DealDateTime] [DateTime]  NOT NULL ,
	[PaidDateTime] [DateTime]  NOT NULL ,
	[Status] [tinyint]  NOT NULL ,
)

-- create table Transaction end --
