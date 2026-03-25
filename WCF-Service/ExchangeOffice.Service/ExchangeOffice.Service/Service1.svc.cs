using System;
using System.Net.Http; // Lets us go to the internet
using Newtonsoft.Json; // Lets us read JSON data
using System.Collections.Generic;

namespace ExchangeOffice.Service
{
    public class Service1 : IService1
    {
        public string TestConnection(string userName)
        {
            return $"Hello {userName}, the WCF Service is running successfully! Time: {DateTime.Now}";
        }

        public decimal GetExchangeRate(string currencyCode)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 1. Build the exact URL for the NBP API using the currency code
                    string url = $"http://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}/?format=json";

                    // 2. Send the request and grab the raw JSON text
                    string jsonResponse = client.GetStringAsync(url).Result;

                    // 3. Translate the JSON text into our C# helper classes
                    NbpResponse nbpData = JsonConvert.DeserializeObject<NbpResponse>(jsonResponse);

                    // 4. Return just the decimal number (the 'mid' rate)
                    return nbpData.rates[0].mid;
                }
                catch (Exception)
                {
                    // If the user types a fake currency like "XYZ", the NBP API returns an error.
                    // For now, we will return 0 to tell the client something went wrong.
                    return 0;
                }
            }
        }
    }

    // --- HELPER CLASSES FOR JSON DESERIALIZATION ---
    // The NBP API sends an object with a "rates" array, which contains the "mid" value.
    public class NbpResponse
    {
        public List<NbpRate> rates { get; set; }
    }

    public class NbpRate
    {
        public decimal mid { get; set; }
    }
}