using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.Accounting
{
	class Biller
	{
		public static Invoice CalculateCurrentForBrokerage(
			ISession session,
			BrokerageInfo target)
		{
            var from = newest(target.Created.Date, target.LastServicePayment);
            var cutTime = DateTime.UtcNow.Date;  // TODO: Should be brokerage's local date!
			var result = new Invoice(from, cutTime, target);

            //var delta = cutTime.Subtract(target.LastServicePayment);
            //var deltaDays = delta.TotalDays / 30.0;

			IList<PricingPolicy> policies;
			using (var dao = new PricingPolicyDao(session))
				policies = dao.GetFor(target);

			IList<User> agents = new List<User>();
			using (var dao = new UserDao(session))
				agents = dao.GetForBrokerage(target, User.Role.Agent);

			var agentHosting = policies.FirstOrDefault(p => p.Service == PricingPolicy.ServiceType.AgentHostingFeeMonthly);
			var agentActive = policies.FirstOrDefault(p => p.Service == PricingPolicy.ServiceType.ActiveAgentMontly);
            List<ViewOrder> vosCreatedInPeriod = new List<ViewOrder>();
			foreach (var a in agents)
			{
				from = newest(a.Created.Date, target.LastServicePayment);
                var monthsSpent = monthsInPeriod(from, cutTime);

				var sum = agentHosting.UnitPrice * monthsSpent;
                result.AddService(new ServiceInvoiceElement(from, cutTime, agentHosting, a, sum));

                IList<ViewOrder> vol;
				using (var dao = new ViewOrderDao(session))
                    vol = dao.GetCreatedBy(a.AutoID, from, cutTime);
				if (vol.Count > 0)  // agent is active
				{
					sum = agentActive.UnitPrice * monthsSpent;
                    result.AddService(new ServiceInvoiceElement(from, cutTime, agentActive, a, sum));
                    vosCreatedInPeriod.AddRange(vol);
				}
			}

            //using (var dao = new ServiceRequestItemDao(session))
            //{
            //    foreach (var sr in dao.GetByTargetAndTime(BusinessLogic.Invoice.SubjectType.Brokerage, target.AutoID, result.StartTime, result.EndTime))
            //    {
            //        object subject = null;
            //        switch (sr.ServiceObjectType)
            //        {
            //            case ServiceRequestItem.ServiceType.ViewOrder:
            //                // TODO: retrieve ViewOrder into subject
            //                break;
            //        }
            //        result.AddRequest(new RequestInvoiceElement(sr, subject));
            //    }
            //}

			return result;
		}

        public static Invoice CalculateCurrentForAgent(
            ISession session,
            User target)
        {
            if ((target.UserRole != User.Role.Agent)
                && (target.UserRole != User.Role.BuyingAgent)
                && (target.UserRole != User.Role.SellingAgent))
                throw new InvalidOperationException("User ID=" + target.AutoID + " is not an agent");

            var cutTime = DateTime.UtcNow.Date;  // TODO: Should be user's local date!
            var result = new Invoice(target.LastServicePayment, cutTime, target);

            IList<PricingPolicy> policies;
            using (var dao = new PricingPolicyDao(session))
                policies = dao.GetFor(target);

            // TODO: monthly payments

            //using (var dao = new ServiceRequestItemDao(session))
            //{
            //    foreach (var sr in dao.GetByTargetAndTime(BusinessLogic.Invoice.SubjectType.Agent, target.AutoID, result.StartTime, result.EndTime))
            //    {
            //        object subject = null;
            //        switch (sr.ServiceObjectType)
            //        {
            //            case ServiceRequestItem.ServiceType.ViewOrder:
            //                // TODO: retrieve ViewOrder into subject
            //                break;
            //        }
            //        result.AddRequest(new RequestInvoiceElement(sr, subject));
            //    }
            //}

            return result;
        }

		private static DateTime newest(DateTime a, DateTime b)
		{
			return (a > b) ? a : b;
		}

        private static readonly Calendar _systemCalendar = new GregorianCalendar();

        /// <summary>
        /// Calculate correct fractional months from date to date taking months' length into account.
        /// "From" day is countd and "to" day is not counted.
        /// </summary>
        private static decimal monthsInPeriod(DateTime from, DateTime to)
        {
            Debug.Assert(from.TimeOfDay.Ticks == 0, "FROM time of day is not empty!");
            Debug.Assert(to.TimeOfDay.Ticks == 0, "TO time of day is not empty!");

            if ((from.Month == to.Month) && (from.Year == to.Year))
            {
                return (decimal)(to.Subtract(from).TotalDays / (double)_systemCalendar.GetDaysInMonth(from.Year, from.Month));
            }
            else
            {
                // days until end of "from" month
                int m1 = _systemCalendar.GetDaysInMonth(from.Year, from.Month);
                int d1 = m1 - from.Day + 1;
                double result = (double)d1 / (double)m1;

                // days since beginning of "to" month
                result += (double)(to.Day - 1) / (double)_systemCalendar.GetDaysInMonth(to.Year, to.Month);

                if (from.Year == to.Year)
                {
                    // add full months
                    result += to.Month - from.Month - 1;
                }
                else
                {
                    // months until end of "from" year
                    result += 12 - from.Month;
                    // months since beginning of "to" year
                    result += to.Month - 1;
                    // full years
                    result += (to.Year - from.Year - 1) * 12.0;
                }
                return (decimal)result;
            }
        }
	}
}