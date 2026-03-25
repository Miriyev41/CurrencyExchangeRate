using System;

namespace ExchangeOffice.Service
{
    // Notice how Service1 inherits from the IService1 interface we just fixed
    public class Service1 : IService1
    {
        public string TestConnection(string userName)
        {
            // A simple response to prove the service received the message
            return $"Hello {userName}, the WCF Service is running successfully! Time: {DateTime.Now}";
        }
    }
}