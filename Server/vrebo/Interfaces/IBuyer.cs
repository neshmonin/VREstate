using System.Collections.Generic;
using System.ServiceModel;
using System;
namespace Vre.Server.BusinessLogic
{
    [ServiceContract(SessionMode=SessionMode.Required)]
    public interface IBuyer : IAuthenticatedUser
    {


        [OperationContract]
        List<Account> GetAccounts();
        [OperationContract]
        ContactInfo GetContactInfo();


        //Option[] ListOptionsBySuite(Suite suite);

        //AccountInfo[] ListAccounts();
        //Transaction[] ListTransactions(Account account);
        //Option[] ListOptionsByTransaction(Transaction transaction);

        //Account CreateAccount(Suite suite);
        //Transaction CreateTransaction(Account account);
        //bool AddTransactionOption(Transaction transaction, Option option, out string errorReason);
        //bool RemoveTransactionOption(Transaction transaction, Option option, out string errorReason);
    }

    //public class AccountInfo : Account
    //{
    //    public string BuildingDescription { get; private set; }  // e.g. "5900 St. Clair Ave W., Toronto"
    //    public int SuiteNo { get; private set; }
    //}
}

