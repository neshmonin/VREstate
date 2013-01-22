using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CoreClasses;
using Vre.Server.BusinessLogic;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace SuperAdminConsole
{
    public class Helpers
    {
        static public string LabelFromViewOrder(string viewOrderId, out ViewOrder theOrder, out User theUser)
        {
            theOrder = null;
            theUser = null;
            viewOrderId = viewOrderId.Replace("-", string.Empty);
            ServerResponse orderResp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "viewOrder/" + viewOrderId,
                                                              "verbose=true", null);
            //id = 20a6b755-58b2-4616-89de-92faf1ed6814
            //version = Vre.Server.BusinessLogic.ClientData[]
            //ownerId = 8
            //expiresOn = 01/11/2012 12:00:00 AM
            //enabled = True
            //product = 0
            //mlsId = 
            //productUrl = 
            //targetObjectType = 0
            //targetObjectId = 8516
            //requestCounter = 0
            //lastRequestTime = 12/10/2012 9:26:40 AM
            if (HttpStatusCode.OK != orderResp.ResponseCode)
                return null;

            theOrder = new ViewOrder(orderResp.Data);
            theOrder.MlsId = orderResp.Data.GetProperty("mlsId", string.Empty);
            string label = orderResp.Data.GetProperty("label", string.Empty);
            //if (label != string.Empty)
            //    return label;

            ServerResponse userResp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "user/" + orderResp.Data["ownerId"],
                                                              "", null);
            if (HttpStatusCode.OK != userResp.ResponseCode)
                return null;

            theUser = new User(userResp.Data);
            if (label != string.Empty)
                return label;

            int buildingId = 0;
            string theLabel = string.Empty;
            if (theOrder.TargetObjectType == ViewOrder.SubjectType.Suite)
            {
                ServerResponse suiteResp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                  "suite/" + theOrder.TargetObjectId,
                                                                  "", null);
                if (HttpStatusCode.OK != suiteResp.ResponseCode)
                    return null;
                Vre.Server.BusinessLogic.Suite suite = new Vre.Server.BusinessLogic.Suite(suiteResp.Data);
                theLabel += suite.SuiteName + " - ";
                buildingId = (int)suiteResp.Data["buildingId"];
            }
            else
                if (theOrder.TargetObjectType == ViewOrder.SubjectType.Building)
                {
                    buildingId = theOrder.TargetObjectId;
                }

            ServerResponse buildingResp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                "building/" + buildingId,
                                                                "", null);

            if (HttpStatusCode.OK != buildingResp.ResponseCode)
                return null;

            Vre.Server.BusinessLogic.Building building = new Vre.Server.BusinessLogic.Building(buildingResp.Data);
            theLabel += building.AddressLine1 + ", ";
            if (!string.IsNullOrEmpty(building.AddressLine2)) theLabel += building.AddressLine2 + ", ";
            theLabel += building.City + ", ";
            theLabel += building.StateProvince + "  " + building.PostalCode + ", ";
            theLabel += building.Country;

            return theLabel;
        }

        public class TransactionInfo
        {
            public bool Succeed { private set; get; }
            public string Error { private set; get; }

            public string CorrelationId { private set; get; }
            public string APIResult { private set; get; }
            public string PaymentReceiver { private set; get; }
            public string Payer { private set; get; }
            public string PaymentDate { private set; get; }
            public string PaymentStatus { private set; get; }
            public decimal GrossAmount { private set; get; }
            public string Currency { private set; get; }
            public decimal SettlementAmount { private set; get; }

            public TransactionInfo(GetTransactionDetailsResponseType response)
            {
                SettlementAmount = 0.0M;
                GrossAmount = 0.0M;
                CorrelationId = response.CorrelationID;
                APIResult = response.Ack.ToString();

                if (response.Ack.Equals(AckCodeType.FAILURE) ||
                    (response.Errors != null && response.Errors.Count > 0))
                {
                    Succeed = false;
                    foreach (var error in response.Errors)
                        Error += error.ToString() + "\n";
                }
                else
                {
                    Succeed = true;
                    PaymentTransactionType transactionDetails = response.PaymentTransactionDetails;
                    PaymentReceiver = transactionDetails.ReceiverInfo.Receiver;
                    Payer = transactionDetails.PayerInfo.Payer;
                    PaymentDate = transactionDetails.PaymentInfo.PaymentDate;
                    PaymentStatus = transactionDetails.PaymentInfo.PaymentStatus.ToString();
                    Currency = transactionDetails.PaymentInfo.GrossAmount.currencyID.ToString();
                    GrossAmount = decimal.Parse(transactionDetails.PaymentInfo.GrossAmount.value);

                    if (transactionDetails.PaymentInfo.SettleAmount != null)
                        SettlementAmount = decimal.Parse(transactionDetails.PaymentInfo.SettleAmount.value);
                }
            }

            public string Formatted
            {
                get
                {
                    string ret = string.Format("Payment Status: {0}\n" +
                                               "Payment Date: {1}\n" +
                                               "Payer: {2}\n" +
                                               "Payment Receiver: {3}\n" +
                                               "Gross Amount: {4}{5}\n" +
                                               "---------------------\n" +
                                               "APIResult: {6}\n" +
                                               "Correlation Id: {7}\n",
                                               PaymentStatus,
                                               PaymentDate,
                                               Payer == null ? "<no data>" : Payer,
                                               PaymentReceiver,
                                               GrossAmount.ToString() + Currency,
                                               SettlementAmount != 0.0M ? "\nSettlement Amount: " + SettlementAmount + Currency : "",
                                               APIResult,
                                               CorrelationId);
                    return ret;
                }
            }
        }

        static public TransactionInfo TransactionDetails(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                return null;

            // Create request object
            GetTransactionDetailsRequestType request = new GetTransactionDetailsRequestType();
            request.TransactionID = transactionId;

            // Invoke the API
            GetTransactionDetailsReq wrapper = new GetTransactionDetailsReq();
            wrapper.GetTransactionDetailsRequest = request;
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService();
            GetTransactionDetailsResponseType response = service.GetTransactionDetails(wrapper);
            if (response.Errors != null &&
                response.Errors.Count > 0 &&
                response.Errors[0].ErrorCode == "10004")
                return null;

            TransactionInfo info = new TransactionInfo(response);

            return info;
        }
    }
}
