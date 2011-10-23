using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Transaction 
	{
	    public enum TransactionStatus : byte
	    {
            OrderPlaced = 0,
            MoneySubmitted = 1,
            MoneyConfirmed = 2,
            MoneyComitted = 3,
            Finished = 4,
            Error = 10,
            Cancelled = 20
	    }

        public int AutoID { get; private set; }
        public int AccountID { get; private set; }

        public float Amount { get; set; }
        public DateTime DealDateTime { get; set; }
        public DateTime PaidDateTime { get; set; }
        public TransactionStatus Status { get; set; }

        public IList<Option> Options { get; private set; }

        private Transaction() { }

        public Transaction(int accountId)
        {
            AccountID = accountId;
            Amount = 0.0f;
            Status = TransactionStatus.OrderPlaced;
        }
	}
}
