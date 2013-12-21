using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using System.Web;
using Vre.Server.Task;

namespace Vre.Server.RemoteService
{
    internal class ReverseRequestService : IHttpService
    {
        private static Dictionary<string, string> _messageTemplates = new Dictionary<string, string>();

        private const string _servicePathPrefix = _servicePathElement0 + "/";
        private const string _servicePathElement0 = "go";
        private const int _maxRequestLinkLength = 64;  // the link is currently a UID (see UniversalId) or GUID (legacy)

        public string ServicePathPrefix { get { return _servicePathPrefix; } }

        public bool RequiresSession { get { return false; } }

        #region unused request types
        public void ProcessCreateRequest(IServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public void ProcessReplaceRequest(IServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public void ProcessDeleteRequest(IServiceRequest request)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void ProcessGetRequest(IServiceRequest request)
        {
            if (!Configuration.Security.AllowSensitiveDataOverNonSecureConnection.Value
				&& !request.Request.IsSecureConnection)
                throw new PermissionException("Service available only over secure connection.");
            
            string path = request.Request.PathSegments[1];
			//int pos = path.IndexOf('/');
			//if ((pos > 0) && (pos < path.Length)) path = path.Substring(pos + 1);

            if (path.Length > _maxRequestLinkLength)
                throw new ArgumentException();

            Guid rqid;
            switch (UniversalId.TypeInUrlId(path))
            {
                default:
                    throw new ArgumentException();

                case UniversalId.IdType.Unknown:  // legacy
                    if (!Guid.TryParseExact(path, "N", out rqid))
                        throw new ArgumentException();
                    break;

                case UniversalId.IdType.ReverseRequest:
                    rqid = UniversalId.ExtractAsGuid(path);
                    break;
            }

            using (ISession session = NHibernateHelper.GetSession())
            {
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
                {
                    bool dataChanged = false;
                    ReverseRequest rq;
                    using (ReverseRequestDao dao = new ReverseRequestDao(session))
                        rq = dao.GetById(rqid);

                    if (null == rq)
                    {
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Error("{0} Reverse is not found: {1}", rn, rqid);
                        makeHttpResponse(request, null, "HTML_GENERIC_ERROR",
                            "Request is unknown or expired", rn);
                    }
                    else
                    {
						rq.Touch();
						session.Update(rq);

                        if (rq.IsAttemptSensitive)
                        {
                            // TODO: Prevent brute-force
                        }

                        switch (rq.Request)
                        {
                            case ReverseRequest.RequestType.AccountRegistration:
                                dataChanged = processAccountRegister(request, rq, session);
                                break;

                            case ReverseRequest.RequestType.LoginChange:
                                dataChanged = processLoginChange(request, rq, session);
                                break;

                            case ReverseRequest.RequestType.ViewOrderControl:
                                dataChanged = processViewOrderControl(request, rq, session);
                                break;

                            //case ReverseRequest.RequestType.Listing:
                            //    processListingAccess(request, rq, session);
                            //    break;

							case ReverseRequest.RequestType.ViewOrderActivation:
								processViewOrderActivation(request, rq, session);
								break;

                            // TODO: the rest of requests

                            default:
                                string rn = Utilities.GenerateReferenceNumber();
                                ServiceInstances.Logger.Error("{0} Reverse request type is not known: {1}", rn, rq.Request);
                                makeHttpResponse(request, null, "HTML_GENERIC_ERROR",
                                    "Unknown request", rn);
                                break;
                        }
                    }

                    if (dataChanged) tran.Commit();
                }
            }
        }

        #region utility methods
        public static string GenerateUrl(ReverseRequest rq)
        {
            return string.Format(Configuration.Urls.StaticLinkTemplate.Value, 
				UniversalId.GenerateUrlId(UniversalId.IdType.ReverseRequest, rq.Id));
        }

		public static string GenerateUrl(ViewOrder viewOrder)
		{
			return string.Format(Configuration.Urls.StaticLinkTemplate.Value,
				UniversalId.GenerateUrlId(UniversalId.IdType.ViewOrder, viewOrder.AutoID));
		}

		private static readonly string[] _refParamNameVariations = new string[] { "email1", "email2" };

        private static string generateReferenceParameterName()
        {
            Random r = new Random();
            return _refParamNameVariations[r.Next(_refParamNameVariations.Length)];
        }

        private static string generateReferenceParameterValue()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static void makeHttpResponse(IServiceRequest request, User user, string templateName, params object[] parameters)
        {
            string html = templateName;
            try
            {
                html = ServiceInstances.MessageGen.GetMessage(user, templateName, parameters);

                request.Response.ResponseCode = HttpStatusCode.OK;
                request.Response.DataStreamContentType = "html";

                // cannot use a StreamWriter here as it disposes off the stream after use!
                byte[] binary = Encoding.UTF8.GetBytes(html);
                request.Response.DataStream.Write(binary, 0, binary.Length);
            }
            catch (Exception)
            {
                ServiceInstances.Logger.Fatal(html);
                throw;
            }
        }

        private static void makeJavascriptResponse(IServiceRequest request, string refNum, string textTemplateName, User relatedUser)
        {
            request.Response.ResponseCode = HttpStatusCode.OK;
            request.Response.DataStreamContentType = "txt";

            // cannot use a StreamWriter here as it disposes off the stream after use!
            byte[] binary = Encoding.UTF8.GetBytes(string.Format("{0}|{1}", 
                refNum, ServiceInstances.MessageGen.GetMessage(relatedUser, textTemplateName)));
            request.Response.DataStream.Write(binary, 0, binary.Length);
        }
        #endregion

        #region Account registration
        public static void InitiateUserRegistration(string login)
        {
            ReverseRequest request;

            using (ISession dbSession = NHibernateHelper.GetSession())
            {
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(dbSession))
                {
                    bool test;
                    using (IAuthentication auth = new Authentication(dbSession))
                        test = (auth.UserIdByLogin(LoginType.Plain, User.Role.SellingAgent, -1, login) >= 0);

                    if (test)
                        throw new InvalidOperationException("The login '" + login + "' is in use");

                    using (ReverseRequestDao dao = new ReverseRequestDao(dbSession))
                    {
                        request = dao.GetByLoginAndType(login, ReverseRequest.RequestType.AccountRegistration);

                        if (null == request)
                        {
                            request = ReverseRequest.CreateRegisterAccount(login, null,
								DateTime.UtcNow.AddSeconds(Configuration.HttpService.ReverseRequestLinkExpirationSec.Value),
                                generateReferenceParameterName(), generateReferenceParameterValue());

                            dao.Create(request);
                        }
                        else
                        {
							request.ProlongUntil(DateTime.UtcNow.AddSeconds(Configuration.HttpService.ReverseRequestLinkExpirationSec.Value));
                            dao.Update(request);
                        }
                    }

                    ServiceInstances.MessageGen.SendMessage(null, login, "MSG_ACCOUNT_CREATE",
                        GenerateUrl(request), request.ExpiresOn);

                    //session.DbSession.Flush();
                    tran.Commit();
                }
            }
        }

        private static bool processAccountRegister(IServiceRequest request, ReverseRequest dbRequest, ISession session)
        {
            bool dataChanged = false;
            string testValue = request.Request.Query.GetParam(dbRequest.ReferenceParamName, string.Empty);
            string refValue = request.Request.Query.GetParam("rid", string.Empty);
            string password = request.Request.Query.GetParam("pwd", string.Empty);
            User user;

            if ((0 == refValue.Length) || (0 == password.Length))
            {
                // a link from email is accessed: generate password entry page
                //
                makeHttpResponse(request, null, "HTML_PASSWORD_ENTRY",
                    UniversalId.GenerateUrlId(UniversalId.IdType.ReverseRequest, dbRequest.Id),
                    dbRequest.ReferenceParamName,
                    "rid", dbRequest.ReferenceParamValue,
                    dbRequest.Login);
            }
            else
            {
                if ((testValue.Length > 0) || !dbRequest.ReferenceParamValue.Equals(refValue)) // TODO: robot action detected!
                {
                    ServiceInstances.Logger.Error(
                        "Robot activity detected (account create).\r\nURL: {0}",
                        request.UserInfo);
                    makeHttpResponse(request, null, "HTML_GENERIC_ERROR", string.Empty, string.Empty);
                }
                else
                {
                    bool result = false;
                    string errorReason = null;

                    // create and fill-in user record
                    //
                    user = new User(null, User.Role.SellingAgent);
                    user.PrimaryEmailAddress = dbRequest.Login;
                    {
                        // TEMP? Use e-mail as a nickname
                        int pos = dbRequest.Login.IndexOf('@');
                        if (pos < 0) pos = dbRequest.Login.Length;
                        user.NickName = dbRequest.Login.Substring(0, pos);
                    }
                    user.PersonalInfo = null;  // TODO
                    //user.BrokerInfo = null;   // TODO

                    using (UserDao dao = new UserDao(session))
                        dao.Create(user);

                    // create login
                    //
                    using (IAuthentication auth = new Authentication(session))
                        result = auth.CreateLogin(LoginType.Plain, User.Role.SellingAgent, -1, dbRequest.Login, password, user.AutoID, out errorReason);

                    if (result)
                    {
                        // remove record and respond with confirmation

                        using (ReverseRequestDao dao = new ReverseRequestDao(session))
                            dao.DeleteByLoginAndType(dbRequest.Login, dbRequest.Request);

                        // successful change callback: generate confirmation information
                        //
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Info("{0} User UID={1} successfully created with login/email {2}.",
                            rn, user.AutoID, dbRequest.Login);

                        // prepare plaintext response to be parsed by javascript on page
                        //
                        makeJavascriptResponse(request, rn, "TXT_ACCOUNT_CREATED", user);

                        dataChanged = true;
                    }
                    else
                    {
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Error("{0} Login create error: {1}", rn, errorReason ?? "Authentication error");
                        throw new InvalidOperationException(errorReason ?? "Authentication error");
                    }
                }
            }
            return dataChanged;
        }
        #endregion

        #region Login change
        public static void InitiateLoginChange(IRequestData srq, ClientSession session, string newLogin)
        {
            ReverseRequest request;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
            {
                bool test;
                using (IAuthentication auth = new Authentication(session.DbSession))
                // TODO: another way of picking login type/role/ED
                    test = (auth.UserIdByLogin(LoginType.Plain, User.Role.SellingAgent, -1, newLogin) >= 0);

                if (test)
                    throw new InvalidOperationException("The login '" + newLogin + "' is in use");

                using (ReverseRequestDao dao = new ReverseRequestDao(session.DbSession))
                {
                    request = dao.GetByLoginAndType(newLogin, ReverseRequest.RequestType.LoginChange);

                    if (null == request)
                    {
                        request = ReverseRequest.CreateLoginChange(session.User.AutoID, newLogin,
                            DateTime.UtcNow.AddSeconds(Configuration.HttpService.ReverseRequestLinkExpirationSec.Value),
                            generateReferenceParameterName(), generateReferenceParameterValue());

                        dao.Create(request);
                    }
                    else
                    {
                        request.ProlongUntil(DateTime.UtcNow.AddSeconds(Configuration.HttpService.ReverseRequestLinkExpirationSec.Value));
                        dao.Update(request);
                    }
                }

                ServiceInstances.MessageGen.SendMessage(null, session.User, newLogin, "MSG_LOGIN_CHANGE",
                    GenerateUrl(request), request.ExpiresOn);

                //session.DbSession.Flush();  // TODO: dunno why this is required!
                tran.Commit();
            }
        }

        private static bool processLoginChange(IServiceRequest request, ReverseRequest dbRequest, ISession session)
        {
            bool dataChanged = false;
            string testValue = request.Request.Query.GetParam(dbRequest.ReferenceParamName, string.Empty);
            string refValue = request.Request.Query.GetParam("rid", string.Empty);
            string password = request.Request.Query.GetParam("pwd", string.Empty);
            string login = null;
            LoginType loginType;
            User.Role role;
            int estateDeveloperId;
            User user;

            // retrieve login by user id
            //
            using (IAuthentication auth = new Authentication(session))
            {
                if (!auth.LoginByUserId(dbRequest.UserId.Value, out loginType, out role, out estateDeveloperId, out login))
                    login = null;
            }
            using (UserDao dao = new UserDao(session)) user = dao.GetById(dbRequest.UserId.Value);

            if ((0 == refValue.Length) || (0 == password.Length))
            {
                // a link from email is accessed: generate account confirmation page
                //
                makeHttpResponse(request, user, "HTML_PASSWORD_CONFIRM",
                    UniversalId.GenerateUrlId(UniversalId.IdType.ReverseRequest, dbRequest.Id),
                    dbRequest.ReferenceParamName,
                    "rid", dbRequest.ReferenceParamValue,
                    login);
            }
            else
            {
                if ((testValue.Length > 0) || !dbRequest.ReferenceParamValue.Equals(refValue)) // TODO: robot action detected!
                {
                    ServiceInstances.Logger.Error(
                        "Robot activity detected (login change).\r\nURL: {0}",
                        request.UserInfo);
                    makeHttpResponse(request, null, "HTML_GENERIC_ERROR", string.Empty, string.Empty);
                }
                else
                {
                    // authenticate user and change login
                    //
                    int userId;
                    bool result = false;
                    string errorReason = null;
                    using (IAuthentication auth = new Authentication(session))
                    {
                        result = auth.AuthenticateUser(loginType, role, estateDeveloperId, login, password, out userId);
                        if (result)
                            result = auth.ChangeLogin(userId, dbRequest.Subject, out errorReason);
                    }

                    if (result)
                    {
                        user.PrimaryEmailAddress = dbRequest.Subject;
                        //using (UserDao dao = new UserDao(session)) dao.SafeCreateOrUpdate(user);
                        session.Save(user);

                        // remove record and respond with confirmation

                        using (ReverseRequestDao dao = new ReverseRequestDao(session))
                            dao.DeleteByLoginAndType(dbRequest.Login, dbRequest.Request);

                        // successful change callback: generate confirmation confirmation page
                        //
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Info("{0} Login for UID={1} successfully changed to {2}.",
                            rn, user.AutoID, login);

                        // prepare plaintext response to be parsed by javascript on page
                        //
                        makeJavascriptResponse(request, rn, "TXT_LOGIN_CHANGED", user);

                        dataChanged = true;
                    }
                    else
                    {
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Error("{0} Login change error: {1}", rn, errorReason ?? "Authentication error");
                        throw new InvalidOperationException(errorReason ?? "Authentication error");
                    }
                }
            }

            return dataChanged;
        }
        #endregion

        #region ViewOrder
        public static ViewOrder CreateViewOrder(IServiceRequest srq, User targetUser, string note,
            ViewOrder.ViewOrderProduct product, ViewOrder.ViewOrderOptions options, string mlsId, string mlsUrl,
            ViewOrder.SubjectType type, int targetObjectId, 
			string productUrl, DateTime expiresOn,
            string paymentSystemRefId)
        {
            ViewOrder viewOrder;
			FinancialTransaction ft;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(srq.UserInfo.Session.DbSession))
            {
                switch (product)
                {
                    case ViewOrder.ViewOrderProduct.PrivateListing:
						// CAN have multiple private listings.
						//using (ViewOrderDao dao = new ViewOrderDao(srq.UserInfo.Session.DbSession))
						//{
						//    if (dao.GetActive(type, targetObjectId).Count > 0)
						//        throw new ObjectExistsException("Listing cannot be created.");  // TODO: should this be exposed?!
						//}
                        break;

                    case ViewOrder.ViewOrderProduct.PublicListing:
                        using (ViewOrderDao dao = new ViewOrderDao(srq.UserInfo.Session.DbSession))
                        {
                            if (dao.GetActive(type, targetObjectId).Count > 0)
                                throw new ObjectExistsException("Listing cannot be created.");  // TODO: should this be exposed?!
                        }
                        break;

                    case ViewOrder.ViewOrderProduct.Building3DLayout:
                        if (type == ViewOrder.SubjectType.Suite)
                            throw new ArgumentException("This product type applies to buildings only.");
                        break;
                }

                viewOrder = new ViewOrder(targetUser.AutoID, 
                    product, options, mlsId, type, targetObjectId, productUrl, expiresOn);

                viewOrder.InfoUrl = mlsUrl;
                viewOrder.Note = note;

                using (ViewOrderDao dao = new ViewOrderDao(srq.UserInfo.Session.DbSession))
                    dao.Create(viewOrder);

                DataService.ReflectViewOrderStatusInTarget(viewOrder, srq.UserInfo.Session.DbSession);

                // Generate financial transaction
                //
                ft = new FinancialTransaction(srq.UserInfo.Session.User.AutoID,
                    FinancialTransaction.AccountType.User, targetUser.AutoID,
                    FinancialTransaction.OperationType.Debit, 0m,
                    FinancialTransaction.TranSubject.ViewOrder,
                    FinancialTransaction.TranTarget.Suite, targetObjectId, viewOrder.AutoID.ToString());

                if (!string.IsNullOrWhiteSpace(paymentSystemRefId))
                    ft.SetPaymentSystemReference(FinancialTransaction.PaymentSystemType.CondoExplorer, paymentSystemRefId);

                using (FinancialTransactionDao dao = new FinancialTransactionDao(srq.UserInfo.Session.DbSession))
                {
                    dao.Create(ft);
                    ft.SetAutoSystemReferenceId();
                    dao.Update(ft);
                }

                srq.Response.ResponseCode = HttpStatusCode.OK;
                srq.Response.Data = new ClientData();
                srq.Response.Data.Add("viewOrder-url", GenerateUrl(viewOrder));
                srq.Response.Data.Add("viewOrder-id", viewOrder.AutoID);
                // TODO: generate button into listing response
                //srq.Response.Data.Add("button-url", string.Format(_listingUrlTemplate, request.Id.ToString("N")));
				srq.Response.Data.Add("ref", ft.SystemRefId);

                tran.Commit();
            }

            ServiceInstances.Logger.Info(
                "User {0} created a view order for user {1}: ({2}) for {3} id={4}, reference URL={5}, expires on {6} UTC, VOID={7}, STRN={8}",
				srq.UserInfo.Session.User, targetUser, product, type, targetObjectId, productUrl, expiresOn, viewOrder.AutoID, ft.SystemRefId);

            return viewOrder;
        }

        //public static void DecodeListing(ClientSession session, string listingId,
        //    out UpdateableBase target, out Listing.ListingType type, out string url)
        //{
        //    Guid rqid;
        //    if (!Guid.TryParseExact(listingId, "N", out rqid))
        //        throw new ArgumentException();

        //    Listing listing;
        //    using (ListingDao dao = new ListingDao(session.DbSession))
        //        listing = dao.GetById(rqid);

        //    target = null;
        //    type = Listing.ListingType.ExternalTour;
        //    url = null;

        //    if ((listing != null) && (listing.ExpiresOn < DateTime.UtcNow))
        //    {
        //        listing.Touch();
        //        using (ListingDao dao = new ListingDao(session.DbSession))
        //            dao.Update(listing);
                
        //        type = listing.Product;
        //        url = listing.ProductUrl;

        //        using (SuiteDao dao = new SuiteDao(session.DbSession))
        //            target = dao.GetById(listing.TargetObjectId);
        //    }
        //}

		private static void processViewOrderActivation(IServiceRequest request, ReverseRequest dbRequest, ISession session)
		{
			ViewOrder vo;
			using (var dao = new ViewOrderDao(session)) vo = dao.GetById(new Guid(dbRequest.Subject));

			User user;
			using (var dao = new UserDao(session)) user = dao.GetById(dbRequest.UserId.Value);

			if (string.IsNullOrWhiteSpace(dbRequest.ReferenceParamValue)  // is reset by payment system notification processing
				|| "next".Equals(request.Request.Query["action"]))
				//(3 == request.Request.PathSegments.Length) && (request.Request.PathSegments[2].Equals("next")))
			{
				makeHttpResponse(request, user, "HTML_VIEWORDER_PENDING", 
					vo.Product,
					vo.Options,
					NotifyExpiringViewOrders.GetSubjectAddress(session, vo)
				);
			}
			else
			{
				// https://developer.paypal.com/webapps/developer/docs/classic/paypal-payments-standard/integration-guide/Appx_websitestandard_htmlvariables
				request.Response.RedirectionUrl = string.Format("{6}/cgi-bin/webscr?cmd=_xclick&"
					+ "business={0}&"
					+ "item_name={1}&"
					+ "item_number={2}&" 
					+ "amount={3}&"
					+ "currency_code={4}&"
					+ "notify_url={5}&"
					+ "return={7}&rm=1&"
					+ "country=CA&email={8}",
					HttpUtility.UrlEncode(Configuration.PaymentSystem.PayPal.MerchantId.Value),
					HttpUtility.UrlEncode(vo.Product.ToString() + " (" + NotifyExpiringViewOrders.GetSubjectAddress(session, vo) + ")"),
					HttpUtility.UrlEncode(UniversalId.GenerateUrlId(UniversalId.IdType.ReverseRequest, dbRequest.Id)),
					HttpUtility.UrlEncode(dbRequest.ReferenceParamValue.Substring(3)),
					HttpUtility.UrlEncode(dbRequest.ReferenceParamValue.Substring(0, 3)),
					HttpUtility.UrlEncode(request.Request.ConstructClientRootUri() + "ps/paypal"),
					Configuration.PaymentSystem.SandboxMode.Value ? "https://www.sandbox.paypal.com" : "https://www.paypal.com",
					HttpUtility.UrlEncode(GenerateUrl(dbRequest) + "&action=next"),
					HttpUtility.UrlEncode(user.PrimaryEmailAddress));

				if (!string.IsNullOrEmpty(user.PersonalInfo))
				{
					try
					{
						// https://cms.paypal.com/uk/cgi-bin/?cmd=_render-content&content_ID=developer/e_howto_html_Appx_websitestandard_htmlvariables#id08A6HI0J0VU
						//var vc = new Thought.vCards.vCard(new StringReader(user.PersonalInfo));

						//vc.DeliveryAddresses[0].

						// address1, address2, city, state, zip, country
						// first_name, last_name
						// night_phone_a, night_phone_b, night_phone_c:
						//  US: "261" "456" "7456"
						// !US: "7495" "4567456" ""
					}
					catch (Exception ex)
					{
						ServiceInstances.Logger.Error("ProcessViewOrderActivation: "
							+ "failed adding user information into payment system: {0}", ex);
					}
				}
				/*
				if ((user.UserRole == User.Role.Buyer) || (user.UserRole == User.Role.BuyingAgent)
					|| (user.UserRole == User.Role.SellingAgent))
				{
					request.Response.RedirectionUrl = request.Response.RedirectionUrl +
						string.Format("&",
						user.PersonalInfo
				}
				*/

				ServiceInstances.Logger.Debug("ProcessViewOrderActivation: "
					+ " payment system URL: " + request.Response.RedirectionUrl);
			}
		}
        #endregion

        #region vieworder control
        public static string CreateViewOrderControlUrl(ISession session, ViewOrder vo)
        {
            bool created = false;
            ReverseRequest request;

            //using (ISession session = NHibernateHelper.GetSession())
            {
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
                {
                    using (ReverseRequestDao dao = new ReverseRequestDao(session))
                    {
                        request = dao.GetBySubjectAndType(vo.AutoID.ToString(), ReverseRequest.RequestType.ViewOrderControl);
                        if (null == request)
                        {
                            request = ReverseRequest.CreateViewOrderControl(vo.OwnerId, vo,
                                vo.ExpiresOn.AddYears(1),  // TODO: INTERMEDIATE SOLUTION
                                generateReferenceParameterName(), generateReferenceParameterValue());
                            dao.Create(request);
                            created = true;
                        }
                    }

                    if (created) tran.Commit();
                }
            }

            return GenerateUrl(request);
        }

        private static bool processViewOrderControl(IServiceRequest request, ReverseRequest dbRequest, ISession session)
        {
            bool dataChanged = false;
            string testValue = request.Request.Query.GetParam(dbRequest.ReferenceParamName, string.Empty);
            string refValue = request.Request.Query.GetParam("rid", string.Empty);
            string control = request.Request.Query.GetParam("ctl", string.Empty);
            User user;
            ViewOrder vo;

            using (UserDao dao = new UserDao(session)) user = dao.GetById(dbRequest.UserId.Value);
            using (ViewOrderDao dao = new ViewOrderDao(session)) vo = dao.GetById(new Guid(dbRequest.Subject));

            DateTime newExpiry = DateTime.MinValue;

            if (vo != null)
            {
                newExpiry = DateTime.UtcNow.AddDays(10);  // TODO: INTERMEDIATE SOLUTION
            }

            if ((0 == refValue.Length) || (0 == control.Length))
            {
                // a link from email is accessed...
                //
                if ((null == vo) || (vo.Deleted))
                {
                    // ... generate error page
                    //
                    makeHttpResponse(request, user, "HTML_GENERIC_ERROR", "@TXT_VIEWORDER_ABSENT", "---");
                }
                else
                {
                    // ... generate control page
                    //
                    makeHttpResponse(request, user, "HTML_VIEWORDER_CONTROL",
                        UniversalId.GenerateUrlId(UniversalId.IdType.ReverseRequest, dbRequest.Id),
                        dbRequest.ReferenceParamName,
                        "rid", dbRequest.ReferenceParamValue,
                        vo.ExpiresOn.ToLocalTime(), vo.Product, vo.Options,  // LEGACY: SERVER LOCAL TIME HERE!
                        Task.NotifyExpiringViewOrders.GetSubjectAddress(session, vo),
                        newExpiry.ToLocalTime());  // LEGACY: SERVER LOCAL TIME HERE!
                }
            }
            else
            {
                if ((testValue.Length > 0) || !dbRequest.ReferenceParamValue.Equals(refValue)) // TODO: robot action detected!
                {
                    ServiceInstances.Logger.Error(
                        "Robot activity detected (ViewOrder control).\r\nURL: {0}",
                        request.UserInfo);
                    makeHttpResponse(request, user, "HTML_GENERIC_ERROR", string.Empty, string.Empty);
                }
                else
                {
                    if (control.Equals("cancel"))
                    {
                        // remove reverse request record
                        using (ReverseRequestDao dao = new ReverseRequestDao(session))
                            dao.Delete(dbRequest);

                        // remove view order
                        vo.MarkDeleted();
                        using (ViewOrderDao dao = new ViewOrderDao(session))
                            dao.Update(vo);

                        // successful change callback: generate confirmation confirmation page
                        //
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Info("{0} User {1} successfully deleted VOID={2}.",
                            rn, user, vo.AutoID);

                        // prepare plaintext response to be parsed by javascript on page
                        //
                        makeJavascriptResponse(request, rn, "TXT_VIEWORDER_CANCELLED", user);

                        dataChanged = true;
                    }
                    else if (control.Equals("prolong"))
                    {
                        // remove reverse request record
                        using (ReverseRequestDao dao = new ReverseRequestDao(session))
                            dao.Delete(dbRequest);

                        // update view order
                        vo.Enabled = true;
                        vo.Prolong(newExpiry);
                        using (ViewOrderDao dao = new ViewOrderDao(session))
                            dao.Update(vo);

                        // successful change callback: generate confirmation confirmation page
                        //
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Info("{0} User {1} successfully prolonged VOID={2} until {3}.",
                            rn, user, vo.AutoID, vo.ExpiresOn);

                        // prepare plaintext response to be parsed by javascript on page
                        //
                        makeJavascriptResponse(request, rn, "TXT_VIEWORDER_PROLONGED", user);

                        dataChanged = true;
                    }
                }
            }

            return dataChanged;
        }
        #endregion
    }
}
