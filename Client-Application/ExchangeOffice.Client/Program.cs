using System;
using ExchangeOffice.Client.ExchangeService;

namespace ExchangeOffice.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Initialize the connection to the service
            Service1Client client = new Service1Client();

            try
            {

                // 2. Initial Connection Tests
                string greeting = client.TestConnection("");
                Console.WriteLine($"[Service Check]: {greeting}");

                string dbResult = client.TestDatabaseConnection();
                Console.WriteLine($"[Database Check]: {dbResult}");

                // 3. INTERACTIVE EXCHANGE MENU
                Console.WriteLine("\n--- NEW TRANSACTION ---");

                Console.Write("Enter currency to SELL (e.g., EUR): ");
                string fromCurr = Console.ReadLine().ToUpper().Trim();

                Console.Write("Enter currency to BUY (e.g., PLN): ");
                string toCurr = Console.ReadLine().ToUpper().Trim();

                Console.Write("Enter amount to exchange: ");
                string amountInput = Console.ReadLine();

                if (decimal.TryParse(amountInput, out decimal amount))
                {
                    Console.WriteLine($"\nRequesting exchange of {amount} {fromCurr} for {toCurr}...");

                    // Call the Service (User ID 1 is Mirparvin)
                    string result = client.PerformExchange(1, fromCurr, toCurr, amount);

                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine("RESULT: " + result);
                    Console.WriteLine("----------------------------------------");
                }
                else
                {
                    Console.WriteLine("Invalid amount format.");
                }

                // 4. SHOW TRANSACTION HISTORY (The "Top Grade" Feature)
                Console.WriteLine("\n--- RECENT TRANSACTION HISTORY ---");
                // Note: WCF converts List<string> to an Array string[] for the client
                string[] history = client.GetTransactionHistory(1);

                if (history != null && history.Length > 0)
                {
                    foreach (var record in history)
                    {
                        Console.WriteLine(record);
                    }
                }
                else
                {
                    Console.WriteLine("No history found for this user.");
                }

                // 5. Final Status Check
                Console.WriteLine("\n--- FINAL ACCOUNT STATUS ---");
                string finalStatus = client.TestDatabaseConnection();
                Console.WriteLine(finalStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[CRITICAL ERROR]: {ex.Message}");
            }
            finally
            {
                if (client.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    client.Close();
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}