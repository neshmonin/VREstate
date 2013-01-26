using System.Xml;
using PayPal;
using PayPal.Authentication;
using PayPal.Util;
using PayPal.Manager;
using PayPal.PayPalAPIInterfaceService.Model;

namespace PayPal.PayPalAPIInterfaceService {
	public partial class PayPalAPIInterfaceServiceService : BasePayPalService {

		// Service Version
		private static string ServiceVersion = "94.0";

		// Service Name
		private static string ServiceName = "PayPalAPIInterfaceService";

		public PayPalAPIInterfaceServiceService() : base(ServiceName, ServiceVersion)
		{
		}
	
		private void setStandardParams(AbstractRequestType request) {
			if (request.Version == null)
			{
				request.Version = ServiceVersion;
			}
			if (request.ErrorLanguage != null && ConfigManager.Instance.GetProperty("languageCode") != null)
			{
				request.ErrorLanguage = ConfigManager.Instance.GetProperty("languageCode");
			}
		}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public RefundTransactionResponseType RefundTransaction(RefundTransactionReq refundTransactionReq, string apiUserName)
	 	{
			setStandardParams(refundTransactionReq.RefundTransactionRequest);
			string response = Call("RefundTransaction", refundTransactionReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='RefundTransactionResponse']");
			return new RefundTransactionResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public RefundTransactionResponseType RefundTransaction(RefundTransactionReq refundTransactionReq)
	 	{
	 		return RefundTransaction(refundTransactionReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public InitiateRecoupResponseType InitiateRecoup(InitiateRecoupReq initiateRecoupReq, string apiUserName)
	 	{
			setStandardParams(initiateRecoupReq.InitiateRecoupRequest);
			string response = Call("InitiateRecoup", initiateRecoupReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='InitiateRecoupResponse']");
			return new InitiateRecoupResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public InitiateRecoupResponseType InitiateRecoup(InitiateRecoupReq initiateRecoupReq)
	 	{
	 		return InitiateRecoup(initiateRecoupReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CompleteRecoupResponseType CompleteRecoup(CompleteRecoupReq completeRecoupReq, string apiUserName)
	 	{
			setStandardParams(completeRecoupReq.CompleteRecoupRequest);
			string response = Call("CompleteRecoup", completeRecoupReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='CompleteRecoupResponse']");
			return new CompleteRecoupResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CompleteRecoupResponseType CompleteRecoup(CompleteRecoupReq completeRecoupReq)
	 	{
	 		return CompleteRecoup(completeRecoupReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CancelRecoupResponseType CancelRecoup(CancelRecoupReq cancelRecoupReq, string apiUserName)
	 	{
			setStandardParams(cancelRecoupReq.CancelRecoupRequest);
			string response = Call("CancelRecoup", cancelRecoupReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='CancelRecoupResponse']");
			return new CancelRecoupResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CancelRecoupResponseType CancelRecoup(CancelRecoupReq cancelRecoupReq)
	 	{
	 		return CancelRecoup(cancelRecoupReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetTransactionDetailsResponseType GetTransactionDetails(GetTransactionDetailsReq getTransactionDetailsReq, string apiUserName)
	 	{
			setStandardParams(getTransactionDetailsReq.GetTransactionDetailsRequest);
			string response = Call("GetTransactionDetails", getTransactionDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetTransactionDetailsResponse']");
			return new GetTransactionDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetTransactionDetailsResponseType GetTransactionDetails(GetTransactionDetailsReq getTransactionDetailsReq)
	 	{
	 		return GetTransactionDetails(getTransactionDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public BillUserResponseType BillUser(BillUserReq billUserReq, string apiUserName)
	 	{
			setStandardParams(billUserReq.BillUserRequest);
			string response = Call("BillUser", billUserReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='BillUserResponse']");
			return new BillUserResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public BillUserResponseType BillUser(BillUserReq billUserReq)
	 	{
	 		return BillUser(billUserReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public TransactionSearchResponseType TransactionSearch(TransactionSearchReq transactionSearchReq, string apiUserName)
	 	{
			setStandardParams(transactionSearchReq.TransactionSearchRequest);
			string response = Call("TransactionSearch", transactionSearchReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='TransactionSearchResponse']");
			return new TransactionSearchResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public TransactionSearchResponseType TransactionSearch(TransactionSearchReq transactionSearchReq)
	 	{
	 		return TransactionSearch(transactionSearchReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public MassPayResponseType MassPay(MassPayReq massPayReq, string apiUserName)
	 	{
			setStandardParams(massPayReq.MassPayRequest);
			string response = Call("MassPay", massPayReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='MassPayResponse']");
			return new MassPayResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public MassPayResponseType MassPay(MassPayReq massPayReq)
	 	{
	 		return MassPay(massPayReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public BAUpdateResponseType BillAgreementUpdate(BillAgreementUpdateReq billAgreementUpdateReq, string apiUserName)
	 	{
			setStandardParams(billAgreementUpdateReq.BAUpdateRequest);
			string response = Call("BillAgreementUpdate", billAgreementUpdateReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='BAUpdateResponse']");
			return new BAUpdateResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public BAUpdateResponseType BillAgreementUpdate(BillAgreementUpdateReq billAgreementUpdateReq)
	 	{
	 		return BillAgreementUpdate(billAgreementUpdateReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public AddressVerifyResponseType AddressVerify(AddressVerifyReq addressVerifyReq, string apiUserName)
	 	{
			setStandardParams(addressVerifyReq.AddressVerifyRequest);
			string response = Call("AddressVerify", addressVerifyReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='AddressVerifyResponse']");
			return new AddressVerifyResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public AddressVerifyResponseType AddressVerify(AddressVerifyReq addressVerifyReq)
	 	{
	 		return AddressVerify(addressVerifyReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public EnterBoardingResponseType EnterBoarding(EnterBoardingReq enterBoardingReq, string apiUserName)
	 	{
			setStandardParams(enterBoardingReq.EnterBoardingRequest);
			string response = Call("EnterBoarding", enterBoardingReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='EnterBoardingResponse']");
			return new EnterBoardingResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public EnterBoardingResponseType EnterBoarding(EnterBoardingReq enterBoardingReq)
	 	{
	 		return EnterBoarding(enterBoardingReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetBoardingDetailsResponseType GetBoardingDetails(GetBoardingDetailsReq getBoardingDetailsReq, string apiUserName)
	 	{
			setStandardParams(getBoardingDetailsReq.GetBoardingDetailsRequest);
			string response = Call("GetBoardingDetails", getBoardingDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetBoardingDetailsResponse']");
			return new GetBoardingDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetBoardingDetailsResponseType GetBoardingDetails(GetBoardingDetailsReq getBoardingDetailsReq)
	 	{
	 		return GetBoardingDetails(getBoardingDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CreateMobilePaymentResponseType CreateMobilePayment(CreateMobilePaymentReq createMobilePaymentReq, string apiUserName)
	 	{
			setStandardParams(createMobilePaymentReq.CreateMobilePaymentRequest);
			string response = Call("CreateMobilePayment", createMobilePaymentReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='CreateMobilePaymentResponse']");
			return new CreateMobilePaymentResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CreateMobilePaymentResponseType CreateMobilePayment(CreateMobilePaymentReq createMobilePaymentReq)
	 	{
	 		return CreateMobilePayment(createMobilePaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetMobileStatusResponseType GetMobileStatus(GetMobileStatusReq getMobileStatusReq, string apiUserName)
	 	{
			setStandardParams(getMobileStatusReq.GetMobileStatusRequest);
			string response = Call("GetMobileStatus", getMobileStatusReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetMobileStatusResponse']");
			return new GetMobileStatusResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetMobileStatusResponseType GetMobileStatus(GetMobileStatusReq getMobileStatusReq)
	 	{
	 		return GetMobileStatus(getMobileStatusReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetMobileCheckoutResponseType SetMobileCheckout(SetMobileCheckoutReq setMobileCheckoutReq, string apiUserName)
	 	{
			setStandardParams(setMobileCheckoutReq.SetMobileCheckoutRequest);
			string response = Call("SetMobileCheckout", setMobileCheckoutReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='SetMobileCheckoutResponse']");
			return new SetMobileCheckoutResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetMobileCheckoutResponseType SetMobileCheckout(SetMobileCheckoutReq setMobileCheckoutReq)
	 	{
	 		return SetMobileCheckout(setMobileCheckoutReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoMobileCheckoutPaymentResponseType DoMobileCheckoutPayment(DoMobileCheckoutPaymentReq doMobileCheckoutPaymentReq, string apiUserName)
	 	{
			setStandardParams(doMobileCheckoutPaymentReq.DoMobileCheckoutPaymentRequest);
			string response = Call("DoMobileCheckoutPayment", doMobileCheckoutPaymentReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoMobileCheckoutPaymentResponse']");
			return new DoMobileCheckoutPaymentResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoMobileCheckoutPaymentResponseType DoMobileCheckoutPayment(DoMobileCheckoutPaymentReq doMobileCheckoutPaymentReq)
	 	{
	 		return DoMobileCheckoutPayment(doMobileCheckoutPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetBalanceResponseType GetBalance(GetBalanceReq getBalanceReq, string apiUserName)
	 	{
			setStandardParams(getBalanceReq.GetBalanceRequest);
			string response = Call("GetBalance", getBalanceReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetBalanceResponse']");
			return new GetBalanceResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetBalanceResponseType GetBalance(GetBalanceReq getBalanceReq)
	 	{
	 		return GetBalance(getBalanceReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetPalDetailsResponseType GetPalDetails(GetPalDetailsReq getPalDetailsReq, string apiUserName)
	 	{
			setStandardParams(getPalDetailsReq.GetPalDetailsRequest);
			string response = Call("GetPalDetails", getPalDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetPalDetailsResponse']");
			return new GetPalDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetPalDetailsResponseType GetPalDetails(GetPalDetailsReq getPalDetailsReq)
	 	{
	 		return GetPalDetails(getPalDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoExpressCheckoutPaymentResponseType DoExpressCheckoutPayment(DoExpressCheckoutPaymentReq doExpressCheckoutPaymentReq, string apiUserName)
	 	{
			setStandardParams(doExpressCheckoutPaymentReq.DoExpressCheckoutPaymentRequest);
			string response = Call("DoExpressCheckoutPayment", doExpressCheckoutPaymentReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoExpressCheckoutPaymentResponse']");
			return new DoExpressCheckoutPaymentResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoExpressCheckoutPaymentResponseType DoExpressCheckoutPayment(DoExpressCheckoutPaymentReq doExpressCheckoutPaymentReq)
	 	{
	 		return DoExpressCheckoutPayment(doExpressCheckoutPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoUATPExpressCheckoutPaymentResponseType DoUATPExpressCheckoutPayment(DoUATPExpressCheckoutPaymentReq doUATPExpressCheckoutPaymentReq, string apiUserName)
	 	{
			setStandardParams(doUATPExpressCheckoutPaymentReq.DoUATPExpressCheckoutPaymentRequest);
			string response = Call("DoUATPExpressCheckoutPayment", doUATPExpressCheckoutPaymentReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoUATPExpressCheckoutPaymentResponse']");
			return new DoUATPExpressCheckoutPaymentResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoUATPExpressCheckoutPaymentResponseType DoUATPExpressCheckoutPayment(DoUATPExpressCheckoutPaymentReq doUATPExpressCheckoutPaymentReq)
	 	{
	 		return DoUATPExpressCheckoutPayment(doUATPExpressCheckoutPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetAuthFlowParamResponseType SetAuthFlowParam(SetAuthFlowParamReq setAuthFlowParamReq, string apiUserName)
	 	{
			setStandardParams(setAuthFlowParamReq.SetAuthFlowParamRequest);
			string response = Call("SetAuthFlowParam", setAuthFlowParamReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='SetAuthFlowParamResponse']");
			return new SetAuthFlowParamResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetAuthFlowParamResponseType SetAuthFlowParam(SetAuthFlowParamReq setAuthFlowParamReq)
	 	{
	 		return SetAuthFlowParam(setAuthFlowParamReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetAuthDetailsResponseType GetAuthDetails(GetAuthDetailsReq getAuthDetailsReq, string apiUserName)
	 	{
			setStandardParams(getAuthDetailsReq.GetAuthDetailsRequest);
			string response = Call("GetAuthDetails", getAuthDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetAuthDetailsResponse']");
			return new GetAuthDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetAuthDetailsResponseType GetAuthDetails(GetAuthDetailsReq getAuthDetailsReq)
	 	{
	 		return GetAuthDetails(getAuthDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetAccessPermissionsResponseType SetAccessPermissions(SetAccessPermissionsReq setAccessPermissionsReq, string apiUserName)
	 	{
			setStandardParams(setAccessPermissionsReq.SetAccessPermissionsRequest);
			string response = Call("SetAccessPermissions", setAccessPermissionsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='SetAccessPermissionsResponse']");
			return new SetAccessPermissionsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetAccessPermissionsResponseType SetAccessPermissions(SetAccessPermissionsReq setAccessPermissionsReq)
	 	{
	 		return SetAccessPermissions(setAccessPermissionsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public UpdateAccessPermissionsResponseType UpdateAccessPermissions(UpdateAccessPermissionsReq updateAccessPermissionsReq, string apiUserName)
	 	{
			setStandardParams(updateAccessPermissionsReq.UpdateAccessPermissionsRequest);
			string response = Call("UpdateAccessPermissions", updateAccessPermissionsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='UpdateAccessPermissionsResponse']");
			return new UpdateAccessPermissionsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public UpdateAccessPermissionsResponseType UpdateAccessPermissions(UpdateAccessPermissionsReq updateAccessPermissionsReq)
	 	{
	 		return UpdateAccessPermissions(updateAccessPermissionsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetAccessPermissionDetailsResponseType GetAccessPermissionDetails(GetAccessPermissionDetailsReq getAccessPermissionDetailsReq, string apiUserName)
	 	{
			setStandardParams(getAccessPermissionDetailsReq.GetAccessPermissionDetailsRequest);
			string response = Call("GetAccessPermissionDetails", getAccessPermissionDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetAccessPermissionDetailsResponse']");
			return new GetAccessPermissionDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetAccessPermissionDetailsResponseType GetAccessPermissionDetails(GetAccessPermissionDetailsReq getAccessPermissionDetailsReq)
	 	{
	 		return GetAccessPermissionDetails(getAccessPermissionDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetIncentiveEvaluationResponseType GetIncentiveEvaluation(GetIncentiveEvaluationReq getIncentiveEvaluationReq, string apiUserName)
	 	{
			setStandardParams(getIncentiveEvaluationReq.GetIncentiveEvaluationRequest);
			string response = Call("GetIncentiveEvaluation", getIncentiveEvaluationReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetIncentiveEvaluationResponse']");
			return new GetIncentiveEvaluationResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetIncentiveEvaluationResponseType GetIncentiveEvaluation(GetIncentiveEvaluationReq getIncentiveEvaluationReq)
	 	{
	 		return GetIncentiveEvaluation(getIncentiveEvaluationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutReq setExpressCheckoutReq, string apiUserName)
	 	{
			setStandardParams(setExpressCheckoutReq.SetExpressCheckoutRequest);
			string response = Call("SetExpressCheckout", setExpressCheckoutReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='SetExpressCheckoutResponse']");
			return new SetExpressCheckoutResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutReq setExpressCheckoutReq)
	 	{
	 		return SetExpressCheckout(setExpressCheckoutReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ExecuteCheckoutOperationsResponseType ExecuteCheckoutOperations(ExecuteCheckoutOperationsReq executeCheckoutOperationsReq, string apiUserName)
	 	{
			setStandardParams(executeCheckoutOperationsReq.ExecuteCheckoutOperationsRequest);
			string response = Call("ExecuteCheckoutOperations", executeCheckoutOperationsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='ExecuteCheckoutOperationsResponse']");
			return new ExecuteCheckoutOperationsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ExecuteCheckoutOperationsResponseType ExecuteCheckoutOperations(ExecuteCheckoutOperationsReq executeCheckoutOperationsReq)
	 	{
	 		return ExecuteCheckoutOperations(executeCheckoutOperationsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(GetExpressCheckoutDetailsReq getExpressCheckoutDetailsReq, string apiUserName)
	 	{
			setStandardParams(getExpressCheckoutDetailsReq.GetExpressCheckoutDetailsRequest);
			string response = Call("GetExpressCheckoutDetails", getExpressCheckoutDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetExpressCheckoutDetailsResponse']");
			return new GetExpressCheckoutDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(GetExpressCheckoutDetailsReq getExpressCheckoutDetailsReq)
	 	{
	 		return GetExpressCheckoutDetails(getExpressCheckoutDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoDirectPaymentResponseType DoDirectPayment(DoDirectPaymentReq doDirectPaymentReq, string apiUserName)
	 	{
			setStandardParams(doDirectPaymentReq.DoDirectPaymentRequest);
			string response = Call("DoDirectPayment", doDirectPaymentReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoDirectPaymentResponse']");
			return new DoDirectPaymentResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoDirectPaymentResponseType DoDirectPayment(DoDirectPaymentReq doDirectPaymentReq)
	 	{
	 		return DoDirectPayment(doDirectPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ManagePendingTransactionStatusResponseType ManagePendingTransactionStatus(ManagePendingTransactionStatusReq managePendingTransactionStatusReq, string apiUserName)
	 	{
			setStandardParams(managePendingTransactionStatusReq.ManagePendingTransactionStatusRequest);
			string response = Call("ManagePendingTransactionStatus", managePendingTransactionStatusReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='ManagePendingTransactionStatusResponse']");
			return new ManagePendingTransactionStatusResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ManagePendingTransactionStatusResponseType ManagePendingTransactionStatus(ManagePendingTransactionStatusReq managePendingTransactionStatusReq)
	 	{
	 		return ManagePendingTransactionStatus(managePendingTransactionStatusReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoCancelResponseType DoCancel(DoCancelReq doCancelReq, string apiUserName)
	 	{
			setStandardParams(doCancelReq.DoCancelRequest);
			string response = Call("DoCancel", doCancelReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoCancelResponse']");
			return new DoCancelResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoCancelResponseType DoCancel(DoCancelReq doCancelReq)
	 	{
	 		return DoCancel(doCancelReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoCaptureResponseType DoCapture(DoCaptureReq doCaptureReq, string apiUserName)
	 	{
			setStandardParams(doCaptureReq.DoCaptureRequest);
			string response = Call("DoCapture", doCaptureReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoCaptureResponse']");
			return new DoCaptureResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoCaptureResponseType DoCapture(DoCaptureReq doCaptureReq)
	 	{
	 		return DoCapture(doCaptureReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoReauthorizationResponseType DoReauthorization(DoReauthorizationReq doReauthorizationReq, string apiUserName)
	 	{
			setStandardParams(doReauthorizationReq.DoReauthorizationRequest);
			string response = Call("DoReauthorization", doReauthorizationReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoReauthorizationResponse']");
			return new DoReauthorizationResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoReauthorizationResponseType DoReauthorization(DoReauthorizationReq doReauthorizationReq)
	 	{
	 		return DoReauthorization(doReauthorizationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoVoidResponseType DoVoid(DoVoidReq doVoidReq, string apiUserName)
	 	{
			setStandardParams(doVoidReq.DoVoidRequest);
			string response = Call("DoVoid", doVoidReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoVoidResponse']");
			return new DoVoidResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoVoidResponseType DoVoid(DoVoidReq doVoidReq)
	 	{
	 		return DoVoid(doVoidReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoAuthorizationResponseType DoAuthorization(DoAuthorizationReq doAuthorizationReq, string apiUserName)
	 	{
			setStandardParams(doAuthorizationReq.DoAuthorizationRequest);
			string response = Call("DoAuthorization", doAuthorizationReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoAuthorizationResponse']");
			return new DoAuthorizationResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoAuthorizationResponseType DoAuthorization(DoAuthorizationReq doAuthorizationReq)
	 	{
	 		return DoAuthorization(doAuthorizationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetCustomerBillingAgreementResponseType SetCustomerBillingAgreement(SetCustomerBillingAgreementReq setCustomerBillingAgreementReq, string apiUserName)
	 	{
			setStandardParams(setCustomerBillingAgreementReq.SetCustomerBillingAgreementRequest);
			string response = Call("SetCustomerBillingAgreement", setCustomerBillingAgreementReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='SetCustomerBillingAgreementResponse']");
			return new SetCustomerBillingAgreementResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetCustomerBillingAgreementResponseType SetCustomerBillingAgreement(SetCustomerBillingAgreementReq setCustomerBillingAgreementReq)
	 	{
	 		return SetCustomerBillingAgreement(setCustomerBillingAgreementReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetBillingAgreementCustomerDetailsResponseType GetBillingAgreementCustomerDetails(GetBillingAgreementCustomerDetailsReq getBillingAgreementCustomerDetailsReq, string apiUserName)
	 	{
			setStandardParams(getBillingAgreementCustomerDetailsReq.GetBillingAgreementCustomerDetailsRequest);
			string response = Call("GetBillingAgreementCustomerDetails", getBillingAgreementCustomerDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetBillingAgreementCustomerDetailsResponse']");
			return new GetBillingAgreementCustomerDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetBillingAgreementCustomerDetailsResponseType GetBillingAgreementCustomerDetails(GetBillingAgreementCustomerDetailsReq getBillingAgreementCustomerDetailsReq)
	 	{
	 		return GetBillingAgreementCustomerDetails(getBillingAgreementCustomerDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CreateBillingAgreementResponseType CreateBillingAgreement(CreateBillingAgreementReq createBillingAgreementReq, string apiUserName)
	 	{
			setStandardParams(createBillingAgreementReq.CreateBillingAgreementRequest);
			string response = Call("CreateBillingAgreement", createBillingAgreementReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='CreateBillingAgreementResponse']");
			return new CreateBillingAgreementResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CreateBillingAgreementResponseType CreateBillingAgreement(CreateBillingAgreementReq createBillingAgreementReq)
	 	{
	 		return CreateBillingAgreement(createBillingAgreementReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoReferenceTransactionResponseType DoReferenceTransaction(DoReferenceTransactionReq doReferenceTransactionReq, string apiUserName)
	 	{
			setStandardParams(doReferenceTransactionReq.DoReferenceTransactionRequest);
			string response = Call("DoReferenceTransaction", doReferenceTransactionReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoReferenceTransactionResponse']");
			return new DoReferenceTransactionResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoReferenceTransactionResponseType DoReferenceTransaction(DoReferenceTransactionReq doReferenceTransactionReq)
	 	{
	 		return DoReferenceTransaction(doReferenceTransactionReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoNonReferencedCreditResponseType DoNonReferencedCredit(DoNonReferencedCreditReq doNonReferencedCreditReq, string apiUserName)
	 	{
			setStandardParams(doNonReferencedCreditReq.DoNonReferencedCreditRequest);
			string response = Call("DoNonReferencedCredit", doNonReferencedCreditReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoNonReferencedCreditResponse']");
			return new DoNonReferencedCreditResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoNonReferencedCreditResponseType DoNonReferencedCredit(DoNonReferencedCreditReq doNonReferencedCreditReq)
	 	{
	 		return DoNonReferencedCredit(doNonReferencedCreditReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoUATPAuthorizationResponseType DoUATPAuthorization(DoUATPAuthorizationReq doUATPAuthorizationReq, string apiUserName)
	 	{
			setStandardParams(doUATPAuthorizationReq.DoUATPAuthorizationRequest);
			string response = Call("DoUATPAuthorization", doUATPAuthorizationReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='DoUATPAuthorizationResponse']");
			return new DoUATPAuthorizationResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoUATPAuthorizationResponseType DoUATPAuthorization(DoUATPAuthorizationReq doUATPAuthorizationReq)
	 	{
	 		return DoUATPAuthorization(doUATPAuthorizationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CreateRecurringPaymentsProfileResponseType CreateRecurringPaymentsProfile(CreateRecurringPaymentsProfileReq createRecurringPaymentsProfileReq, string apiUserName)
	 	{
			setStandardParams(createRecurringPaymentsProfileReq.CreateRecurringPaymentsProfileRequest);
			string response = Call("CreateRecurringPaymentsProfile", createRecurringPaymentsProfileReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='CreateRecurringPaymentsProfileResponse']");
			return new CreateRecurringPaymentsProfileResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CreateRecurringPaymentsProfileResponseType CreateRecurringPaymentsProfile(CreateRecurringPaymentsProfileReq createRecurringPaymentsProfileReq)
	 	{
	 		return CreateRecurringPaymentsProfile(createRecurringPaymentsProfileReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetRecurringPaymentsProfileDetailsResponseType GetRecurringPaymentsProfileDetails(GetRecurringPaymentsProfileDetailsReq getRecurringPaymentsProfileDetailsReq, string apiUserName)
	 	{
			setStandardParams(getRecurringPaymentsProfileDetailsReq.GetRecurringPaymentsProfileDetailsRequest);
			string response = Call("GetRecurringPaymentsProfileDetails", getRecurringPaymentsProfileDetailsReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='GetRecurringPaymentsProfileDetailsResponse']");
			return new GetRecurringPaymentsProfileDetailsResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetRecurringPaymentsProfileDetailsResponseType GetRecurringPaymentsProfileDetails(GetRecurringPaymentsProfileDetailsReq getRecurringPaymentsProfileDetailsReq)
	 	{
	 		return GetRecurringPaymentsProfileDetails(getRecurringPaymentsProfileDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ManageRecurringPaymentsProfileStatusResponseType ManageRecurringPaymentsProfileStatus(ManageRecurringPaymentsProfileStatusReq manageRecurringPaymentsProfileStatusReq, string apiUserName)
	 	{
			setStandardParams(manageRecurringPaymentsProfileStatusReq.ManageRecurringPaymentsProfileStatusRequest);
			string response = Call("ManageRecurringPaymentsProfileStatus", manageRecurringPaymentsProfileStatusReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='ManageRecurringPaymentsProfileStatusResponse']");
			return new ManageRecurringPaymentsProfileStatusResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ManageRecurringPaymentsProfileStatusResponseType ManageRecurringPaymentsProfileStatus(ManageRecurringPaymentsProfileStatusReq manageRecurringPaymentsProfileStatusReq)
	 	{
	 		return ManageRecurringPaymentsProfileStatus(manageRecurringPaymentsProfileStatusReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public BillOutstandingAmountResponseType BillOutstandingAmount(BillOutstandingAmountReq billOutstandingAmountReq, string apiUserName)
	 	{
			setStandardParams(billOutstandingAmountReq.BillOutstandingAmountRequest);
			string response = Call("BillOutstandingAmount", billOutstandingAmountReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='BillOutstandingAmountResponse']");
			return new BillOutstandingAmountResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public BillOutstandingAmountResponseType BillOutstandingAmount(BillOutstandingAmountReq billOutstandingAmountReq)
	 	{
	 		return BillOutstandingAmount(billOutstandingAmountReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public UpdateRecurringPaymentsProfileResponseType UpdateRecurringPaymentsProfile(UpdateRecurringPaymentsProfileReq updateRecurringPaymentsProfileReq, string apiUserName)
	 	{
			setStandardParams(updateRecurringPaymentsProfileReq.UpdateRecurringPaymentsProfileRequest);
			string response = Call("UpdateRecurringPaymentsProfile", updateRecurringPaymentsProfileReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='UpdateRecurringPaymentsProfileResponse']");
			return new UpdateRecurringPaymentsProfileResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public UpdateRecurringPaymentsProfileResponseType UpdateRecurringPaymentsProfile(UpdateRecurringPaymentsProfileReq updateRecurringPaymentsProfileReq)
	 	{
	 		return UpdateRecurringPaymentsProfile(updateRecurringPaymentsProfileReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ReverseTransactionResponseType ReverseTransaction(ReverseTransactionReq reverseTransactionReq, string apiUserName)
	 	{
			setStandardParams(reverseTransactionReq.ReverseTransactionRequest);
			string response = Call("ReverseTransaction", reverseTransactionReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='ReverseTransactionResponse']");
			return new ReverseTransactionResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ReverseTransactionResponseType ReverseTransaction(ReverseTransactionReq reverseTransactionReq)
	 	{
	 		return ReverseTransaction(reverseTransactionReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ExternalRememberMeOptOutResponseType ExternalRememberMeOptOut(ExternalRememberMeOptOutReq externalRememberMeOptOutReq, string apiUserName)
	 	{
			setStandardParams(externalRememberMeOptOutReq.ExternalRememberMeOptOutRequest);
			string response = Call("ExternalRememberMeOptOut", externalRememberMeOptOutReq.ToXMLString(), apiUserName);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='ExternalRememberMeOptOutResponse']");
			return new ExternalRememberMeOptOutResponseType(xmlNode);
			
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ExternalRememberMeOptOutResponseType ExternalRememberMeOptOut(ExternalRememberMeOptOutReq externalRememberMeOptOutReq)
	 	{
	 		return ExternalRememberMeOptOut(externalRememberMeOptOutReq, null);
	 	}
	}
}
