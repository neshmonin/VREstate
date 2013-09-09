using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	public partial class FinancialTransaction : IClientDataProvider
	{
	    public enum TranSubject : byte
	    {
            ViewOrder = 0
	    }
        public enum TranTarget : byte
        {
            Suite = 0,
            Building = 1
        }
        public enum OperationType : byte
        {
            Debit = 0,
            Credit = 1
        }
        public enum AccountType : byte
        {
            User = 0
        }
        public enum PaymentSystemType : byte
        {
            Unknown = 0,
            CondoExplorer = 1,
            PayPal = 2
        }

        public int AutoID { get; private set; }
        public DateTime Created { get; private set; }

        /// <summary>
        /// System reference ID to be provided after transation is stored into system.  Cannot be altered once initialized.
        /// </summary>
        public string SystemRefId { get; private set; }

        /// <summary>
        /// User who initiated transaction.
        /// </summary>
        public int InitiatorId { get; private set; }

        /// <summary>
        /// Type of account this transaction aplies to.
        /// </summary>
        public AccountType Account { get; private set; }
        /// <summary>
        /// ID of account this transaction applies to (e.g. User record ID).
        /// </summary>
        public int AccountId { get; private set; }
        
        /// <summary>
        /// Transaction operation type.
        /// </summary>
        public OperationType Operation { get; private set; }
        /// <summary>
        /// Amount of CreditUnits involved in transaction.
        /// </summary>
        public decimal? CuAmount { get; private set; }
		/// <summary>
		/// Amount of currency involved in transaction (net value).
		/// </summary>
		public Money? CurrencyAmount { get; private set; }
		/// <summary>
		/// Amount of taxes applied to currency involved in transaction.
		/// </summary>
		public Money? CurrencyTax { get; private set; }
		/// <summary>
        /// Subject of transaction.
        /// </summary>
        public TranSubject Subject { get; private set; }

		protected virtual string currencyAmountDb
		{
			get { return (CurrencyAmount.HasValue ? CurrencyAmount.Value.ToFullString() : null); }
			set { Money m; if (Money.TryParse(value, out m)) CurrencyAmount = m; else CurrencyAmount = null; }
		}
		protected virtual string currencyTaxDb
		{
			get { return (CurrencyTax.HasValue ? CurrencyTax.Value.ToFullString() : null); }
			set { Money m; if (Money.TryParse(value, out m)) CurrencyTax = m; else CurrencyTax = null; }
		}

        /// <summary>
        /// Transaction target object.
        /// </summary>
        public TranTarget Target { get; private set; }
        /// <summary>
        /// Transaction target object ID (e.g. Suite record ID).
        /// </summary>
        public int TargetId { get; private set; }
        /// <summary>
        /// Extra information about target, e.g. listing/view ID or textual description.
        /// </summary>
        public string ExtraTargetInfo { get; private set; }

        /// <summary>
        /// Payment system type; cannot be altered once initialized.
        /// </summary>
        public PaymentSystemType PaymentSystem { get; private set; }
        /// <summary>
        /// Payment system reference id; cannot be altered once initialized.
        /// </summary>
        public string PaymentRefId { get; private set; }

        private FinancialTransaction() { }
        
		public FinancialTransaction(
            int initiatorId,
            AccountType account, int accountId,
            OperationType operation, decimal cuAmount,
            TranSubject subject, TranTarget target, int targetId, string extraTargetInfo)
		{
            AutoID = 0;
            Created = DateTime.UtcNow;

            InitiatorId = initiatorId;

            Account = account;
            AccountId = accountId;

            Operation = operation;
            CuAmount = cuAmount;
			CurrencyAmount = null;
			CurrencyTax = null;
            Subject = subject;

            Target = target;
            TargetId = targetId;
            ExtraTargetInfo = extraTargetInfo;

            SystemRefId = null;
            PaymentSystem = PaymentSystemType.Unknown;
            PaymentRefId = null;
        }

		public FinancialTransaction(
			int initiatorId,
			AccountType account, int accountId,
			OperationType operation, Money currencyAmount, Money currencyTax,
			TranSubject subject, TranTarget target, int targetId, string extraTargetInfo)
		{
			AutoID = 0;
			Created = DateTime.UtcNow;

			InitiatorId = initiatorId;

			Account = account;
			AccountId = accountId;

			Operation = operation;
			CuAmount = null;
			CurrencyAmount = currencyAmount;
			CurrencyTax = currencyTax;
			Subject = subject;

			Target = target;
			TargetId = targetId;
			ExtraTargetInfo = extraTargetInfo;

			SystemRefId = null;
			PaymentSystem = PaymentSystemType.Unknown;
			PaymentRefId = null;
		}

		public void SetSystemReferenceId(string refId)
        {
            if (SystemRefId != null) throw new InvalidOperationException("Cannot alter value");
            SystemRefId = refId;
        }

        public void SetPaymentSystemReference(PaymentSystemType type, string refId)
        {
            if (PaymentRefId != null) throw new InvalidOperationException("Cannot alter value");
            PaymentSystem = type;
            PaymentRefId = refId;
        }

        public FinancialTransaction(ClientData data)
        {
            AutoID = -1;
            Created = data.GetProperty("created", DateTime.MinValue);

            InitiatorId = data.GetProperty("initiatorId", -1);

            SystemRefId = data.GetProperty("systemRefId", string.Empty);

            PaymentSystem = data.GetProperty<PaymentSystemType>("paymentSystem", PaymentSystemType.Unknown);
            PaymentRefId = data.GetProperty("paymentRefId", string.Empty);

            Account = data.GetProperty<AccountType>("account", AccountType.User);
            AccountId = data.GetProperty("accountId", -1);

            Operation = data.GetProperty<OperationType>("operation", OperationType.Credit);
            CuAmount = data.GetProperty("amount", 0m);
			string c; Money m;
			c = data.GetProperty("cuAmount", string.Empty);
			if (c.Length > 0) { if (Money.TryParse(c, out m)) CurrencyAmount = m; }
			c = data.GetProperty("cuTax", string.Empty);
			if (c.Length > 0) { if (Money.TryParse(c, out m)) CurrencyTax = m; }
			Subject = data.GetProperty<TranSubject>("subject", TranSubject.ViewOrder);

            Target = data.GetProperty<TranTarget>("target", TranTarget.Suite);
            TargetId = data.GetProperty("targetId", -1);
            ExtraTargetInfo = data.GetProperty("extraTargetInfo", string.Empty);
        }

        public ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("created", Created);

            if (SystemRefId != null) result.Add("systemRefId", SystemRefId);

            if (PaymentRefId != null)
            {
                result.Add("paymentSystem", ClientData.ConvertProperty<PaymentSystemType>(PaymentSystem));
                result.Add("paymentRefId", PaymentRefId);
            }

            result.Add("initiatorId", InitiatorId);

            result.Add("account", ClientData.ConvertProperty<AccountType>(Account));
            result.Add("accountId", AccountId);

            result.Add("operation", ClientData.ConvertProperty<OperationType>(Operation));
            if (CuAmount.HasValue) result.Add("amount", CuAmount.Value);
			if (CurrencyAmount.HasValue) result.Add("cuAmount", CurrencyAmount.Value.ToFullString());
			if (CurrencyTax.HasValue) result.Add("cuTax", CurrencyTax.Value.ToFullString());
			result.Add("subject", ClientData.ConvertProperty<TranSubject>(Subject));

            result.Add("target", ClientData.ConvertProperty<TranTarget>(Target));
            result.Add("targetId", TargetId);
            result.Add("extraTargetInfo", ExtraTargetInfo);

            return result;
        }

        public bool UpdateFromClient(ClientData data)
        {
            throw new NotImplementedException();
        }

        //public override string ToString()
        //{
        //    if (EstateDeveloperID != null)
        //        return string.Format("ID={0},r={1},ED={2}", AutoID, UserRole, EstateDeveloperID);
        //    else
        //        return string.Format("ID={0},r={1}", AutoID, UserRole);
        //}
    }
}
