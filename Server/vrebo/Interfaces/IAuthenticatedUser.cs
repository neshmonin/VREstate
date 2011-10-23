using System;
using System.ServiceModel;

namespace Vre.Server.BusinessLogic
{
    /// <summary>
    /// Generic user services.  These services are available to all types of users.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IAuthenticatedUser : IUser
    {
        /// <summary>
        /// Returns current contact information of logged in user.
        /// </summary>
        /// <returns>Contact information object or null on error.</returns>
        [OperationContract]
        ContactInfo GetMyContactInfo();

        /// <summary>
        /// Updates current logged in user's contact information.
        /// </summary>
        /// <param name="info">Contact information object; the object must be obtained via a call to <see cref="GetMyContactInfo"/>.</param>
        /// <returns>Data update result.</returns>
        [OperationContract]
        DataUpdateResult UpdateMyContactInfo(ContactInfo info);

        /// <summary>
        /// Deletes a record of currenly logged on user from system.
        /// </summary>
        /// <param name="errorReason">Error description in case login delete fails.</param>
        /// <returns>true if operation succeeded.</returns>
        [OperationContract]
        bool DeleteMyRecord(out string errorReason);

        /// <summary>
        /// Login to system.
        /// </summary>
        /// <param name="type">Login type.</param>
        /// <param name="login">Login string.</param>
        /// <param name="password">Password string.</param>
        /// <param name="errorReason">Error description in case login fails.</param>
        /// <returns>true if user is authenticated.</returns>
        [OperationContract]
        bool Login(LoginType type, string login, string password, out string errorReason);

        /// <summary>
        /// Change password for currently logged on user.
        /// </summary>
        /// <param name="currentPassword">Current password string.</param>
        /// <param name="newPassword">New password string.  Must be non-null, non-empty, 4-64 symbols long.</param>
        /// <param name="errorReason">Error description in case password change fails.</param>
        /// <returns>true if password was changed.</returns>
        [OperationContract]
        bool ChangePassword(string currentPassword, string newPassword, out string errorReason);

        /// <summary>
        /// Logout and close server session.
        /// </summary>
        [OperationContract(IsTerminating = true)]
        void Logout();
    }
}

