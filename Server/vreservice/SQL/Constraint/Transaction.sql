-- create constraints Transaction start --


ALTER TABLE [dbo].[aTransactions]	ADD FOREIGN KEY ( [OptionPaidID] )
	REFERENCES [dbo].[aOptions] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Transaction end --
