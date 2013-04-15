using System;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Command;
using Vre.Server.Dao;
using Vre.Server.RemoteService;

namespace Vre.Server.Task
{
    internal class NotifyExpiringViewOrders : BaseTask
    {
        public override string Name { get { return "NotifyExpiringViewOrders"; } }

        public override void Execute(Parameters param)
        {
            int ccnt = 0, ecnt = 0;
            string refVal = Utilities.GenerateReferenceNumber();
            bool testMode;
            //string salesAdminAddress = param.GetOption("salesAddress");            
            if (!bool.TryParse(param.GetOption("testMode"), out testMode)) testMode = false;
            //if (string.IsNullOrWhiteSpace(salesAdminAddress)) salesAdminAddress = "sales@3dcondox.com";

            bool error = true;
            do
            {
                try
                {
                    using (ISession session = NHibernateHelper.GetSession())
                    {
                        DatabaseSettingsDao.VerifyDatabase();

                        using (ViewOrderDao vodao = new ViewOrderDao(session))
                        {
                            foreach (ViewOrder vo in vodao.GetExpiringBeforeNotNotified(getcutOffTime(param), 0))
                            {
                                notifyViewOrderStatus(ref ccnt, ref ecnt, refVal, testMode,
                                    session, vo, "MSG_VIEWORDER_EXPIRING");
                            }  // view order loop

                            DateTime cutOff = DateTime.UtcNow;
                            foreach (ViewOrder vo in vodao.GetExpiringBeforeNotNotified(cutOff, -2))
                            {
                                if ((int)cutOff.Subtract(vo.ExpiresOn).TotalDays == vo.NotificationsSent)
                                {
                                    notifyViewOrderStatus(ref ccnt, ref ecnt, refVal, testMode,
                                        session, vo, "MSG_VIEWORDER_EXPIRED");
                                }
                            }  // view order loop

                            error = false;
                        }  // view order dao
                    }  // session
                }
                catch (HibernateException ex)
                {
                    ServiceInstances.Logger.Error("Database error: {0}", ex);
                    ecnt++;
                }
            } 
            while (error);

            if (ccnt > 0) ServiceInstances.Logger.Info("{0} notifications sent.", ccnt);

            if (ecnt > 0) 
                SendAdministrativeAlert("issues", 
                    string.Format(@"{0} view order notifications were not sent due to some issues.
Please see logs for more details.
Ref#{1}", ecnt, refVal));
        }

        private void notifyViewOrderStatus(ref int ccnt, ref int ecnt, string refVal, bool testMode,
            ISession session, ViewOrder vo, string messageTemplate)
        {
            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
            {
                string subject = GetSubjectAddress(session, vo);

                if (null == subject)
                {
                    ServiceInstances.Logger.Error("{0}: VOID={1} references unknown subject; no notification sent.",
                        refVal, vo.AutoID);
                    ecnt++;
                }
                else
                {
                    User owner;
                    using (UserDao udao = new UserDao(session))
                        owner = udao.GetById(vo.OwnerId);

                    if (string.IsNullOrWhiteSpace(owner.PrimaryEmailAddress))
                    {
                        ServiceInstances.Logger.Error("{0}: User {1} owns VOID={2} but has no valid email; no notification sent.",
                            refVal, owner, vo.AutoID);
                        ecnt++;
                    }
                    else
                    {
                        try
                        {
                            if (testMode)
                            {
                                string subj, body;
                                ServiceInstances.MessageGen.GetMessage(owner, messageTemplate,
                                    out subj, out body,
                                    // LEGACY: SERVER LOCAL TIME HERE!
                                    vo.AutoID, vo.ExpiresOn.ToLocalTime(), vo.Product, vo.Options, subject,
                                    ReverseRequestService.CreateViewOrderControlUrl(session, vo));
                                SendAdministrativeAlert(subj, body);
                            }
                            else
                            {
                                ServiceInstances.MessageGen.SendMessage(null, owner, messageTemplate,
                                    // LEGACY: SERVER LOCAL TIME HERE!
                                    vo.AutoID, vo.ExpiresOn.ToLocalTime(), vo.Product, vo.Options, subject,
                                    ReverseRequestService.CreateViewOrderControlUrl(session, vo));
                            }
                            ccnt++;
                        }
                        catch (Exception e)
                        {
                            ServiceInstances.Logger.Error("{0}: Failed sending message to user {1} RE VOID={2}: {3}",
                                refVal, owner, vo.AutoID, e);
                            ecnt++;
                        }
                    }
                }

                vo.NotificationsSent++;
                using (ViewOrderDao vodao = new ViewOrderDao(session)) vodao.Update(vo);

                tran.Commit();
            }  // transaction
        }

        public static string GetSubjectAddress(ISession session, ViewOrder vo)
        {
            string result = null;

            switch (vo.TargetObjectType)
            {
                case ViewOrder.SubjectType.Suite:
                    {
                        Suite suite;
                        using (SuiteDao sdao = new SuiteDao(session))
                            suite = sdao.GetById(vo.TargetObjectId);
                        if (suite != null)
                            result = AddressHelper.ConvertToReadableAddress(suite.Building, suite);
                    }
                    break;

                case ViewOrder.SubjectType.Building:
                    {
                        Building building;
                        using (BuildingDao bdao = new BuildingDao(session))
                            building = bdao.GetById(vo.TargetObjectId);
                        if (building != null)
                            result = AddressHelper.ConvertToReadableAddress(building, null);
                    }
                    break;
            }
            return result;
        }

        private DateTime getcutOffTime(Parameters param)
        {
            DateTime result;
            string sval;
            int ival;

            sval = param.GetOption("expiringInDays");
            if (!string.IsNullOrWhiteSpace(sval) && int.TryParse(sval, out ival))
            {
                result = DateTime.UtcNow.AddDays(ival);
            }
            else
            {
                ival = ServiceInstances.Configuration.GetValue("ViewOrderExpiryWarningDays", 3);
                result = DateTime.UtcNow.AddDays(ival);
            }

            return result;
        }
    }
}