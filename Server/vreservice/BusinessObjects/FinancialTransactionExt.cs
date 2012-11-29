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
    }
}