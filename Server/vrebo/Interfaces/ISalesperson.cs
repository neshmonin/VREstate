using System.ServiceModel;
namespace Vre.Server.BusinessLogic
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ISalesperson : IAuthenticatedUser
    {
    }
}

