using System;
using System.Diagnostics;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class PricingPolicy : UpdateableBase, IComparable
    {
		public enum SubjectType : byte
		{
			Brokerage = 1,
			Agent = 2
		}

		public enum ServiceType : int
		{
			ActiveAgentMontly = 1,
			AgentHostingFeeMonthly = 2,
			Interactive3DLayoutCreate = 101, 
			Interactive3DLayoutMonthly = 201, 
			ConferenceMonthly = 202
		}

		public SubjectType TargetObjectType { get; private set; }
        public int TargetObjectId { get; private set; }
		public ServiceType Service { get; private set; }
		public decimal UnitPrice { get; private set; }

		private PricingPolicy() { }

		public PricingPolicy(SubjectType targetObjectType, int targetObjectId,
			ServiceType service, decimal unitPrice)
        {
			InitializeNew();
			TargetObjectType = targetObjectType;
			TargetObjectId = targetObjectId;
			Service = service;
			UnitPrice = unitPrice;
        }

		public PricingPolicy(ClientData data)
            : base(data)
        {
			TargetObjectType = data.GetProperty<SubjectType>("targetObjectType", SubjectType.Brokerage);
			TargetObjectId = data.GetProperty("targetObjectId", 0);
			Service = data.GetProperty<ServiceType>("service", ServiceType.ActiveAgentMontly);
			UpdateFromClient(data);
        }

		public ClientData GetClientData()
        {
			ClientData result = base.GetClientData();

			result.Add("targetObjectType", ClientData.ConvertProperty<SubjectType>(TargetObjectType));
			result.Add("targetObjectId", TargetObjectId);
			result.Add("service", ClientData.ConvertProperty<ServiceType>(Service));
			result.Add("unitPrice", UnitPrice);

            return result;
        }

        public bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

			UnitPrice = data.UpdateProperty("unitPrice", UnitPrice, ref changed);

            return changed;
        }

		public override string ToString()
		{
			return string.Format("{0}-{1}-{2}->{3}",
				TargetObjectType, TargetObjectId, Service, UnitPrice);
		}

		public int CompareTo(object other)
		{
			PricingPolicy opp = other as PricingPolicy;
			if (null == opp) return 1;
			return Service.CompareTo(opp.Service);
		}
    }
}
