using System;
using Vre.Server;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Accounting
{
	class InvoiceElement : IClientDataProvider
	{
		public readonly DateTime StartTime, EndTime;
		public readonly PricingPolicy Policy;
		public readonly object Subject;
		public readonly decimal Amount;

		public InvoiceElement(DateTime from, DateTime to,
			PricingPolicy policy, decimal amount)
		{
			StartTime = from;
			EndTime = to;
			Policy = policy;
			Subject = null;
			Amount = amount;
		}

		public InvoiceElement(DateTime from, DateTime to,
			PricingPolicy policy, object subject, decimal amount)
		{
			if (!(subject is ViewOrder)
				&& !(subject is User))
				throw new ArgumentException("Supported subjects are Users and ViewOrders");

			StartTime = from;
			EndTime = to;
			Policy = policy;
			Subject = subject;
			Amount = amount;
		}

		public ClientData GetClientData()
		{
			var result = new ClientData();

			result.Add("from", StartTime);
			result.Add("to", EndTime);
			result.Add("pid", Policy.AutoID);
			result.Add("pup", Policy.UnitPrice);
			result.Add("pu", Policy.Updated);
			result.Add("effectiveCost", Amount);

			if (Subject is ViewOrder)
			{
				var vo = Subject as ViewOrder;
				result.Add("id", UniversalId.GenerateUrlId(UniversalId.IdType.ViewOrder, vo.AutoID));
				result.Add("mlsId", string.IsNullOrEmpty(vo.MlsId) ? "N/A" : vo.MlsId);
			}
			else if (Subject is User)
			{
				var u = Subject as User;
				result.Add("id", u.AutoID);
				result.Add("userAddress", u.PrimaryEmailAddress);
			}

			return result;
		}

		public bool UpdateFromClient(ClientData data)
		{
			throw new InvalidOperationException();
		}
	}
}