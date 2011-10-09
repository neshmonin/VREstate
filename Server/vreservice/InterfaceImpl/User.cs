using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Vre.Server.BusinessLogic
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession, ConcurrencyMode=ConcurrencyMode.Multiple)]
    public partial class UserInterface : IUser
    {

        public UserInterface()
        {
        }

        /// <summary>
        /// Role operations restriction logic.  Extending classes should override this to prevent improper users from using extensions.
        /// </summary>
        internal virtual bool IsAllowed(User.Role role)
        {
            return true;
        }

        internal bool IsAllowed()
        {
            if (null == _user) return false;
            return IsAllowed(_user.UserRole);
        }

        internal RemoteEndpointMessageProperty setupContext(bool engageFloodStop)
        {
            OperationContext context;
            return setupContext(engageFloodStop, out context);
        }

        internal RemoteEndpointMessageProperty setupContext(bool engageFloodStop, out OperationContext context)
        {
            context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint =
                prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            if (engageFloodStop) ServiceInstances.FloodStopper.UpdatePeer(endpoint.Address);

            //if (logEndpoint)
            ServiceInstances.Logger.Info("{0}: {1}:{2} attached.", context.SessionId, endpoint.Address, endpoint.Port);

            return endpoint;
        }

        // This method is mainly used on client side to dispose of connection properly.
        public void Dispose()
        {
            dropDbSession();

            if (_user != null)
            {
                _user = null;
                _login = null;
            }
        }

        #region database session control
        private readonly object _dbSessionLock = new object();
        protected NHibernate.ISession _dbSession = null;

        protected void getDbSession()
        {
            lock (_dbSessionLock)
            {
                if (null == _dbSession) _dbSession = NHibernateHelper.GetSession();
            }
        }

        protected void dropDbSession()
        {
            lock (_dbSessionLock)
            {
                if (_dbSession != null)
                {
                    _dbSession.Close();
                    _dbSession.Dispose();
                    _dbSession = null;
                }
            }
        }
        #endregion

        #region managers retrieval
        internal UserManager getUserManager() { return new UserManager(_dbSession); }
        #endregion

        #region user session state control
#if DEBUG
        protected string _sessionId = null;
#endif
        protected LoginType _loginType;
        protected string _login = null;
        public User _user = null;

        public bool Login(LoginType type, string login, string password, out string errorReason)
        {
            if (_user != null) { errorReason = "Already logged in.";  return false; }

            bool result = false;

            try
            {
#if DEBUG
                OperationContext context;
                setupContext(true, out context);
                ServiceInstances.Logger.Info("Client login attempt: uid={0}[{1}].", type, login);
                _sessionId = context.SessionId;
#else
                setupContext(true);
                ServiceInstances.Logger.Info("Client login attempt: uid={0}[...].", type);
#endif
                using (UserManager um = getUserManager())
                {
                    _user = um.Login(type, login, password);

                    if (_user != null)
                    {
                        if (IsAllowed(_user.UserRole))
                        {
                            _loginType = type;
                            _login = login;
                            getDbSession();
                            result = true;
                            errorReason = null;
                        }
                        else
                        {
                            _user = null;
                            ServiceInstances.Logger.Error("Error logging user in: this login is not allowed to use this interface.");
                            errorReason = "This login is not allowed to use this interface.";
                        }
                    }
                    else
                    {
                        ServiceInstances.Logger.Error("Error logging user in: unknown username or bad password.");
                        errorReason = "Unknown login or bad password.";
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Error logging user in: {0}", ex);
                errorReason = "Operation failed.  Try again later.";
            }

            return result;
        }

        public bool ChangePassword(string currentPassword, string newPassword, out string errorReason)
        {
            if (null == _user)
            {
                errorReason = "Not logged in.";
                return false;
            }

            bool result = false;

            try
            {
#if DEBUG
                OperationContext context;
                setupContext(true, out context);
                Debug.Assert(context.SessionId.Equals(_sessionId), "Session ID changed!");
#else
                setupContext(false);
#endif
                ServiceInstances.Logger.Info("Client password change attempt: uid={0}[{1}].", _loginType, _login);

                using (UserManager um = getUserManager())
                {
                    if (um.ChangePassword(_loginType, _login, currentPassword, newPassword, out errorReason))
                    {
                        errorReason = null;
                        result = true;
                    }
                    else
                    {
                        ServiceInstances.Logger.Error("Client password change for {0}[{1}] failed!", _loginType, _login);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Error changing password for {0}[{1}]: {2}", _loginType, _login, ex);
                errorReason = "Operation failed (0).  Try again later.";
            }

            return result;
        }

        public void Logout()
        {
            if (null == _user) return;

#if DEBUG
            OperationContext context;
            setupContext(false, out context);
            Debug.Assert(context.SessionId.Equals(_sessionId), "Session ID changed!");
#else
            setupContext(false);
#endif

            ServiceInstances.Logger.Info("Client logout request: uid={0}[{1}].", _loginType, _login);

            doLogout();
        }

        internal void doLogout()
        {
            dropDbSession();

            _user = null;
            _login = null;
        }
        #endregion

        #region create/control
        public bool CreateBuyer(string developerId, LoginType type, string login, string password, out string errorReason)
        {
            try
            {
                setupContext(true);

                int estateDeveloperId;
                if (!int.TryParse(developerId, out estateDeveloperId))
                {
                    errorReason = "Developer ID format is invalid.";
                    return false;
                }
#if DEBUG
                ServiceInstances.Logger.Info("Login create attempt: uid={0}[{1}]; did={2}.", type, login, estateDeveloperId);
#else
                ServiceInstances.Logger.Info("Login create attempt: uid={0}[...]; did={1}.", type, estateDeveloperId);
#endif
                using (UserManager um = getUserManager())
                {
                    return um.CreateUser(User.Role.Buyer, estateDeveloperId, type, login, password, out errorReason);
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Login create failed (1): {0}.", ex);
                errorReason = "Operation failed (1).  Try again later.";
                return false;
            }
        }

        public bool DeleteMyRecord(out string errorReason)
        {
            if (null == _user) { errorReason = "Not logged in."; return false; }
            setupContext(false);  // TODO: insert flood preventing?

            bool result = false;

            try
            {
                // ROLE PERMISSION LOGIC
                if ((_user.UserRole == User.Role.Buyer) ||
                    (_user.UserRole == User.Role.DeveloperAdmin) ||
                    (_user.UserRole == User.Role.SuperAdmin))
                {
                    using (UserManager um = getUserManager()) result = um.DeleteUser(_user, out errorReason);
                }
                else
                {
                    errorReason = "You are not allowed to delete your record.";
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Login delete failed (1): {0}.", ex);
                errorReason = "Operation failed (1).  Try again later.";
            }

            return result;
        }

        public ContactInfo GetMyContactInfo()
        {
            if (null == _user) return null;
            setupContext(false);  // TODO: insert flood preventing?

            return _user.PersonalInfo;
        }

        public DataUpdateResult UpdateMyContactInfo(ContactInfo info)
        {
            if (null == _user) return DataUpdateResult.Failed;
            setupContext(false);  // TODO: insert flood preventing?

            DataUpdateResult result = DataUpdateResult.Failed;

            try
            {
                if (info != null)
                {
                    _user.UpdatePersonalInfo(info);
                    using (UserManager um = getUserManager()) um.UpdateUser(_user);
                    result = DataUpdateResult.Succeeded;
                }
            }
            catch (NHibernate.StaleObjectStateException)
            {
                ServiceInstances.Logger.Error("Contact info update failed (conflict).");
                result = DataUpdateResult.Conflicted;
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Contact info update failed (0): {0}.", ex);
            }

            return result;
        }
        #endregion

        #region building/suite browsing
        public Building[] ListBuildings(string developerId)
        {
            if (null == _user) return null;
            setupContext(false);  // TODO: insert flood preventing?
            
            int estateDeveloperId;
            if (!int.TryParse(developerId, out estateDeveloperId))
            {
                ServiceInstances.Logger.Error("ListBuildings: Developer ID format is invalid.");
                return null;
            }

            BuildingManager bm = new BuildingManager(_dbSession);
            return bm.ListBuildings(estateDeveloperId);
        }

        public Suite[] ListSuitesByBuiding(Building building)
        {
            if (null == _user) return null;
            setupContext(false);  // TODO: insert flood preventing?

            BuildingManager bm = new BuildingManager(_dbSession);
            return bm.ListSuitesByBuiding(building);
        }
        #endregion



    }


    /*
    class CustomClientCredentials : ClientCredentials
    {
        public CustomClientCredentials()
            : base()
        {
        }

        protected CustomClientCredentials(ClientCredentials other)
            : base(other)
        {
        }

        protected override ClientCredentials CloneCore()
        {
            return new CustomClientCredentials(this);
        }

        /// <summary>
        /// Returns a custom security token manager
        /// </summary>
        /// <returns></returns>
        public override System.IdentityModel.Selectors.SecurityTokenManager CreateSecurityTokenManager()        
        {
            return new CustomClientCredentialsSecurityTokenManager(this);
        }
    }
    
    class CustomClientCredentialsSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        private static Dictionary<Uri, CustomIssuedSecurityTokenProvider> providers = new Dictionary<Uri, CustomIssuedSecurityTokenProvider>();

        public CustomClientCredentialsSecurityTokenManager(ClientCredentials credentials)
            : base(credentials)
        {
        }

        /// <summary>
        /// Returns a custom token provider when a issued token is required
        /// </summary>
        public override System.IdentityModel.Selectors.SecurityTokenProvider CreateSecurityTokenProvider(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement)
        {
            if (this.IsIssuedSecurityTokenRequirement(tokenRequirement))
            {
                IssuedSecurityTokenProvider baseProvider = (IssuedSecurityTokenProvider)base.CreateSecurityTokenProvider(tokenRequirement);
                CustomIssuedSecurityTokenProvider provider = new CustomIssuedSecurityTokenProvider(baseProvider);
                return provider;
            }
            else
            {
                return base.CreateSecurityTokenProvider(tokenRequirement);
            }
        }
    } 

    /// <summary>
    /// Helper class used as cache for security tokens
    /// </summary>
    class TokenCache
    {
        private const int DefaultTimeout = 1000;
        private static Dictionary<Uri, SecurityToken> tokens = new Dictionary<Uri, SecurityToken>();
        private static ReaderWriterLock tokenLock = new ReaderWriterLock();

        private TokenCache()
        {
        }

        public static SecurityToken GetToken(Uri endpoint)
        {
            SecurityToken token = null;
            tokenLock.AcquireReaderLock(DefaultTimeout);
       
            try
            {
                tokens.TryGetValue(endpoint, out token);
                return token;
            }
            finally
            {
                tokenLock.ReleaseReaderLock();
            }
        }

        public static void AddToken(Uri endpoint, SecurityToken token)
        {
            tokenLock.AcquireWriterLock(DefaultTimeout);

            try
            {
                if (tokens.ContainsKey(endpoint)) tokens.Remove(endpoint);
                tokens.Add(endpoint, token);
            }
            finally
            {
                tokenLock.ReleaseWriterLock();
            }
        }
    }

 

    /// <summary>
    /// Custom token provider. This class keeps the tokens outside of the channel
    /// so they can be reused
    /// </summary>
    class CustomIssuedSecurityTokenProvider : IssuedSecurityTokenProvider
    {
        private IssuedSecurityTokenProvider innerProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomIssuedSecurityTokenProvider(IssuedSecurityTokenProvider innerProvider)
            : base()
        {
            this.innerProvider = innerProvider;
            this.CacheIssuedTokens = innerProvider.CacheIssuedTokens;
            this.IdentityVerifier = innerProvider.IdentityVerifier;
            this.IssuedTokenRenewalThresholdPercentage = innerProvider.IssuedTokenRenewalThresholdPercentage;
            this.IssuerAddress = innerProvider.IssuerAddress;
            this.IssuerBinding = innerProvider.IssuerBinding;
            foreach (IEndpointBehavior behavior in innerProvider.IssuerChannelBehaviors)
            {
                this.IssuerChannelBehaviors.Add(behavior);
            }
            this.KeyEntropyMode = innerProvider.KeyEntropyMode;
            this.MaxIssuedTokenCachingTime = innerProvider.MaxIssuedTokenCachingTime;
            this.MessageSecurityVersion = innerProvider.MessageSecurityVersion;
            this.SecurityAlgorithmSuite = innerProvider.SecurityAlgorithmSuite;
            this.SecurityTokenSerializer = innerProvider.SecurityTokenSerializer;
            this.TargetAddress = innerProvider.TargetAddress;
            foreach (XmlElement parameter in innerProvider.TokenRequestParameters)
            {
                this.TokenRequestParameters.Add(parameter);
            }
            this.innerProvider.Open();
        }

        /// <summary>
        /// Gets the security token
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        protected override System.IdentityModel.Tokens.SecurityToken GetTokenCore(TimeSpan timeout)
        {
            SecurityToken securityToken = null;

            if (this.CacheIssuedTokens)
            {
                securityToken = TokenCache.GetToken(this.innerProvider.IssuerAddress.Uri);

                if (securityToken == null || !IsServiceTokenTimeValid(securityToken))
                {
                    securityToken = innerProvider.GetToken(timeout);
                    TokenCache.AddToken(this.innerProvider.IssuerAddress.Uri, securityToken);
                }
            }
            else
            {
                securityToken = innerProvider.GetToken(timeout);
            }

            return securityToken;
        }

        /// <summary>
        /// Checks the token expiration.
        /// A more complex algorithm can be used here to determine whether the token is valid or not.
        /// </summary>
        private bool IsServiceTokenTimeValid(SecurityToken serviceToken)
        {
            return (DateTime.UtcNow <= serviceToken.ValidTo.ToUniversalTime());
        }

        ~CustomIssuedSecurityTokenProvider()
        {
            this.innerProvider.Close();
        }
    }
    */
}