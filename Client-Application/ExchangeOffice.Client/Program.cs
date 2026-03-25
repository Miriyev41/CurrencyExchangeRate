using System;

namespace ExchangeOffice.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting the Exchange Office Client...");
            var client = new ExchangeServiceReference.Service1Client();

            try
            {
                // Let's keep the Lab 1 test just to make sure the server is awake
                Console.Write("Enter your name: ");
                string userName = Console.ReadLine();
                Console.WriteLine(client.TestConnection(userName));
                Console.WriteLine("--------------------------------------------------");

                // --- NEW LAB 2 TEST ---
                Console.Write("\nEnter a currency code to check the live rate (e.g., USD, EUR, GBP): ");
                string currencyCode = Console.ReadLine().ToUpper(); // The NBP API requires uppercase!

                decimal liveRate = client.GetExchangeRate(currencyCode);

                if (liveRate == 0)
                {
                    Console.WriteLine($"\nError: Could not find rate for {currencyCode}. Make sure it is a valid 3-letter code.");
                }
                else
                {
                    Console.WriteLine($"\nSUCCESS: The current rate for {currencyCode} is {liveRate} PLN.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                client.Close();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}