using System;

namespace ExchangeOffice.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting the Exchange Office Client...");

            // 1. Create the client object using the reference Visual Studio just generated
            var client = new ExchangeServiceReference.Service1Client();

            try
            {
                // 2. Get user input
                Console.Write("Enter your name: ");
                string userName = Console.ReadLine();

                // 3. Call the method over the local network!
                string response = client.TestConnection(userName);

                // 4. Print the response from the server
                Console.WriteLine("\nResponse from WCF Server:");
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // 5. Always securely close the client when done
                client.Close();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}