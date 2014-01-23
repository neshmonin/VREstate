using System;
using System.Collections.Generic;
using NHibernate;
using Vre.Server.Accounting;
using Vre.Server.BusinessLogic;
using Vre.Server.Command;
using Vre.Server.Dao;

namespace Vre.Server.Task
{
    internal class GenerateInvoices : BaseTask
    {
        public override string Name { get { return "GenerateInvoices"; } }

        public override void Execute(Parameters param)
        {
            using (ISession session = NHibernateHelper.GetSession())
            {
                DatabaseSettingsDao.VerifyDatabase();

                IEnumerable<BrokerageInfo> brokerages;
                using (var dao = new BrokerageInfoDao(session)) brokerages = dao.GetAll();
                
                foreach (var b in brokerages)
                {
                    Accounting.Invoice invoice;
                    BusinessLogic.Invoice dbInvoice;
                    Money billingAmount = Money.Zero;
                    using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
                    {
                        session.Refresh(b, LockMode.Write);

                        invoice = Biller.CalculateCurrentForBrokerage(session, b);

                        dbInvoice = new BusinessLogic.Invoice(
                            BusinessLogic.Invoice.SubjectType.Brokerage, b.AutoID,
                            invoice.GetClientData());
                        session.Save(dbInvoice);

                        b.Debit(invoice.TotalInCreditUnits);
                        b.LastServicePayment = invoice.EndTime;
                        if (b.CreditUnits < 0m)
                        {
                            billingAmount = new Money(invoice.TargetCurrencyRate * (-b.CreditUnits), invoice.TargetCurrency);
                            generatePaymentSystemRequest(session, b, dbInvoice.AutoID, billingAmount);
                        }

                        session.Update(b);

                        tran.Commit();
                    }

                    if (billingAmount.ToDecimal(null) > 0m)
                        ServiceInstances.Logger.Info("Processed billing for Brokerage '{0}': invoice ID={1}, total (CU) {2}, billed for {3}",
                            b, dbInvoice.AutoID, invoice.TotalInCreditUnits, billingAmount.ToFullString());
                    else
                        ServiceInstances.Logger.Info("Processed billing for Brokerage '{0}': invoice ID={1}, total (CU) {2}, remaining on account (CU) {3}",
                            b, dbInvoice.AutoID, invoice.TotalInCreditUnits, b.CreditUnits);

                    // TODO: Generate brokerage invoice?

                    IEnumerable<User> agents;
                    using (var dao = new UserDao(session)) agents = dao.GetForBrokerage(b, User.Role.Agent);

                    foreach (var a in agents)
                    {
                        billingAmount = Money.Zero;

                        using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
                        {
                            session.Refresh(a, LockMode.Write);

                            invoice = Biller.CalculateCurrentForAgent(session, a);

                            dbInvoice = new BusinessLogic.Invoice(
                                BusinessLogic.Invoice.SubjectType.Agent, a.AutoID,
                                invoice.GetClientData());
                            session.Save(dbInvoice);

                            a.Debit(invoice.TotalInCreditUnits);
                            a.LastServicePayment = invoice.EndTime;
                            if (a.CreditUnits < 0m)
                            {
                                billingAmount = new Money(invoice.TargetCurrencyRate * (-a.CreditUnits), invoice.TargetCurrency);
                                generatePaymentSystemRequest(session, a, dbInvoice.AutoID, billingAmount);
                            }

                            session.Update(a);

                            tran.Commit();
                        }

                        if (billingAmount.ToDecimal(null) > 0m)
                            ServiceInstances.Logger.Info("Processed billing for Agent '{0}': invoice ID={1}, total (CU) {2}, billed for {3}",
                                a, dbInvoice.AutoID, invoice.TotalInCreditUnits, billingAmount.ToFullString());
                        else
                            ServiceInstances.Logger.Info("Processed billing for Agent '{0}': invoice ID={1}, total (CU) {2}, remaining on account (CU) {3}",
                                a, dbInvoice.AutoID, invoice.TotalInCreditUnits, a.CreditUnits);

                        // TODO: Generate agent invoice?
                    }
                }

                // TODO: Process individual (not belonging to brokerage) agents
            }
        }

        private void generatePaymentSystemRequest(ISession session, BrokerageInfo @object, int invoiceId, Money amount)
        {
            List<string> recipients = new List<string>();

            using (var dao = new UserDao(session))
                foreach (var ba in dao.ListUsers(User.Role.BrokerageAdmin, null, @object.AutoID, null, false))
                    recipients.Add(ba.PrimaryEmailAddress);

            if (0 == recipients.Count) recipients.AddRange(@object.EmailList);

            generatePaymentSystemRequest(recipients, amount);

            var ft = new FinancialTransaction(0, FinancialTransaction.AccountType.Brokerage, @object.AutoID, 
                FinancialTransaction.OperationType.Debit, amount, 
                FinancialTransaction.TranSubject.Service, FinancialTransaction.TranTarget.Invoice, invoiceId, 
                "Brokerage invoice generated.");

            session.Save(ft);
        }

        private void generatePaymentSystemRequest(ISession session, User @object, int invoiceId, Money amount)
        {
            generatePaymentSystemRequest(new [] { @object.PrimaryEmailAddress }, amount);

            var ft = new FinancialTransaction(0, FinancialTransaction.AccountType.User, @object.AutoID,
                FinancialTransaction.OperationType.Debit, amount,
                FinancialTransaction.TranSubject.Service, FinancialTransaction.TranTarget.Invoice, invoiceId,
                "User (agent) invoice generated.");

            session.Save(ft);
        }

        private void generatePaymentSystemRequest(IEnumerable<string> recipients, Money amount)
        {
            // TODO: PayPal reverse request!
            ServiceInstances.Logger.Debug("DEBUG BILL {0} TO {1}", amount.ToFullString(), CsvUtilities.ToCsv(recipients));
        }
    }
}