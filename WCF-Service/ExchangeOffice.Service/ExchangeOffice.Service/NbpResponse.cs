using System.Collections.Generic;

namespace ExchangeOffice.Service
{
    public class NbpResponse
    {
        public string Code { get; set; }
        public List<NbpRate> Rates { get; set; }
    }

    public class NbpRate
    {
        public decimal Mid { get; set; }
    }
}