using System;

namespace Vre.Server.BusinessLogic
{
    /// <summary>
    /// This interface is strictly for in-assembly use; there's no added security for exposure.
    /// </summary>
    internal interface IAuthentication : IDisposable
    {
        bool CreateLogin(LoginType type, string login, string password, int userId, out string errorReason);
        bool ChangePassword(LoginType type, string login, string currentPassword, string newPassword, out string errorReason);
        bool AuthenticateUser(LoginType type, string login, string password, out int userId);
        bool DropLogin(LoginType type, string login);
        bool DropLogin(int userId);
        int UserIdByLogin(LoginType type, string login);
    }
}