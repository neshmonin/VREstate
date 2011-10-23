using System;
using Vre.Server.BusinessLogic;
using System.ServiceModel;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ISubcontractor : IAuthenticatedUser
    {
        [OperationContract]
        IList<Option> ListOptionsByBuilding(int buildingId);
        [OperationContract]
        IList<Option> ListOptionsByBuildingAndSuite(int buildingId, int suiteId);

        [OperationContract]
        void UpdateCutOffDate(int optionId, int buildingId, DateTime cutOffDate);

        //Option[] ListOptions(Building building);
        //Option[] ListOptionsByType(Building building, int type);
        //OptionInfo[] ListPaidOptionsByBuilding(Building building);
        //OptionInfo[] ListPaidOptionsBySuite(Suite suite);
        //bool SetCutoffTime(Option option, DateTime time, out string errorReason);
    }

    //public class OptionInfo : Option
    //{
    //    public int Count { get; private set; }
    //}
}
