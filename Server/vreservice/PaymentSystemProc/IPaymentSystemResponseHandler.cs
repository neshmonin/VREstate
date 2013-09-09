using Vre.Server.RemoteService;
using System;

namespace Vre.Server.PaymentSystem
{
	internal class PaymentResponse
	{
		public PaymentResponse(bool succeed, string statusDescription,
			string systemReferenceNumber, string tranId, 
			Money netAmount, Money taxes,
			string productId, string payerMemo,
			string rawData)
		{
			Succeed = succeed;
			StatusDescription = statusDescription;
			SystemReferenceNumber = systemReferenceNumber;
			SystemTransactionId = tranId;
			NetAmount = netAmount;
			Taxes = taxes;
			ProductId = productId;
			PayerMemo = payerMemo;
			RawData = rawData;
		}

		/// <summary>
		/// Response status
		/// </summary>
		public bool Succeed { get; private set; }
		/// <summary>
		/// Negative status description
		/// </summary>
		public string StatusDescription { get; private set; }
		/// <summary>
		/// Visible and user-trackable reference number AKA receipt ID
		/// </summary>
		public string SystemReferenceNumber { get; private set; }
		/// <summary>
		/// System transaction ID
		/// </summary>
		public string SystemTransactionId { get; private set; }
		public Money NetAmount { get; private set; }
		public Money Taxes { get; private set; }
		public string ProductId { get; private set; }
		public string PayerMemo { get; private set; }
		public string RawData { get; private set; }
	}

	internal interface IPaymentSystemResponseHandler
	{
		string SystemName { get; }
		void ProcessResponse(IServiceRequest request, ResponseConsumer consumer);
	}

	internal delegate void ResponseConsumer(IPaymentSystemResponseHandler handler, PaymentResponse response);
}