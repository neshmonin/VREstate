using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Vre.Server.RemoteService;

namespace Vre.Server.PaymentSystem
{
	internal class PayPalResponseHandler : IPaymentSystemResponseHandler
	{
		public string SystemName
		{
			get { return "paypal"; }
		}

		// Protocol explanation: https://developer.paypal.com/webapps/developer/docs/classic/ipn/integration-guide/IPNIntro

		private const string IoContentType = "application/x-www-form-urlencoded";
		private static readonly Encoding IoEncoding = Encoding.ASCII;
		private const string SandboxIoUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
		private const string LiveIoUrl = "https://www.paypal.com/cgi-bin/webscr";
		private const string ValidationCommand = "cmd=_notify-validate";

		public void ProcessResponse(IServiceRequest request, ResponseConsumer consumer)
		{
			var rawRequest = IoEncoding.GetString(request.Request.RawData);

			ServiceInstances.PaymentLogger.Debug("PayPal IPN Received: {0}", rawRequest);

			string validationResponse;
			var req = (HttpWebRequest)WebRequest.Create(Configuration.PaymentSystem.SandboxMode.Value 
				? SandboxIoUrl : LiveIoUrl);
			req.Method = "POST";
			req.ContentType = IoContentType;
			var validationRequest = ValidationCommand + "&" + rawRequest;
			req.ContentLength = validationRequest.Length;

			using (var streamOut = new StreamWriter(req.GetRequestStream(), IoEncoding))
				streamOut.Write(validationRequest);
			using (var streamIn = new StreamReader(req.GetResponse().GetResponseStream(), IoEncoding))
				validationResponse = streamIn.ReadToEnd();

			if ("VERIFIED".Equals(validationResponse))
			{
				//check that txn_id has not been previously processed
				//check that payment_amount/payment_currency are correct
				ServiceInstances.PaymentLogger.Info("PayPal IPN Validation: OK");

				var notificationParams = new ServiceQuery(HttpUtility.ParseQueryString(rawRequest, IoEncoding));

				// https://developer.paypal.com/webapps/developer/docs/classic/ipn/integration-guide/IPNandPDTVariables/
				validateRecipient(notificationParams);

				Currency pc;
				if (!Currency.TryParse(notificationParams["mc_currency"], out pc))
					throw new ArgumentException("Unknown or undefined transaction currency");

				decimal grossAmount, tax;
				if (!decimal.TryParse(notificationParams["mc_gross"], out grossAmount))
					throw new ArgumentException("Unknown or undefined transaction gross amount");
				if (!decimal.TryParse(notificationParams["tax"], out tax))
					throw new ArgumentException("Unknown or undefined transaction tax amount");

				var paymentStatus = notificationParams["payment_status"];
				var status = (!string.IsNullOrWhiteSpace(paymentStatus)
					&& paymentStatus.Equals("completed", StringComparison.InvariantCultureIgnoreCase));

				try
				{
					consumer(this, new PaymentResponse(
						status, status ? string.Empty : notificationParams["pending_reason"],
						notificationParams["receipt_id"], notificationParams["txn_id"],
						new Money(grossAmount - tax, pc), new Money(tax, pc),
						notificationParams["item_number"],
						notificationParams["memo"],
						rawRequest));

					// Echo back content again to confirm that our system processed notification.
					request.Response.DataStreamContentType = IoContentType;
					request.Response.DataStream.Write(request.Request.RawData, 0, request.Request.RawData.Length);
					request.Response.ResponseCode = HttpStatusCode.OK;
				}
				catch
				{
					// Nothing to do here? PayPal assumes no reponse as error?
					// TODO: Should rollback or credit user?

					throw;
				}
			}
			else if ("INVALID".Equals(validationResponse))
			{
				//log for manual investigation
				ServiceInstances.Logger.Error("PayPal IPN Validation FLAGGED: {0}",
					rawRequest);
			}
			else
			{
				//log response/ipn data for manual investigation
				ServiceInstances.Logger.Error("PayPal IPN Validation failed ({1}): {0}",
					rawRequest, validationResponse);
			}
		}

		private void validateRecipient(ServiceQuery notificationParams)
		{
			bool verified = false;

			var id = notificationParams["receiver_email"];
			if (!string.IsNullOrWhiteSpace(id))
			{
				if (id.Equals(Configuration.PaymentSystem.PayPal.MerchantId.Value)) verified = true;
				else throw new ArgumentException("Merchant ID is invalid");
			}

			id = notificationParams["business"];
			if (!string.IsNullOrWhiteSpace(id))
			{
				if (id.Equals(Configuration.PaymentSystem.PayPal.BusinessId.Value)) verified = true;
				else throw new ArgumentException("Business ID is invalid");
			}

			id = notificationParams["receiver_id"];
			if (!string.IsNullOrWhiteSpace(id))
			{
				if (id.Equals(Configuration.PaymentSystem.PayPal.AccountId.Value)) verified = true;
				else throw new ArgumentException("Merchant account ID is invalid");
			}

			if (!verified) throw new ArgumentException("Unable to verify payment recipient ID");
		}
	}
}