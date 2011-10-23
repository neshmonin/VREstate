-- create constraints Subcontractor start --


ALTER TABLE [dbo].[aSubcontractors]	ADD FOREIGN KEY ( [ContactInfoID] )
	REFERENCES [dbo].[aContactInfos] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints Subcontractor end --
