using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Vre.Server.BusinessLogic;
using System;

namespace Vre.Server.Dao
{
	internal static class PricingPolicyFallbackRules
	{
		private static Dictionary<PricingPolicy.SubjectType, List<PricingPolicy.SubjectType>> _searchRules;
		private static List<PricingPolicy.ServiceType> _services;

		static PricingPolicyFallbackRules()
		{
			_services = new List<PricingPolicy.ServiceType>();
			// we do not care of specific order really
			foreach (PricingPolicy.ServiceType pps in Enum.GetValues(typeof(PricingPolicy.ServiceType)))
				_services.Add(pps);

			_searchRules = new Dictionary<PricingPolicy.SubjectType, List<PricingPolicy.SubjectType>>();

			_searchRules.Add(PricingPolicy.SubjectType.Brokerage, new List<PricingPolicy.SubjectType>(new[] 
				{ PricingPolicy.SubjectType.Brokerage }));

			_searchRules.Add(PricingPolicy.SubjectType.Agent, new List<PricingPolicy.SubjectType>(new[] 
				{ PricingPolicy.SubjectType.Agent, PricingPolicy.SubjectType.Brokerage }));
		}

		public static IList<PricingPolicy> OrderFor(this IList<PricingPolicy> items, PricingPolicy.SubjectType subject)
		{
			List<PricingPolicy.SubjectType> rule;
			if (!_searchRules.TryGetValue(subject, out rule)) return items;

			// The rule is:
			// 1. Rules targeted specific objects ordered by rule chain (e.g. first for agent, then brokerage)
			// 2. default rules ordered by rule chain (e.g. first for agent, then brokerage)
			// Result sample would be: agent personal, brokerage dedicated, agent default, brokerage default
			// If multiple services' rules are ordered, top N items shall be effective values.
			return items.OrderBy(p => (
				((0 == p.TargetObjectId) ? 1000000 : 0) +
				(rule.IndexOf(p.TargetObjectType) * 1000) +
				_services.IndexOf(p.Service)
				)).ToList();
		}
	}

    internal class PricingPolicyDao : UpdateableBaseDao<PricingPolicy>
    {
		public PricingPolicyDao(ISession session) : base(session) { }

		public decimal? GetEffectivePriceForBrokerage(BrokerageInfo b, PricingPolicy.ServiceType service)
		{
			var policy = GetFor(b, service).FirstOrDefault();

			return (policy != null) ? policy.UnitPrice : (decimal?)null;
		}

		public decimal? GetEffectivePriceForAgent(User u, PricingPolicy.ServiceType service)
		{
			var policy = GetFor(u, service).FirstOrDefault();

			return (policy != null) ? policy.UnitPrice : (decimal?)null;
		}

		/// <summary>
		/// Get policy list for specific target, ordered by fallback rules.
		/// </summary>
		public IList<PricingPolicy> GetFor(object target)
		{
			var user = target as User;
			if ((user != null) && (user.BrokerInfo != null))
			{
				return _session.CreateQuery(@"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE Deleted=0
AND ((TargetObjectType=:ot1 AND (TargetObjectId=:oid1 OR TargetObjectId=0))
OR (TargetObjectType=:ot2 AND (TargetObjectId=:oid2 OR TargetObjectId=0)))")
					.SetEnum("ot1", PricingPolicy.SubjectType.Brokerage)
					.SetInt32("oid1", user.BrokerInfo.AutoID)
					.SetEnum("ot2", PricingPolicy.SubjectType.Agent)
					.SetInt32("oid2", user.AutoID)
					.List<PricingPolicy>().OrderFor(PricingPolicy.SubjectType.Agent);
			}
			var brokerage = target as BrokerageInfo;
			if (brokerage != null)
			{
				return _session.CreateQuery(@"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE Deleted=0
AND TargetObjectType=:ot AND (TargetObjectId=:oid OR TargetObjectId=0)")
					.SetEnum("ot", PricingPolicy.SubjectType.Brokerage)
					.SetInt32("oid", brokerage.AutoID)
					.List<PricingPolicy>().OrderFor(PricingPolicy.SubjectType.Brokerage);
			}
			return new List<PricingPolicy>(0);
		}

		/// <summary>
		/// Get policy list for specific target and service, ordered by fallback rules.
		/// </summary>
		public IList<PricingPolicy> GetFor(object target, PricingPolicy.ServiceType service)
		{
			var user = target as User;
			if ((user != null) && (user.BrokerInfo != null))
			{
				return _session.CreateQuery(@"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE Deleted=0 AND Service=:srv
AND ((TargetObjectType=:ot1 AND (TargetObjectId=:oid1 OR TargetObjectId=0))
OR (TargetObjectType=:ot2 AND (TargetObjectId=:oid2 OR TargetObjectId=0)))")
					.SetEnum("srv", service)
					.SetEnum("ot1", PricingPolicy.SubjectType.Brokerage)
					.SetInt32("oid1", user.BrokerInfo.AutoID)
					.SetEnum("ot2", PricingPolicy.SubjectType.Agent)
					.SetInt32("oid2", user.AutoID)
					.List<PricingPolicy>().OrderFor(PricingPolicy.SubjectType.Agent);
			}
			var brokerage = target as BrokerageInfo;
			if (brokerage != null)
			{
				return _session.CreateQuery(@"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE Deleted=0 AND Service=:srv
AND TargetObjectType=:ot AND (TargetObjectId=:oid OR TargetObjectId=0)")
					.SetEnum("srv", service)
					.SetEnum("ot", PricingPolicy.SubjectType.Brokerage)
					.SetInt32("oid", brokerage.AutoID)
					.List<PricingPolicy>().OrderFor(PricingPolicy.SubjectType.Brokerage);
			}
			return new List<PricingPolicy>(0);
		}

		public IList<PricingPolicy> GetDefaults(bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId=0";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q).List<PricingPolicy>();
		}

		public IList<PricingPolicy> GetDefaults(PricingPolicy.ServiceType service, bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId=0 AND Service=:srv";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.SetEnum("srv", service)
				.List<PricingPolicy>();
		}

		public IList<PricingPolicy> GetDefaults(PricingPolicy.SubjectType subject, bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId=0 AND TargetObjectType=:ot";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.SetEnum("ot", subject)
				.List<PricingPolicy>();
		}

		public IList<PricingPolicy> GetDefaults(PricingPolicy.SubjectType subject, PricingPolicy.ServiceType service, bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId=0 AND Service=:srv AND TargetObjectType=:ot";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.SetEnum("srv", service)
				.SetEnum("ot", subject)
				.List<PricingPolicy>();
		}

		public bool Exists(PricingPolicy.SubjectType subject, int subjectId, PricingPolicy.ServiceType service)
		{
			return _session.CreateQuery(@"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE Deleted=0 AND TargetObjectId=:oid AND Service=:srv AND TargetObjectType=:ot")
				.SetEnum("srv", service)
				.SetInt32("oid", subjectId)
				.SetEnum("ot", subject)
				.UniqueResult<PricingPolicy>() != null;
		}

		public IList<PricingPolicy> GetBySubject(PricingPolicy.SubjectType subject, bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId<>0 AND TargetObjectType=:ot";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.SetEnum("ot", subject)
				.List<PricingPolicy>();
		}

		public IList<PricingPolicy> GetBySubject(PricingPolicy.SubjectType subject, int subjectId, bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId=:oid AND TargetObjectType=:ot";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.SetEnum("ot", subject)
				.SetInt32("oid", subjectId)
				.List<PricingPolicy>();
		}

		public IList<PricingPolicy> GetBySubjectAndService(PricingPolicy.SubjectType subject, PricingPolicy.ServiceType service, bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId<>0 AND TargetObjectType=:ot AND Service=:srv";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.SetEnum("ot", subject)
				.SetEnum("srv", service)
				.List<PricingPolicy>();
		}

		public IList<PricingPolicy> GetByService(PricingPolicy.ServiceType service, bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId<>0 AND AND Service=:srv";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.SetEnum("srv", service)
				.List<PricingPolicy>();
		}

		public IList<PricingPolicy> GetNonDefaults(bool includeDeleted)
		{
			string q = @"FROM Vre.Server.BusinessLogic.PricingPolicy
WHERE TargetObjectId<>0";
			if (!includeDeleted) q += " AND Deleted=0";
			return _session.CreateQuery(q)
				.List<PricingPolicy>();
		}
	}
}