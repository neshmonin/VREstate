using System;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Accounting
{
    abstract class InvoiceElement : IClientDataProvider
    {
        public abstract decimal Amount { get; }

        public InvoiceElement() { }

        public abstract ClientData GetClientData();

        public bool UpdateFromClient(ClientData data)
        {
            throw new InvalidOperationException();
        }
    }

    class RequestInvoiceElement : InvoiceElement
    {
        public readonly ServiceRequestItem Request;
        public readonly object Subject;

        public override decimal Amount { get { return Request.Price; } }

        public RequestInvoiceElement(ServiceRequestItem request, object subject)
        {
            Request = request;
            Subject = subject;
        }

        public override ClientData GetClientData()
        {
            var result = new ClientData();

            result.Add("time", Request.Created);

            switch (Request.ServiceObjectType)
            {
                case ServiceRequestItem.ServiceType.ViewOrder:
                    {
                        var vo = Subject as ViewOrder;
                        if (vo != null)
                        {
                            result.Add("type", "viewOrder");
                            result.Add("id", UniversalId.GenerateUrlId(UniversalId.IdType.ViewOrder, vo.AutoID));
                            result.Add("mlsId", string.IsNullOrEmpty(vo.MlsId) ? "N/A" : vo.MlsId);
                        }
                        else
                        {
                            result.Add("type", "viewOrder");
                            result.Add("id", "N/A");
                            result.Add("mlsId", "N/A");
                        }
                    }
                    break;

                default:
                    result.Add("type", Request.ServiceObjectType.ToString());
                    break;
            }

            result.Add("effectiveCost", Request.Price);

            return result;
        }
    }

	class ServiceInvoiceElement : InvoiceElement
	{
		public readonly DateTime StartTime, EndTime;
		public readonly PricingPolicy Policy;
		public readonly object Subject;
        public override decimal Amount { get { return _amount; } }

		private readonly decimal _amount;

		public ServiceInvoiceElement(DateTime from, DateTime to,
			PricingPolicy policy, decimal amount)
		{
			StartTime = from;
			EndTime = to;
			Policy = policy;
			Subject = null;
			_amount = amount;
		}

        public ServiceInvoiceElement(DateTime from, DateTime to,
			PricingPolicy policy, object subject, decimal amount)
		{
			if (!(subject is ViewOrder)
				&& !(subject is User))
				throw new ArgumentException("Supported subjects are Users and ViewOrders");

			StartTime = from;
			EndTime = to;
			Policy = policy;
			Subject = subject;
			_amount = amount;
		}

		public override ClientData GetClientData()
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
	}
}