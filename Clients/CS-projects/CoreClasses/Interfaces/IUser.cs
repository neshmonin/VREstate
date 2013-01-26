using System;
namespace VrEstate.Interfaces
{
    public enum UserRole
    {
        SuperAdmin,
        DeveloperAdmin,
        Buyer,
        Saleseperson,
        Builder
    }

    interface IUser
    {
        IVrCredentials Credentials { get; set; }
        string Id { get; }
        UserRole Role { get; set; }

        IVrSession Login();
        void Logout(IVrSession session);
    }
}
