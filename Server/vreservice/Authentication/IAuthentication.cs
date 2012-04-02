using System;

namespace Vre.Server.BusinessLogic
{
    /// <summary>
    /// This interface is strictly for in-assembly use; there's no added security for exposure.
    /// </summary>
    internal interface IAuthentication : IDisposable
    {
        bool CreateLogin(LoginType type, User.Role role, int estateDeveloperId, string login, string password, int userId, out string errorReason);
        bool ChangePassword(int userId, string currentPassword, string newPassword, out string errorReason);
        bool AuthenticateUser(LoginType type, User.Role role, int estateDeveloperId, string login, string password, out int userId);
        bool DropLogin(LoginType type, User.Role role, int estateDeveloperId, string login);
        bool DropLogin(int userId);
        int UserIdByLogin(LoginType type, User.Role role, int estateDeveloperId, string login);
        bool LoginByUserId(int userId, out LoginType loginType, out User.Role role, out int estateDeveloperId, out string login);
    }
}