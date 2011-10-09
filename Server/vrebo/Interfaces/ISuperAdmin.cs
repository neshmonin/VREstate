namespace Vre.Server.BusinessLogic
{
    public interface ISuperAdmin : IDeveloperAdmin, IBuyer
    {
        bool AddEstateDeveloper(EstateDeveloper newDev);
    }
}

