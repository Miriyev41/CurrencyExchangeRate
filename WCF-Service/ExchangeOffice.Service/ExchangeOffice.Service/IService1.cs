using System.ServiceModel;

namespace ExchangeOffice.Service
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string TestConnection(string userName);

        // --- NEW LAB 2 METHOD ---
        [OperationContract]
        decimal GetExchangeRate(string currencyCode);
    }
}