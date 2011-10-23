using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic.Client
{
	[Serializable]
	public partial class SuiteEx : Suite
	{
        //public bool Changed { get; set; }

        public double CurrentPrice { get; set; }

        protected SuiteEx() { }

        /// <summary>
        /// C'tor for server side
        /// </summary>
        public SuiteEx(Suite baseData, double currentPrice) : base(baseData)
        {
            CurrentPrice = currentPrice;
        }

        /// <summary>
        /// C'tor for client side: restore from data sent by server.
        /// </summary>
        public SuiteEx(ClientData fromServer) : base(fromServer)
        {
            //Changed = false;
            CurrentPrice = fromServer.GetProperty("currentPrice", -1.0);
        }

        public static ClientData GetClientData(Suite s, double currentPrice)
        {
            ClientData result = s.GetClientData();

            if (!(s is SuiteEx)) result.Add("currentPrice", currentPrice);

            return result;
        }

        public override ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

            result.Add("currentPrice", CurrentPrice);

            return result;
        }

        /// <summary>
        /// This is a NEW method required to detect child-level updates separately, please call base class method explicitly.
        /// </summary>
        public new bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

            CurrentPrice = data.UpdateProperty("currentPrice", CurrentPrice, ref changed);

            return changed;
        }
    }
}
