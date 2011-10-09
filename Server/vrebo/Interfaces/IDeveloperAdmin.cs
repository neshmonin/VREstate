using System.Collections.Generic;
using System.ServiceModel;

namespace Vre.Server.BusinessLogic
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IDeveloperAdmin : ISubcontractor, ISalesperson
    {
        // User management

        /// <summary>
        /// List users belonging to current Estate Developer.
        /// </summary>
        /// <param name="role">User role to filter by.</param>
        /// <returns>A list of user objects or null in case of error.</returns>
        [OperationContract]
        User[] ListUsers(User.Role role);

        /// <summary>
        /// Create Subcontractor account.
        /// </summary>
        /// <param name="type">Login type.</param>
        /// <param name="login">Login string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="password">Password string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="errorReason">Error description in case login create fails.</param>
        /// <returns>true if login is created.</returns>
        [OperationContract]
        bool CreateSubcontractor(LoginType type, string login, string password, out string errorReason);
        /// <summary>
        /// Create Salesperson account.
        /// </summary>
        /// <param name="type">Login type.</param>
        /// <param name="login">Login string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="password">Password string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="errorReason">Error description in case login create fails.</param>
        /// <returns>true if login is created.</returns>
        [OperationContract]
        bool CreateSalesperson(LoginType type, string login, string password, out string errorReason);
        /// <summary>
        /// Create another Estate Developer Admin account.
        /// </summary>
        /// <param name="type">Login type.</param>
        /// <param name="login">Login string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="password">Password string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="errorReason">Error description in case login create fails.</param>
        /// <returns>true if login is created.</returns>
        [OperationContract]
        bool CreateAdmin(LoginType type, string login, string password, out string errorReason);
        /// <summary>
        /// Delete Subcontractor, Salesperson or another Estate Developer Admin.
        /// </summary>
        /// <param name="user">User object to delete</param>
        /// <param name="errorReason">Error description in case user delete fails.</param>
        /// <returns>true if operation succeeded.</returns>
        [OperationContract]
        bool DeleteUser(User user, out string errorReason);

        // Option management
        [OperationContract]
        IList<Option> ListOptions(bool enabledOnly);
        [OperationContract]
        void CreateOption(Option option);
        [OperationContract]
        void UpdateOption(Option option);

        //Option[] ListOptions(Building building);
        //Option[] ListOptionsByType(Building building, int type);
        //Option CreateOption(Building building, User provider, int type, string description);
        //bool EnableOption(Option option, Suite suite, bool enable, out string errorReason);
        //void SetNewPrice(Option option, long subcontractorPrice, long buyerPrice);
        //bool DeleteOption(Option option, out string errorReason);


        // Hit/miss stats
        // SalespersonWrapper stats

        // BuyerWrapper management
        [OperationContract]
        User GetBuyerDetails(User buyer);
        [OperationContract]
        void CancelTransaction(Transaction tran);
    }
}

