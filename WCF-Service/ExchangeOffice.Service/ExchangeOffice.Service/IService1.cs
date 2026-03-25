using System.ServiceModel;

namespace ExchangeOffice.Service
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string TestConnection(string userName);

        [OperationContract]
        decimal GetExchangeRate(string currencyCode);
    }
}