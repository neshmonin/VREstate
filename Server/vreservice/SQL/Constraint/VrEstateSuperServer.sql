-- create constraints VrEstateSuperServer start --


ALTER TABLE [dbo].[aVrEstateSuperServers]	ADD FOREIGN KEY ( [SuperAdminID] )
	REFERENCES [dbo].[aSuperAdmins] ([AutoID])
	 ON UPDATE CASCADE 	 ON DELETE CASCADE 


-- create constraints VrEstateSuperServer end --
