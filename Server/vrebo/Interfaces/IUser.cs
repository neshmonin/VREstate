using System;
using System.ServiceModel;

namespace Vre.Server.BusinessLogic
{
    /// <summary>
    /// Type of user authentication.<br/>
    /// Possible future extension may include OpenID and other external logins.<br/>
    /// External logins store no password and are verified synchronously with external authentication providers.
    /// </summary>
    public enum LoginType : int
    {
        /// <summary>
        /// Plain autonomous login with password.
        /// </summary>
        Plain = 0
    }

    /// <summary>
    /// Result of data update requested by client.
    /// </summary>
    public enum DataUpdateResult : int
    {
        /// <summary>
        /// Changes successfully committed to database.
        /// </summary>
        Succeeded = 0,
        /// <summary>
        /// Unknown failure; try again later.
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Commit conflicted with concurent update; re-read data and try again.
        /// </summary>
        Conflicted = 2
    }

    /// <summary>
    /// Generic user services.  These services are available anonymously.
    /// </summary>
    [ServiceContract(SessionMode=SessionMode.Required)]
    public interface IUser : IDisposable
    {
        /// <summary>
        /// Create a new Buyer's login.
        /// </summary>
        /// <param name="developerId">Valid ID of estate developer this buyer is associated with.</param>
        /// <param name="type">Login type.</param>
        /// <param name="login">Login string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="password">Password string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="errorReason">Error description in case login create fails.</param>
        /// <returns>true if login is created.</returns>
        [OperationContract]
        bool CreateBuyer(string developerId, LoginType type, string login, string password, out string errorReason);

        /// <summary>
        /// Return a list of buildings by a Estate Devepoler.
        /// </summary>
        /// <param name="developerId">Valid ID of estate developer.</param>
        /// <returns>Array of buildings availabe from Estate Developer; null on error.</returns>
        [OperationContract]
        Building[] ListBuildings(string developerId);

        /// <summary>
        /// Return a list of suites for a building.
        /// </summary>
        /// <param name="building">Building to get suite list from.</param>
        /// <returns>Array of suites in the building; null on error.</returns>
        [OperationContract]
        Suite[] ListSuitesByBuiding(Building building);
    }
}

