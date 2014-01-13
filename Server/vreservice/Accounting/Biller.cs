using NHibernate;
using Vre.Server.BusinessLogic;
using System;
using System.Linq;
using Vre.Server.Dao;
using System.Collections.Generic;

namespace Vre.Server.Accounting
{
	class Biller
	{
		public static Invoice CalculateCurrentForBrokerage(
			ISession session,
			BrokerageInfo target)
		{
			var cutTime = DateTime.UtcNow.Date;  // TODO: Should be brokerage's local date!
			var result = new Invoice(target.LastServicePayment, cutTime, target);

			var delta = cutTime.Subtract(target.LastServicePayment);
			var deltaDays = delta.TotalDays / 30.0;

			IList<PricingPolicy> policies;
			using (var dao = new PricingPolicyDao(session))
				policies = dao.GetFor(target);

			IList<User> agents = new List<User>();
			using (var dao = new UserDao(session))
				agents = dao.GetForBrokerage(target, User.Role.Agent);

			var agentHosting = policies.FirstOrDefault(p => p.Service == PricingPolicy.ServiceType.AgentHostingFeeMonthly);
			var agentActive = policies.FirstOrDefault(p => p.Service == PricingPolicy.ServiceType.ActiveAgentMontly);
			foreach (var a in agents)
			{
				var from = newest(a.Created, target.LastServicePayment);
				var period = cutTime.Subtract(from).TotalDays;
				var sum = agentHosting.UnitPrice * (decimal)(period / 30.0);
				result.AddElement(new InvoiceElement(from, cutTime, agentHosting, a, sum));

				bool isActive;
				using (var dao = new ViewOrderDao(session))
					isActive = dao.GetCreatedBy(a.AutoID, from, cutTime).Count > 0;
				if (isActive)
				{
					sum = agentActive.UnitPrice * (decimal)(period / 30.0);
					result.AddElement(new InvoiceElement(from, cutTime, agentActive, a, sum));
				}
			}

			return result;
		}

		private static DateTime newest(DateTime a, DateTime b)
		{
			return (a > b) ? a : b;
		}
	}
}