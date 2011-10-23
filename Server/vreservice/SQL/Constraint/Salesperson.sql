-- create constraints Salesperson start --


ALTER TABLE [dbo].[aSalespersons]	ADD FOREIGN KEY ( [ContactInfoID] )
	REFERENCES [dbo].[aContactInfos] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Salesperson end --
