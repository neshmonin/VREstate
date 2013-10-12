using System;

namespace Vre.Server.BusinessLogic
{
    internal static class FinancialTransactionExt
    {
        private const string FinancialTransactionRefNumPrefix = "3DCX";
        private const int FinancialTransactionRefNumKey = 0x752EB3A6;

        public static string GenerateRefNum(this FinancialTransaction ft)
        {
            return string.Format("{0}{1}",
                FinancialTransactionRefNumPrefix,
                Utilities.GenerateReferenceNumber(ft.AutoID ^ FinancialTransactionRefNumKey));
        }

        public static void SetAutoSystemReferenceId(this FinancialTransaction ft)
        {
            ft.SetSystemReferenceId(string.Format("{0}{1}",
                FinancialTransactionRefNumPrefix,
                Utilities.GenerateReferenceNumber(ft.AutoID ^ FinancialTransactionRefNumKey)));
        }

		public static void SetPaymentSystemReference(this FinancialTransaction ft,
			string paymentSystemName, string referenceId)
		{
			ft.SetPaymentSystemReference(SystemTypeByName(paymentSystemName), referenceId);
		}

		public static FinancialTransaction.PaymentSystemType SystemTypeByName(string name)
		{
			var n = name.Trim().ToUpper();
			if (n.Equals("PAYPAL")) return FinancialTransaction.PaymentSystemType.PayPal;
			else if (n.Equals("CX")) return FinancialTransaction.PaymentSystemType.CondoExplorer;
			else return FinancialTransaction.PaymentSystemType.Unknown;
		}

		public static FinancialTransaction.TranTarget TargetByViewOrderTarget(ViewOrder.SubjectType type)
		{
			switch (type)
			{
				case ViewOrder.SubjectType.Building:
					return FinancialTransaction.TranTarget.Building;

				case ViewOrder.SubjectType.Suite:
					return FinancialTransaction.TranTarget.Suite;

				default:
					throw new ArgumentException("Unknown ViewOrder Target Type (" + type.ToString() + ")");
			}
		}
    }
}