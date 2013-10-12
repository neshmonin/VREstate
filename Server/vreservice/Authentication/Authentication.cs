using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using Vre.Server.Dao;

namespace Vre.Server.BusinessLogic
{
    internal class Authentication : IAuthentication
    {
        private ISession _session;
        private bool _initiatedSession;

        public Authentication(ISession dbSession)
        {
            if (null == dbSession) { _session = NHibernateHelper.GetSession(); _initiatedSession = true; }
            else { _session = dbSession; _initiatedSession = false; }
        }

        public void Dispose()
        {
            if (_initiatedSession) { _session.Dispose(); _initiatedSession = false; }
        }

        private static bool validateLogin(string login, out string errorReason)
        {
            if (string.IsNullOrEmpty(login)) { errorReason = "Login cannot be empty"; return false; }
            if (string.IsNullOrWhiteSpace(login)) { errorReason = "Login cannot be whitespace"; return false; }
            
            if (login.Length < 4) { errorReason = "Login is too short"; return false; }
            // TODO: This constraint must relate to DB create script value!
            // 12 symbols are: 2 internal login construction service characters and 10 for estate developer ID (32-bit)
            if (login.Length > (64 - 12)) { errorReason = "Login is too long"; return false; }
            // See intLoginFromVisible() and loginElementsFromIntLogin() for more details
            if (!Char.IsLetterOrDigit(login[0])) { errorReason = "Login must start with a letter or digit"; return false; }
            //if (login.StartsWith("@")) { errorReason = "Login must start with a letter"; return false; }
            
            errorReason = null;
            return true;
        }

        private static bool validatePassword(string password, out string errorReason)
        {
            if (string.IsNullOrEmpty(password)) { errorReason = "Password cannot be empty"; return false; }
            if (string.IsNullOrWhiteSpace(password)) { errorReason = "Password cannot be whitespace"; return false; }

            if (password.Length < 4) { errorReason = "Password is too short"; return false; }
            // TODO: Should ever limit password length?!
            if (password.Length > 64) { errorReason = "Password is too long"; return false; }

            errorReason = null;
            return true;
        }

        public bool CreateLogin(LoginType type, User.Role role, int estateDeveloperId, string login, string password, 
            int userId, out string errorReason)
        {
            bool result = false;

            errorReason = null;

            if (type == LoginType.Plain)
            {
                if (!validateLogin(login, out errorReason)) return false;
                if (!validatePassword(password, out errorReason)) return false;
            }
            else
            {
                // TODO: Validate login/password pair against external services for external logins
            }
            
            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
            {
                using (CredentialsDao dao = new CredentialsDao(_session))
                {
                    string intLogin = intLoginFromVisible(role, estateDeveloperId, login);
                    Credentials item = dao.GetByLogin(type, intLogin);
                    if (null == item)
                    {
                        if (type == LoginType.Plain)
                            item = new Credentials(intLogin, password, userId, 
								Configuration.Security.PasswordHashType.Value,
								Configuration.Security.PasswordSaltSizeBytes.Value);
                        else
                            item = new Credentials(type, intLogin, userId);

                        dao.Create(item);
                        result = true;
                    }
                    else
                    {
                        errorReason = "Login already registered.";
                    }
                }
                if (result) tran.Commit();
            }

            return result;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword, out string errorReason)
        {
            bool result = false;

            if (!validatePassword(newPassword, out errorReason)) return false;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
            {
                using (CredentialsDao dao = new CredentialsDao(_session))
                {
                    Credentials item = dao.GetByUserId(userId);
                    if (null != item)
                    {
                        if (item.VerifyPassword(currentPassword))
                        {
                            item.SetPassword(
								Configuration.Security.PasswordHashType.Value,
								Configuration.Security.PasswordSaltSizeBytes.Value, 
								newPassword);
                            dao.Update(item);
                            result = true;
                        }
                        else
                        {
                            errorReason = "Current password is not valid.";
                        }
                    }
                    else
                    {
                        errorReason = "Login is not registered in the system.";
                    }
                }
                if (result) tran.Commit();
            }

            return result;
        }

        public bool ChangeLogin(int userId, string newLogin, out string errorReason)
        {
            bool result = false;

            if (!validateLogin(newLogin, out errorReason)) return false;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
            {
                using (CredentialsDao dao = new CredentialsDao(_session))
                {
                    Credentials item = dao.GetByUserId(userId);
                    if (null != item)
                    {
                        User.Role role;
                        int estateDeveloperId;
                        string login = null;

                        result = loginElementsFromIntLogin(item.Login, out role, out estateDeveloperId, out login);

                        string test = intLoginFromVisible(role, estateDeveloperId, newLogin);

                        if (null == dao.GetByLogin(LoginType.Plain, test))
                        {
                            item.Login = test;
                            dao.Update(item);
                            result = true;
                        }
                        else
                        {
                            errorReason = "New login is in use.";
                        }
                    }
                    else
                    {
                        errorReason = "Login is not registered in the system.";
                    }
                }
                if (result) tran.Commit();
            }

            return result;
        }

        public bool AuthenticateUser(LoginType type, User.Role role, int estateDeveloperId, string login, string password, 
            out int userId)
        {
            bool result = false;
            userId = -1;

            using (CredentialsDao dao = new CredentialsDao(_session))
            {
                Credentials item = dao.GetByLogin(type, intLoginFromVisible(role, estateDeveloperId, login));
                if (null != item)
                {
                    // TODO: Validate login/password pair against external services for external logins
                    result = item.VerifyPassword(password);
                    if (result) userId = item.UserId;
                }
            }

            return result;
        }

        public int UserIdByLogin(LoginType type, User.Role role, int estateDeveloperId, string login)
        {
            int result = -1;

            using (CredentialsDao dao = new CredentialsDao(_session))
            {
                Credentials item = dao.GetByLogin(type, intLoginFromVisible(role, estateDeveloperId, login));
                if (null != item) result = item.UserId;
            }

            return result;
        }

        public bool LoginByUserId(int userId, 
            out LoginType loginType, out User.Role role, out int estateDeveloperId, out string login)
        {
            bool result = false;

            loginType = LoginType.Plain;
            role = User.Role.Anonymous;
            estateDeveloperId = -1;
            login = null;

            using (CredentialsDao dao = new CredentialsDao(_session))
            {
                Credentials item = dao.GetByUserId(userId);
                if (item != null)
                {
                    loginType = item.Type;
                    result = loginElementsFromIntLogin(item.Login, out role, out estateDeveloperId, out login);
                }
            }

            return result;
        }

        public bool DropLogin(LoginType type, User.Role role, int estateDeveloperId, string login)
        {
            bool result = false;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
            {
                using (CredentialsDao dao = new CredentialsDao(_session))
                {
                    Credentials item = dao.GetByLogin(type, intLoginFromVisible(role, estateDeveloperId, login));
                    if (null != item)
                    {
                        dao.Delete(item);
                        result = true;
                    }
                }
                if (result) tran.Commit();
            }

            return result;
        }

        public bool DropLogin(int userId)
        {
            bool result = false;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
            {
                using (CredentialsDao dao = new CredentialsDao(_session))
                {
                    Credentials item = dao.GetByUserId(userId);
                    if (null != item)
                    {
                        dao.Delete(item);
                        result = true;
                    }
                }
                if (result) tran.Commit();
            }

            return result;
        }

        private static string intLoginFromVisible(User.Role role, int estateDeveloperId, string login)
        {
            switch (role)
            {
                case User.Role.SuperAdmin:
                    return login;

                case User.Role.DeveloperAdmin:
                    return string.Format("@{0}a{1}", estateDeveloperId, login);

                case User.Role.SalesPerson:
                    return string.Format("@{0}s{1}", estateDeveloperId, login);

                case User.Role.Subcontractor:
                    return string.Format("@{0}c{1}", estateDeveloperId, login);

                case User.Role.Buyer:
                    return string.Format("%{0}", login);

                case User.Role.SellingAgent:
                    return string.Format("#{0}", login);

				case User.Role.BuyingAgent:
					return string.Format("^{0}", login);

				case User.Role.Kiosk:
                    return string.Format("@{0}k{1}", estateDeveloperId, login);

                default:
                    throw new ArgumentException("Unknown user role");
            }
        }

        private static bool loginElementsFromIntLogin(string intLogin,
            out User.Role role, out int estateDeveloperId, out string login)
        {
            bool result = false;

            if (intLogin.StartsWith("@"))
            {
                int idx = 1;
                while ((idx < intLogin.Length) && (intLogin[idx] >= '0') && (intLogin[idx] <= '9')) idx++;

                role = User.Role.Buyer;
                estateDeveloperId = -1;
                login = null;

                if ((idx < intLogin.Length) && int.TryParse(intLogin.Substring(1, idx - 1), out estateDeveloperId))
                {
                    login = intLogin.Substring(idx + 1);
                    if ('a' == intLogin[idx]) { role = User.Role.DeveloperAdmin; result = true; }
                    else if ('s' == intLogin[idx]) { role = User.Role.SalesPerson; result = true; }
                    else if ('c' == intLogin[idx]) { role = User.Role.Subcontractor; result = true; }
                    else if ('k' == intLogin[idx]) { role = User.Role.Kiosk; result = true; }
                }
            }
            else if (intLogin.StartsWith("#"))
            {
                role = User.Role.SellingAgent;
                estateDeveloperId = -1;
                login = intLogin.Substring(1);
                result = true;
            }
			else if (intLogin.StartsWith("^"))
			{
				role = User.Role.BuyingAgent;
				estateDeveloperId = -1;
				login = intLogin.Substring(1);
				result = true;
			}
			else if (intLogin.StartsWith("%"))
            {
                role = User.Role.Buyer;
                estateDeveloperId = -1;
                login = intLogin.Substring(1);
                result = true;
            }
            else
            {
                role = User.Role.SuperAdmin;
                estateDeveloperId = -1;
                login = intLogin;
                result = true;
            }

            return result;
        }
    }

    internal class CredentialsDao : AbstractDisposableDao
    {
        public CredentialsDao() : base() {}
        public CredentialsDao(ISession session) : base(session) {}

        public void Create(Credentials entity)
        {
            lock (_session)
            {
                _session.Save(entity);
                if (_forcedFlush) _session.Flush();
            }
        }

        public void Update(Credentials entity)
        {
            lock (_session)
            {
                _session.Update(entity);
                if (_forcedFlush) _session.Flush();
            }
        }

        public void Delete(Credentials entity)
        {
            lock (_session)
            {
                _session.Delete(entity);
                if (_forcedFlush) _session.Flush();
            }
        }

        public Credentials GetByLogin(LoginType type, string login)
        {
            lock (_session)
            {
                return _session.CreateCriteria<Credentials>()
                    .Add(Restrictions.Eq("Type", type) && Restrictions.Eq("Login", login))
                    .UniqueResult<Credentials>();
            }
        }

        public Credentials GetByUserId(int userId)
        {
            lock (_session)
            {
                return _session.CreateCriteria<Credentials>()
                    .Add(Restrictions.Eq("UserId", userId))
                    .UniqueResult<Credentials>();
            }
        }
    }

    internal class Credentials
    {
        private int Id { get; set; }  // for the sake of good NHibernate handling, compound PKs are not good.
        public LoginType Type { get; private set; }
        public string Login { get; set; }
        public int UserId { get; private set; }
        private byte[] Password { get; set; }
        private byte[] Salt { get; set; }
        private string HashType { get; set; }
        public DateTime Created { get; private set; }
        public DateTime Updated { get; private set; }

        public Credentials() { }

        public Credentials(string login, string password, int userId, string hashType, int saltSize)
        {
            Type = LoginType.Plain;
            Login = login;
            UserId = userId;
            SetPassword(hashType, saltSize, password);
            Created = DateTime.UtcNow;
            Updated = Created;
        }

        public Credentials(LoginType type, string login, int userId)
        {
            Type = type;
            Login = login;
            UserId = userId;
            Created = DateTime.UtcNow;
            Updated = Created;
        }

        public bool VerifyPassword(string password)
        {
            if (Type != LoginType.Plain) throw new InvalidOperationException("Cannot verify password for external credentials.");

            if ((null == Password) || (null == Salt) || (0 == Password.Length) || (0 == Salt.Length))
                throw new ApplicationException("Password is not set.");

            HashAlgorithm hash = HashAlgorithm.Create(HashType);

            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] toHash = combinePasswordAndSalt(passwordBytes, Salt);

            byte[] toCompare = hash.ComputeHash(toHash);

            return compareArrays(toCompare, Password);
        }

        public void SetPassword(string hashType, int saltSize, string password)
        {
            if (Type != LoginType.Plain) throw new InvalidOperationException("Cannot set password for external credentials.");

            HashAlgorithm hash = HashAlgorithm.Create(hashType);

            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] salt = createSalt(saltSize);
            byte[] toHash = combinePasswordAndSalt(passwordBytes, salt);

            Password = hash.ComputeHash(toHash);
            Salt = salt;
            HashType = hashType;
            Updated = DateTime.UtcNow;
        }

        private static bool compareArrays(byte[] a, byte[] b)
        {
            if ((null == a) || (null == b)) return false;
            if (a.Length != b.Length) return false;
            for (int idx = a.Length - 1; idx >= 0; idx--)
                if (a[idx] != b[idx]) return false;
            return true;
        }

        private static byte[] combinePasswordAndSalt(byte[] password, byte[] salt)
        {
            Debug.Assert(((password != null) && (password.Length > 0)), "Password is invalid!");
            Debug.Assert(((salt != null) && (salt.Length > 0)), "Salt is invalid!");

            byte[] result = new byte[password.Length + salt.Length];

            Array.Copy(salt, 0, result, 0, salt.Length);
            Array.Copy(password, 0, result, salt.Length, password.Length);

            return result;
        }

        private static byte[] createSalt(int size)
        {
            Debug.Assert(((size > 16) && (size < 8192)), "Salt size is wrong!");
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] result = new byte[size];
            rng.GetBytes(result);
            return result;
        }
    }
}