using Vre.Server.Mls;
using System;
namespace Vre.Server.BusinessLogic
{
	internal class MlsInfo : UpdateableBase
	{
		public string MlsNum { get; private set; }
		public string RawInfo { get; set; }

		private MlsInfo() { }

		public MlsInfo(MlsItem source) : base()
		{
			InitializeNew();
			MlsNum = source.MlsId;
			Update(source);
		}

		public void Update(MlsItem source)
		{
			if (!source.MlsId.Equals(MlsNum)) throw new InvalidOperationException("Incorrect MLS#");

			var rawData = new ClientData();
			foreach (var rp in source.RawData) rawData.Add(rp.Key, rp.Value);
			RawInfo = JavaScriptHelper.ClientDataToJson(rawData);

			MarkUpdated();
		}

		public ClientData RawInfoAsClientData { get { return JavaScriptHelper.JsonToClientData(RawInfo); } }
	}
}