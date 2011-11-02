-- creates default suite level for suite types having no suite levels


declare @stid int;
declare stc cursor for select st.autoid from suiteTypes st left outer join suitelevels sl on sl.suitetypeid = st.autoid where sl.autoid is null;
open stc;
fetch next from stc into @stid;
while @@fetch_status = 0
begin
	print @stid;

	insert into suitelevels (created, updated, deleted, suitetypeid, levelorder, [name], model, bedrooms, dens, toilets, showers, baths, balconies)
		values (getutcdate(), getutcdate(), 0, @stid, 0, 'Main Floor', null, 2, 0, 0, 0, 1, 1);

	fetch next from stc into @stid;
end
close stc;
deallocate stc;