using System;
using System.Diagnostics;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class Invoice : IClientDataProvider
    {
		public enum SubjectType : byte
		{
			Brokerage = 1,
			Agent = 2
		}

        public int AutoID { get; private set; }
        public DateTime Created { get; private set; }

		public SubjectType TargetObjectType { get; private set; }
        public int TargetObjectId { get; private set; }

		public string Contents { get; private set; }

		public string Number
		{
			get { return string.Format("3DCX{0:000}{1:000}/{2:00}", AutoID, Created.DayOfYear, Created.Year - 2000); }
		}

		private Invoice() { }

		public Invoice(SubjectType targetObjectType, int targetObjectId,
			ClientData contents)
        {
			AutoID = 0;
			Created = DateTime.UtcNow;

			TargetObjectType = targetObjectType;
			TargetObjectId = targetObjectId;
			Contents = JavaScriptHelper.ClientDataToJson(contents);
        }

		public ClientData GetClientData()
        {
			// TODO: converting text->json->text
			return JavaScriptHelper.JsonToClientData(Contents);
        }

		public bool UpdateFromClient(ClientData data)
		{
			throw new InvalidOperationException();
		}

		public override string ToString()
		{
			return string.Format("{0}-{1} ({2})",
				TargetObjectType, TargetObjectId, Created);
		}
	}
}
