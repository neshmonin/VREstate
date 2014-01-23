using System;
using System.Collections.Generic;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Accounting
{
	class Invoice : IClientDataProvider
	{
		public readonly DateTime StartTime, EndTime;
		public readonly object Target;

		public readonly Currency TargetCurrency;
		/// <summary>
		/// Rate is a value of currency's major units (e.g. dollars) in single credit unit.
		/// </summary>
		public readonly decimal TargetCurrencyRate;

		public ICollection<ServiceInvoiceElement> Services
		{
			get { return _services.ToArray(); }
		}

        public ICollection<RequestInvoiceElement> Requests
        {
            get { return _requests.ToArray(); }
        }

        public decimal TotalInCreditUnits
		{
			get 
			{ 
				decimal total = 0.0m;
				_services.ForEach(e => total += e.Amount);
                _requests.ForEach(e => total += e.Amount);
                return total;
			}
		}

		public Money TotalInMoney
		{
			get
			{
				return new Money(TotalInCreditUnits * TargetCurrencyRate, TargetCurrency);
			}
		}

		private List<ServiceInvoiceElement> _services;
        private List<RequestInvoiceElement> _requests;

		public Invoice(DateTime from, DateTime to,
			object target)
		{
			if (!(target is User)
				&& !(target is BrokerageInfo))
				throw new ArgumentException("Supported targets are Brokerages and Users");

			StartTime = from;
			EndTime = to;
			Target = target;

			_services = new List<ServiceInvoiceElement>();
            _requests = new List<RequestInvoiceElement>();

			// TODO: HARD-CODED CURRENCY AND RATE
			TargetCurrency = Currency.Cad;
			TargetCurrencyRate = 1.0m;
		}

		public void AddService(ServiceInvoiceElement e) { _services.Add(e); }

        public void AddRequest(RequestInvoiceElement e) { _requests.Add(e); }

        public ClientData GetClientData()
        {
            var result = new ClientData();

            result.Add("from", StartTime);
            result.Add("to", EndTime);

            if (Target is BrokerageInfo)
            {
                var bi = Target as BrokerageInfo;
                result.Add("id", bi.AutoID);
                result.Add("brokerageName", bi.Name);
                result.Add("creditUnits", bi.CreditUnits);
            }
            else if (Target is User)
            {
                var u = Target as User;
                result.Add("id", u.AutoID);
                result.Add("userAddress", u.PrimaryEmailAddress);
                result.Add("creditUnits", u.CreditUnits);
            }

            result.Add("totalUnits", TotalInCreditUnits);
            result.Add("totalCurrency", TotalInMoney.ToString());

            Dictionary<PricingPolicy.ServiceType, List<InvoiceElement>> grouped
                = new Dictionary<PricingPolicy.ServiceType, List<InvoiceElement>>();
            foreach (var ie in _services)
            {
                List<InvoiceElement> g;
                if (!grouped.TryGetValue(ie.Policy.Service, out g))
                {
                    g = new List<InvoiceElement>();
                    grouped.Add(ie.Policy.Service, g);
                }
                g.Add(ie);
            }

            var groups = new List<ClientData>(grouped.Count);
            foreach (var kvp in grouped)
            {
                var group = new ClientData();
                var list = new ClientData[kvp.Value.Count];
                group.Add("type", kvp.Key.ToString());
                var total = 0m;
                for (int idx = list.Length - 1; idx >= 0; idx--)
                {
                    list[idx] = kvp.Value[idx].GetClientData();
                    total += kvp.Value[idx].Amount;
                }
                group.Add("total", total);
                group.Add("details", list);
                groups.Add(group);
            }
            result.Add("services", groups.ToArray());

            groups = new List<ClientData>();
            {
                _requests.Sort((a, b) => a.Request.Created.CompareTo(b.Request.Created));

                var group = new ClientData();
                var list = new ClientData[_requests.Count];
                var total = 0m;
                for (int idx = _requests.Count - 1; idx >= 0; idx--)
                {
                    list[idx] = _requests[idx].GetClientData();
                    total += _requests[idx].Amount;
                }
                group.Add("total", total);
                group.Add("details", list);
                groups.Add(group);
            }
            result.Add("requests", groups.ToArray());

            result.Add("note", "Requests are already debited into Credit Units.");

            return result;
        }

        //private List<ClientData> getClientDataGrouped(List<InvoiceElement> elements)
        //{
        //    Dictionary<PricingPolicy.ServiceType, List<InvoiceElement>> grouped
        //        = new Dictionary<PricingPolicy.ServiceType, List<InvoiceElement>>();
        //    foreach (var ie in elements)
        //    {
        //        List<InvoiceElement> g;
        //        if (!grouped.TryGetValue(ie.Policy.Service, out g))
        //        {
        //            g = new List<InvoiceElement>();
        //            grouped.Add(ie.Policy.Service, g);
        //        }
        //        g.Add(ie);
        //    }

        //    var groups = new List<ClientData>(grouped.Count);
        //    foreach (var kvp in grouped)
        //    {
        //        var group = new ClientData();
        //        var list = new ClientData[kvp.Value.Count];
        //        group.Add("type", kvp.Key.ToString());
        //        var total = 0m;
        //        for (int idx = list.Length - 1; idx >= 0; idx--)
        //        {
        //            list[idx] = kvp.Value[idx].GetClientData();
        //            total += kvp.Value[idx].Amount;
        //        }
        //        group.Add("total", total);
        //        group.Add("details", list);
        //        groups.Add(group);
        //    }

        //    return groups;
        //}
		
		public bool UpdateFromClient(ClientData data)
		{
			throw new InvalidOperationException();
		}
	}
}