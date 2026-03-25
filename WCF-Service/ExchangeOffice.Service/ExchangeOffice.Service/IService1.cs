using System.ServiceModel;

namespace ExchangeOffice.Service
{
    // [ServiceContract] tells the framework that this interface is a Web Service
    [ServiceContract]
    public interface IService1
    {
        // [OperationContract] exposes this specific method to the outside world
        [OperationContract]
        string TestConnection(string userName);
    }
}