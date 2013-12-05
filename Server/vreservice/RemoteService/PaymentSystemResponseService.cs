using System;
using System.Collections.Generic;
using System.Linq;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.PaymentSystem;
using Vre.Server.Task;

namespace Vre.Server.RemoteService
{
	internal class PaymentSystemResponseService : IHttpService
	{
		private static bool _configured = false;

		private static Dictionary<string, IPaymentSystemResponseHandler> _handlers = null;

		private const string _servicePathPrefix = _servicePathElement0;
		private const string _servicePathElement0 = "ps";

		private static void configure()
		{
			// TODO: Ensure lazy initialization is thread-safe!!!

			_handlers = new Dictionary<string, IPaymentSystemResponseHandler>();
			addHandler(new PayPalResponseHandler());

			_configured = true;
		}

		private static void addHandler(IPaymentSystemResponseHandler handler)
		{
			_handlers.Add(handler.SystemName, handler);
		}

		private static void sendFailureMessage(NHibernate.ISession dbSession, 
			ViewOrder product, User owner,
			string systemRefNo, Money? totalAmount,
			string errorText)
		{
			var param = new object[6];

			if (product != null)
			{
				param[0] = product.Product;
				param[1] = product.Options;
				param[2] = (dbSession != null) ? NotifyExpiringViewOrders.GetSubjectAddress(dbSession, product) : "N/A";
			}
			else
			{
				param[0] = "@N/A";
				param[1] = "@N/A";
				param[2] = "@N/A";
			}

			param[3] = systemRefNo ?? "@N/A";
			param[4] = totalAmount.HasValue ? totalAmount.Value.ToFullString() : "@N/A";
			param[5] = errorText;

			ServiceInstances.MessageGen.SendMessage(
				ServiceInstances.EmailSender, owner.PrimaryEmailAddress,
				"MSG_PAYMENT_FAILURE", param);
		}

		private static void sendSalesFailureMessage(NHibernate.ISession dbSession, 
			ViewOrder product, User owner,
			string systemName, string systemRefNo, Money? totalAmount,
			string errorText)
		{
			var param = new object[11];

			if (product != null)
			{
				param[0] = product.Product;
				param[1] = product.Options;
				param[2] = (dbSession != null) ? NotifyExpiringViewOrders.GetSubjectAddress(dbSession, product) : "N/A";
			}
			else
			{
				param[0] = "@N/A";
				param[1] = "@N/A";
				param[2] = "@N/A";
			}

			if (owner != null)
			{
				param[3] = owner.UserRole;
				param[4] = owner.NickName;
				param[5] = owner.PrimaryEmailAddress;
				param[6] = "@is";
			}
			else
			{
				param[3] = "@N/A";
				param[4] = "@N/A";
				param[5] = "@N/A";
				param[6] = "@IS NOT";
			}

			param[7] = systemName ?? "@N/A";
			param[8] = systemRefNo ?? "@N/A";
			param[9] = totalAmount.HasValue ? totalAmount.Value.ToFullString() : "@N/A";
			param[10] = errorText;

			ServiceInstances.MessageGen.SendMessage(
				ServiceInstances.EmailSender, Configuration.Messaging.SalesMessageRecipients.Value,
				"MSG_SALES_FAILED_PAYMENT", param);
		}

		private static void sendSuccessMessage(NHibernate.ISession dbSession, 
			ViewOrder product, User owner,
			string paymentRefNo, string systemRefNo, Money? totalAmount,
			string displayUrl, string controlUrl,
			string mlsId, string moreInfoUrl, string vTourUrl)
		{
			var param = new object[14];

			if (product != null)
			{
				param[0] = product.Product;
				param[1] = product.Options;
				param[2] = (dbSession != null) ? NotifyExpiringViewOrders.GetSubjectAddress(dbSession, product) : "N/A";
				param[3] = product.ExpiresOn;
			}
			else
			{
				param[0] = "@N/A";
				param[1] = "@N/A";
				param[2] = "@N/A";
				param[3] = "@N/A";
			}

			param[4] = paymentRefNo ?? "@N/A";
			param[5] = systemRefNo ?? "@N/A";
			param[6] = totalAmount.HasValue ? totalAmount.Value.ToFullString() : "@N/A";

			param[7] = displayUrl ?? "@N/A";
			param[8] = controlUrl ?? "@N/A";

			param[9] = mlsId ?? "@N/A";
			param[10] = moreInfoUrl ?? string.Empty;
			param[11] = moreInfoUrl ?? "@N/A";
			param[12] = vTourUrl ?? string.Empty;
			param[13] = vTourUrl ?? "@N/A";

			ServiceInstances.MessageGen.SendMessage(
				ServiceInstances.EmailSender, owner.PrimaryEmailAddress,
				"MSG_PAYMENT_SUCCESS", param);
		}

		private void ConsumeResponse(IPaymentSystemResponseHandler handler, PaymentResponse response)
		{
			if (response.Succeed)
				ServiceInstances.PaymentLogger.Info(
					"{0} posted a notification on processed payment (request {1}) for {2} plus taxes ({3}); system ref#: {4}; payer memo: '{5}'",
					handler.SystemName, response.ProductId,
					response.NetAmount, response.Taxes,
					response.SystemReferenceNumber,
					response.PayerMemo);
			else
				ServiceInstances.PaymentLogger.Warn(
					"{0} posted a notification on unprocessed payment (request {1}) for {2} plus taxes ({3}); system ref#: {4}; payer memo: '{5}'",
					handler.SystemName, response.ProductId,
					response.NetAmount, response.Taxes,
					response.SystemReferenceNumber,
					response.PayerMemo);

			if (UniversalId.TypeInUrlId(response.ProductId) != UniversalId.IdType.ReverseRequest)
				throw new ArgumentException("Product ID is not valid.");

			using (var dbSession = NHibernateHelper.GetSession())
			{
				using (var tran = NHibernateHelper.OpenNonNestedTransaction(dbSession))
				{
					ViewOrder voProduct = null;
					User productOwner = null;

					ReverseRequest rr = null;
					using (var dao = new ReverseRequestDao(dbSession))
						rr = dao.GetById(UniversalId.ExtractAsGuid(response.ProductId));
					if (null != rr)
					{
						if (rr.Request != ReverseRequest.RequestType.ViewOrderActivation)
						{
							ServiceInstances.PaymentLogger.Error("Invalid Reverse Request type: {0}", rr.Request);
							ServiceInstances.Logger.Error("Invalid Reverse Request type: {0}", rr.Request);
							throw new ArgumentException("Product ID is not valid.");
						}

						using (var dao = new ViewOrderDao(dbSession))
							voProduct = dao.GetById(new Guid(rr.Subject));

						if (voProduct != null)
							using (var dao = new UserDao(dbSession))
								productOwner = dao.GetById(voProduct.OwnerId);

						if (response.Succeed)
						{
							processPayment(handler, response, dbSession, rr, voProduct, productOwner);
						}
						else
						{
							if ((voProduct != null) && (productOwner != null))
							{
								sendFailureMessage(dbSession, voProduct, productOwner,
									response.SystemReferenceNumber, response.NetAmount + response.Taxes,
									"Payment system responded with failure: probably transaction was rejected.");
								sendSalesFailureMessage(dbSession,
									voProduct, productOwner,
									handler.SystemName,
									response.SystemReferenceNumber, response.NetAmount + response.Taxes,
									"Payment system responded with failure: probably transaction was rejected.");

								ServiceInstances.Logger.Error(
									"Payment system {0} (product={1}, payment reference={2}) response: {3}; payment not processed; message sent.",
									handler.SystemName,
									response.ProductId, response.SystemReferenceNumber,
									response.StatusDescription);
							}
							else
							{
								sendSalesFailureMessage(dbSession,
									voProduct, productOwner,
									handler.SystemName,
									response.SystemReferenceNumber, response.NetAmount + response.Taxes,
									"Payment system responded with failure: probably transaction was rejected.");

								ServiceInstances.Logger.Error(
									"Payment system {0} (product={1}, payment reference={2}) response: {3}; payment not processed; message NOT sent.",
									handler.SystemName,
									response.ProductId, response.SystemReferenceNumber,
									response.StatusDescription);
							}
						}

						tran.Commit();
					}  // reverse request found
					else
					{
						ServiceInstances.PaymentLogger.Warn("Reverse request not found or duplicate processing detected");
					}
				}
			}
		}

		private static void processPayment(IPaymentSystemResponseHandler handler, PaymentResponse response,
			NHibernate.ISession dbSession, 
			ReverseRequest request, ViewOrder voProduct, User productOwner)
		{
			var ftSt = FinancialTransactionExt.SystemTypeByName(handler.SystemName);

			// Test for repeated call from payment system
			using (var dao = new FinancialTransactionDao(dbSession))
			{
				if (dao.Get(ftSt, response.SystemReferenceNumber).Count() > 0)
				{
					ServiceInstances.PaymentLogger.Warn("Duplicate processing detected");
					return;
				}
			}

			//check that NetAmount is correct
			Money requestedAmount;
			if (!Money.TryParse(request.ReferenceParamValue, out requestedAmount))
				throw new ArgumentException("Internal error #0");
			if (!response.NetAmount.Equals(requestedAmount))
			{
				var errorText = string.Format("Requested amount is {0}; net paid amount is {1}",
					requestedAmount, response.NetAmount);

				ServiceInstances.PaymentLogger.Error(errorText);

				// TODO: WHAT TO DO?! Credit user and delete everything?!
				sendSalesFailureMessage(dbSession, voProduct, productOwner,
					handler.SystemName, response.SystemReferenceNumber, response.NetAmount + response.Taxes,
					errorText);
				
				throw new InvalidOperationException("Amount received is not correct");
			}
			
			// Update ViewOrder expiry time
			voProduct.Prolong(new DateTime(long.Parse(request.ReferenceParamName)));
			dbSession.Update(voProduct);

			// Update reverse request
			request.ReferenceParamValue = string.Empty;
			request.ProlongBy(new TimeSpan(1, 0, 0));  // make sure it exists for a while to tell customer "everything is OK"
			dbSession.Update(request);

			// Create Financial Transaction
			var ft = new FinancialTransaction(productOwner.AutoID,
				FinancialTransaction.AccountType.User, -1,
				FinancialTransaction.OperationType.Debit,
				response.NetAmount, response.Taxes,
				FinancialTransaction.TranSubject.ViewOrder,
				FinancialTransactionExt.TargetByViewOrderTarget(voProduct.TargetObjectType),
				voProduct.TargetObjectId, string.Empty);
			dbSession.Save(ft);
			ft.SetAutoSystemReferenceId();
			ft.SetPaymentSystemReference(handler.SystemName, response.SystemReferenceNumber);
			dbSession.Update(ft);

			var logLine = string.Format(
				"ViewOrder is successfully paid and activated: VOID={0}, Expires: {1}, STRN={2}, PSRID={3}, PSTXID={4}, Net={5}, Tax={6}",
				voProduct.AutoID, voProduct.ExpiresOn, ft.SystemRefId,
				response.SystemReferenceNumber, response.SystemTransactionId,
				response.NetAmount.ToFullString(), response.Taxes.ToFullString());
			ServiceInstances.Logger.Info(logLine);
			ServiceInstances.PaymentLogger.Info(logLine);

			sendSuccessMessage(dbSession, voProduct, productOwner,
				ft.SystemRefId, 
				response.SystemReferenceNumber, response.NetAmount + response.Taxes,
				ReverseRequestService.GenerateUrl(voProduct),
				ReverseRequestService.CreateViewOrderControlUrl(dbSession, voProduct),
				voProduct.MlsId, voProduct.InfoUrl, voProduct.VTourUrl);
		}

		#region IHttpService implementaion
		public string ServicePathPrefix { get { return _servicePathPrefix; } }

		public bool RequiresSession { get { return false; } }

		public void ProcessCreateRequest(IServiceRequest request)
		{
			if (!_configured) configure();

			if (!Configuration.Security.AllowSensitiveDataOverNonSecureConnection.Value
				&& !request.Request.IsSecureConnection)
				throw new PermissionException("Service available only over secure connection.");

			if (request.Request.PathSegments.Length < 2)
				throw new ArgumentException("System type must be specified.");

			string systemType = request.Request.PathSegments[1];

			IPaymentSystemResponseHandler handler;
			if (!_handlers.TryGetValue(systemType, out handler))
				throw new ArgumentException("Unknown system type specified.");

			try
			{
				handler.ProcessResponse(request, ConsumeResponse);
			}
			catch (Exception ex)
			{
				ServiceInstances.PaymentLogger.Error(
					"{0} notification processing error: {1}",
					handler.SystemName, ex);

				sendSalesFailureMessage(null, null, null,
					handler.SystemName, null, null,
					"Generic processing error: " + ex.Message);

				throw;
			}
		}

		public void ProcessGetRequest(IServiceRequest request)
		{
			throw new System.NotImplementedException();
		}

		public void ProcessReplaceRequest(IServiceRequest request)
		{
			throw new System.NotImplementedException();
		}

		public void ProcessDeleteRequest(IServiceRequest request)
		{
			throw new System.NotImplementedException();
		}
		#endregion
	}
}