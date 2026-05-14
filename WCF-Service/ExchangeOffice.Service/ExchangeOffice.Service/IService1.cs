using System.Collections.Generic;
using System.ServiceModel;

[ServiceContract]
public interface IService1
{
    [OperationContract]
    string TestConnection(string userName);

    [OperationContract]
    decimal GetExchangeRate(string currencyCode);

    [OperationContract]
    string TestDatabaseConnection();

    [OperationContract]
    string PerformExchange(int userId, string fromCurrency, string toCurrency, decimal amount);

    // MAKE SURE THIS LINE IS HERE:
    [OperationContract]
    List<string> GetTransactionHistory(int userId);

    [OperationContract]
    List<string> GetUserWallets(int userId);

    [OperationContract]
    int AuthenticateUser(string username, string password);
}