
namespace Vre.Server.BusinessLogic
{
	public class NamedSearchFilter : UpdateableBase
	{
		#region properties
		public int OwnerId { get; private set; }

		public int RelatedUserId { get; set; }
		public string Note { get; set; }

		public string Filter { get; set; }
		public string ViewPointInfo { get; set; }
		#endregion

		#region local constructors
		private NamedSearchFilter() { }

		public NamedSearchFilter(User owner) : this(owner, null) { }

		public NamedSearchFilter(User owner, User relatedUser) : base()
		{
			InitializeNew();
			OwnerId = owner.AutoID;
			RelatedUserId = (relatedUser != null) ? relatedUser.AutoID : -1;
			Note = null;
			Filter = null;
			ViewPointInfo = null;
		}
		#endregion

		#region serialization support
		public NamedSearchFilter(ClientData data) : base(data)
        {
            OwnerId = data.GetProperty("ownerId", -1);
			// all the rest is picked via UpdateFromClient invoked by base c'tor
		}

		public override ClientData GetClientData()
		{
			ClientData result = base.GetClientData();

			result.Add("ownerId", OwnerId);  // read only

			if (RelatedUserId >= 0) result.Add("relatedUserId", RelatedUserId);
			if (Note != null) result.Add("note", Note);

			if (Filter != null) result.Add("filter", Filter);
			if (ViewPointInfo != null) result.Add("viewPointInfo", ViewPointInfo);

			return result;
		}

		public override bool UpdateFromClient(ClientData data)
		{
			bool changed = base.UpdateFromClient(data);

			RelatedUserId = data.UpdateProperty("relatedUserId", RelatedUserId, ref changed);
			Note = data.UpdateProperty("note", Note, ref changed);

			Filter = data.UpdateProperty("filter", Filter, ref changed);
			ViewPointInfo = data.UpdateProperty("viewPointInfo", ViewPointInfo, ref changed);

			return changed;
		}
		#endregion
	}
}