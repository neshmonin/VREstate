using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.RemoteService
{
    internal class ReverseRequestService : IHttpService
    {
        private static bool _configured = false;
        private static bool _allowUnsecureService = true;
        
        private static TimeSpan _linkExpirationTime;

        private static string _smtpServerHost;
        private static int _smtpServerPort;
        private static bool _smtpUseSsl;
        private static string _smtpLogin;
        private static string _smtpPassword;
        private static string _viewOrderUrlTemplate;

        private static Dictionary<string, string> _messageTemplates = new Dictionary<string, string>();

        private const string _servicePathPrefix = _servicePathElement0 + "/";
        private const string _servicePathElement0 = "go";
        private const int _maxRequestLinkLength = 64;  // the link is currently a GUID

        public string ServicePathPrefix { get { return _servicePathPrefix; } }

        public bool RequiresSession { get { return false; } }

        private static void configure()
        {
            _allowUnsecureService = ServiceInstances.Configuration.GetValue("AllowSensitiveDataOverNonSecureConnection", false);

            _smtpServerHost = ServiceInstances.Configuration.GetValue("SmtpServerHost", string.Empty);
            _smtpServerPort = ServiceInstances.Configuration.GetValue("SmtpServerPort", 25);
            _smtpLogin = ServiceInstances.Configuration.GetValue("SmtpServerLogin", string.Empty);
            _smtpPassword = ServiceInstances.Configuration.GetValue("SmtpServerPassword", string.Empty);
            _smtpUseSsl = ServiceInstances.Configuration.GetValue("SmtpServerUseSsl", false);

            _linkExpirationTime = new TimeSpan(0, 0, ServiceInstances.Configuration.GetValue("ReversRequestLinkExpirationSec", 3600));

            _viewOrderUrlTemplate = ServiceInstances.Configuration.GetValue("ViewOrderUrlTemplate", "http://ref.3dcondox.com/go?id={0}");

            scanTemplates(ServiceInstances.Configuration.GetValue("MessageTemplateRoot", "."));

            _configured = true;
        }

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
            if (!_configured) configure();

            if (!_allowUnsecureService && !request.Request.IsSecureConnection)
                throw new PermissionException("Service available only over secure connection.");
            
            string path = request.Request.Path;
            int pos = path.IndexOf('/');
            if ((pos > 0) && (pos < path.Length)) path = path.Substring(pos + 1);

            if (path.Length > _maxRequestLinkLength)
                throw new ArgumentException();

            Guid rqid;
            if (!Guid.TryParseExact(path, "N", out rqid))
                throw new ArgumentException();

            using (ISession session = NHibernateHelper.GetSession())
            {
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
                {
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
                        if (rq.IsAttemptSensitive)
                        {
                            // TODO: Prevent brute-force
                        }

                        switch (rq.Request)
                        {
                            case ReverseRequest.RequestType.AccountRegistration:
                                processAccountRegister(request, rq, session);
                                break;

                            case ReverseRequest.RequestType.LoginChange:
                                processLoginChange(request, rq, session);
                                break;

                            //case ReverseRequest.RequestType.Listing:
                            //    processListingAccess(request, rq, session);
                            //    break;

                            // TODO: the rest of requests

                            default:
                                string rn = Utilities.GenerateReferenceNumber();
                                ServiceInstances.Logger.Error("{0} Reverse request type is not known: {1}", rn, rq.Request);
                                makeHttpResponse(request, null, "HTML_GENERIC_ERROR",
                                    "Unknown request", rn);
                                break;
                        }
                    }

                    tran.Commit();
                }
            }
        }

        #region utility methods
        #region message sending
        private static void sendMessage(User user, string templateName, params object[] parameters)
        {
            string template = getTemplate(templateName, user);

            sendMessageInt(user.PrimaryEmailAddress, template, parameters);

            ServiceInstances.Logger.Info("{0} message sent to {1} ({2})", templateName, user.PrimaryEmailAddress, user.AutoID);
        }

        private static void sendMessage(User user, string recipient, string templateName, params object[] parameters)
        {
            string template = getTemplate(templateName, user);

            sendMessageInt(recipient, template, parameters);

            ServiceInstances.Logger.Info("{0} message sent to {1} ({2})", 
                templateName, recipient, (user != null) ? user.AutoID : -1);
        }

        private static void sendMessage(string recipient, string templateName, params object[] parameters)
        {
            string template = getTemplate(templateName, null);

            sendMessageInt(recipient, template, parameters);

            ServiceInstances.Logger.Info("{0} message sent to {1}", templateName, recipient);
        }

        private static void sendMessageInt(string recipient, string template, params object[] parameters)
        {
            string message;

            // Process placeholders
            //
            message = string.Format(template, parameters);

            // Detach subject from body: subject is first line
            //
            int pos1 = message.IndexOf('\r');
            int pos2 = pos1;
            if ((pos1 < 0) || (pos1 >= message.Length)) { pos1 = message.IndexOf('\n'); pos2 = pos1; }
            else if ((pos2 < (message.Length - 1)) && (message[pos2 + 1] == '\n')) { pos2++; }

            string subject = VersionGen.ProductName;  // default, should never appear though! :)

            if ((pos1 >= 0) && (pos1 < message.Length))
            {
                subject = message.Substring(0, pos1);
                message = message.Substring(pos2 + 1);
            }

            // Send ready message
            //
            try
            {
                using (SmtpClient client = new SmtpClient(_smtpServerHost, _smtpServerPort))
                {
                    client.Credentials = new NetworkCredential(_smtpLogin, _smtpPassword);
                    client.EnableSsl = _smtpUseSsl;
                    client.Send(_smtpLogin, recipient, subject, message);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cannot send message to recipient: " + ex.Message, ex);
            }
        }
        #endregion

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

        #region template management
        private static void scanTemplates(string rootPath)
        {
            foreach (string file in 
                Directory.GetFiles(Path.Combine(rootPath, "en"), "*.*", SearchOption.TopDirectoryOnly))
            {
                if (!Path.GetExtension(file).Equals(".txt", StringComparison.InvariantCultureIgnoreCase)) continue;

                string name = Path.GetFileNameWithoutExtension(file);
                string value;
                using (StreamReader sr = File.OpenText(file))
                    value = sr.ReadToEnd();

                _messageTemplates.Add(name.ToUpperInvariant(), processTemplate(value));
            }

            // TODO: Hardcoded!
            // Non-localized text templates
            _messageTemplates.Add("TXT_ACCOUNT_CREATED", "Account has been successfully created.");
            _messageTemplates.Add("TXT_PASSWORD_UPDATED", "Password has been successfully updated.");
            _messageTemplates.Add("TXT_LOGIN_CHANGED", "Login has been successfully changed.");
        }

        private static readonly char[] _formatEscape = new char[] { '{', '}' };
        private static string processTemplate(string template)
        {
            Stack<bool> decisions = new Stack<bool>();
            int pos = 0;
            do
            {
                pos = template.IndexOfAny(_formatEscape, pos);
                if (pos < 0) break;
                bool escape;
                if (template[pos] == '{')
                {
                    if (pos < (template.Length - 1)) escape = !char.IsDigit(template, pos + 1);// char.IsWhiteSpace(template, pos + 1);
                    else escape = true;

                    decisions.Push(escape);
                    if (escape) template = template.Insert(pos++, "{");
                }
                else
                {
                    if (decisions.Pop()) template = template.Insert(pos++, "}");
                }
                pos++;
            }
            while (true);
            return template;
        }

        private static string getTemplate(string name, User relatedUser)
        {
            string result;

            if (!_messageTemplates.TryGetValue(name, out result))
                throw new ApplicationException("Template name is unknown: " + name);

            return result;
        }
        #endregion

        private static void makeHttpResponse(IServiceRequest request, User user, string templateName, params object[] parameters)
        {
            string template = getTemplate(templateName, user);
            try
            {
                string html = string.Format(template, parameters);

                request.Response.ResponseCode = HttpStatusCode.OK;
                request.Response.DataStreamContentType = "html";

                // cannot use a StreamWriter here as it disposes off the stream after use!
                byte[] binary = Encoding.UTF8.GetBytes(html);
                request.Response.DataStream.Write(binary, 0, binary.Length);
            }
            catch (Exception e)
            {
                ServiceInstances.Logger.Fatal(template);
                throw;
            }
        }

        private static void makeJavascriptResponse(IServiceRequest request, string refNum, string textTemplateName, User relatedUser)
        {
            request.Response.ResponseCode = HttpStatusCode.OK;
            request.Response.DataStreamContentType = "txt";

            // cannot use a StreamWriter here as it disposes off the stream after use!
            byte[] binary = Encoding.UTF8.GetBytes(string.Format("{0}|{1}", refNum, getTemplate(textTemplateName, relatedUser)));
            request.Response.DataStream.Write(binary, 0, binary.Length);
        }
        #endregion

        #region Account registration
        public static void InitiateUserRegistration(IRequestData srq, ClientSession session, string login)
        {
            if (!_configured) configure();

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
                        request = dao.Get(login, ReverseRequest.RequestType.AccountRegistration);

                        if (null == request)
                        {
                            request = ReverseRequest.CreateRegisterAccount(login, null,
                                DateTime.UtcNow.Add(_linkExpirationTime),
                                generateReferenceParameterName(), generateReferenceParameterValue());

                            dao.Create(request);
                        }
                        else
                        {
                            request.ExpiresOn = DateTime.UtcNow.Add(_linkExpirationTime);
                            dao.Update(request);
                        }
                    }

                    sendMessage(null, login, "MSG_ACCOUNT_CREATE",
                        srq.ConstructClientRootUri() + _servicePathPrefix + request.Id.ToString("N"), request.ExpiresOn);

                    //session.DbSession.Flush();
                    tran.Commit();
                }
            }
        }

        private static void processAccountRegister(IServiceRequest request, ReverseRequest dbRequest, ISession session)
        {
            string testValue = request.Request.Query.GetParam(dbRequest.ReferenceParamName, string.Empty);
            string refValue = request.Request.Query.GetParam("rid", string.Empty);
            string password = request.Request.Query.GetParam("pwd", string.Empty);
            User user;

            if ((0 == refValue.Length) || (0 == password.Length))
            {
                // a link from email is accessed: generate password entry page
                //
                makeHttpResponse(request, null, "HTML_PASSWORD_ENTRY",
                    dbRequest.Id.ToString("N"),
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
                    user.LastLogin = NHibernateHelper.DateTimeMinValue;

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
                    }
                    else
                    {
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Error("{0} Login create error: {1}", rn, errorReason ?? "Authentication error");
                        throw new InvalidOperationException(errorReason ?? "Authentication error");
                    }
                }
            }
        }
        #endregion

        #region Login change
        public static void InitiateLoginChange(IRequestData srq, ClientSession session, string newLogin)
        {
            if (!_configured) configure();

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
                    request = dao.Get(newLogin, ReverseRequest.RequestType.LoginChange);

                    if (null == request)
                    {
                        request = ReverseRequest.CreateLoginChange(session.User.AutoID, newLogin,
                            DateTime.UtcNow.Add(_linkExpirationTime),
                            generateReferenceParameterName(), generateReferenceParameterValue());

                        dao.Create(request);
                    }
                    else
                    {
                        request.ExpiresOn = DateTime.UtcNow.Add(_linkExpirationTime);
                        dao.Update(request);
                    }
                }

                sendMessage(session.User, newLogin, "MSG_LOGIN_CHANGE",
                    srq.ConstructClientRootUri() + _servicePathPrefix + request.Id.ToString("N"), request.ExpiresOn);

                //session.DbSession.Flush();  // TODO: dunno why this is required!
                tran.Commit();
            }
        }

        private static void processLoginChange(IServiceRequest request, ReverseRequest dbRequest, ISession session)
        {
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
                    dbRequest.Id.ToString("N"),
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
                    }
                    else
                    {
                        string rn = Utilities.GenerateReferenceNumber();
                        ServiceInstances.Logger.Error("{0} Login change error: {1}", rn, errorReason ?? "Authentication error");
                        throw new InvalidOperationException(errorReason ?? "Authentication error");
                    }
                }
            }
        }
        #endregion

        #region ViewOrder
        public static string CreateViewOrder(IServiceRequest srq, int targetUserId, string note,
            ViewOrder.ViewOrderType product, string mlsId, string mlsUrl,
            ViewOrder.SubjectType type, int targetObjectId, string productUrl, DateTime expiresOn,
            string paymentSystemRefId)
        {
            if (!_configured) configure();

            string result = null;
            User targetUser;
            ViewOrder viewOrder;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(srq.UserInfo.Session.DbSession))
            {
                using (UserDao dao = new UserDao(srq.UserInfo.Session.DbSession))
                    targetUser = dao.GetById(targetUserId);

                if (null == targetUser) targetUser = srq.UserInfo.Session.User;  // if unknown/not specified - make a view order for caller

                RolePermissionCheck.CheckCreateViewOrder(srq.UserInfo.Session, targetUser);

                using (ViewOrderDao dao = new ViewOrderDao(srq.UserInfo.Session.DbSession))
                    viewOrder = dao.Get(targetUser.AutoID, type, targetObjectId);

                if (null == viewOrder)
                {
                    viewOrder = new ViewOrder(targetUser.AutoID, 
                        product, mlsId, type, targetObjectId, productUrl, expiresOn);

                    viewOrder.MlsUrl = mlsUrl;
                    viewOrder.Note = note;

                    using (ViewOrderDao dao = new ViewOrderDao(srq.UserInfo.Session.DbSession))
                        dao.Create(viewOrder);

                    DataService.ReflectViewOrderStatusInTarget(viewOrder, srq.UserInfo.Session.DbSession);

                    // Generate financial transaction
                    //
                    FinancialTransaction ft = new FinancialTransaction(srq.UserInfo.Session.User.AutoID,
                        FinancialTransaction.AccountType.User, targetUser.AutoID,
                        FinancialTransaction.OperationType.Debit, 0m,
                        FinancialTransaction.TranSubject.View,
                        FinancialTransaction.TranTarget.Suite, targetObjectId, viewOrder.AutoID.ToString());

                    if (!string.IsNullOrWhiteSpace(paymentSystemRefId))
                        ft.SetPaymentSystemReference(FinancialTransaction.PaymentSystemType.CondoExplorer, paymentSystemRefId);

                    using (FinancialTransactionDao dao = new FinancialTransactionDao(srq.UserInfo.Session.DbSession))
                    {
                        dao.Create(ft);
                        ft.SetAutoSystemReferenceId();
                        dao.Update(ft);
                    }

                    result = ft.SystemRefId;
                }
                else
                {
                    throw new ObjectExistsException("View Order already exists.");

                    //viewOrder.Update(product, mlsId, productUrl, expiresOn);

                    //using (ViewOrderDao dao = new ViewOrderDao(srq.UserInfo.Session.DbSession))
                    //    dao.Update(viewOrder);

                    //result = Utilities.GenerateReferenceNumber();
                }

                srq.Response.ResponseCode = HttpStatusCode.OK;
                srq.Response.Data = new ClientData();
                srq.Response.Data.Add("viewOrder-url", ConstructViewOrderUrl(viewOrder));
                srq.Response.Data.Add("viewOrder-id", viewOrder.AutoID);
                // TODO: generate button into listing response
                //srq.Response.Data.Add("button-url", string.Format(_listingUrlTemplate, request.Id.ToString("N")));
                srq.Response.Data.Add("ref", result);

                tran.Commit();
            }

            ServiceInstances.Logger.Info(
                "User {0} created a view order for user {1}: ({2}) for {3} id={4}, reference URL={5}, expires on {6}, RID={7}, STRN={8}",
                srq.UserInfo.Session.User, targetUser, product, type, targetObjectId, productUrl, expiresOn, viewOrder.AutoID, result);

            return result;
        }

        public static string ConstructViewOrderUrl(ViewOrder viewOrder)
        {
            if (!_configured) configure();

            return string.Format(_viewOrderUrlTemplate, viewOrder.AutoID.ToString("N"));
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
        #endregion
    }
}
