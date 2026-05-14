using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeOffice.Service
{
    public class Service1 : IService1
    {
        // 1. Simple Connection Test
        public string TestConnection(string userName)
        {
            return $"Hello {userName}, the WCF Service is running successfully! Time: {DateTime.Now}";
        }

        // 2. NBP API Integration (Live Rates)
        public decimal GetExchangeRate(string currencyCode)
        {
            if (currencyCode == "PLN") return 1.0m;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"http://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}/?format=json";
                    string jsonResponse = client.GetStringAsync(url).Result;
                    NbpResponse nbpData = JsonConvert.DeserializeObject<NbpResponse>(jsonResponse);
                    return nbpData.rates[0].mid;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        // 3. Database Connection Test
        public string TestDatabaseConnection()
        {
            try
            {
                using (var db = new ExchangeDbContext())
                {
                    var user = db.Users.FirstOrDefault();
                    if (user != null)
                    {
                        return $"Success! Database linked. Found user: {user.Username} (ID: {user.Id})";
                    }
                    return "Connected to database file, but no users found in the table.";
                }
            }
            catch (Exception ex)
            {
                return $"Database Error: {ex.Message}";
            }
        }

        // 4. PERFORM EXCHANGE LOGIC
        public string PerformExchange(int userId, string fromCurrency, string toCurrency, decimal amount)
        {
            using (var db = new ExchangeDbContext())
            {
                try
                {
                    string currencyToFetch = (fromCurrency == "PLN") ? toCurrency : fromCurrency;
                    decimal rate = GetExchangeRate(currencyToFetch);

                    if (rate <= 0) return $"Error: Could not find rate for {currencyToFetch}.";

                    var sourceWallet = db.Wallets.FirstOrDefault(w => w.UserId == userId && w.CurrencyCode == fromCurrency);
                    var targetWallet = db.Wallets.FirstOrDefault(w => w.UserId == userId && w.CurrencyCode == toCurrency);

                    if (sourceWallet == null) return $"Error: Wallet for {fromCurrency} does not exist.";
                    if (sourceWallet.Balance < amount) return $"Error: Insufficient funds in {fromCurrency}.";
                    if (targetWallet == null) return $"Error: Target wallet for {toCurrency} does not exist.";

                    decimal convertedAmount = (fromCurrency == "PLN") ? amount / rate : amount * rate;

                    sourceWallet.Balance -= amount;
                    targetWallet.Balance += convertedAmount;

                    db.Transactions.Add(new Transaction
                    {
                        UserId = userId,
                        BoughtCurrency = toCurrency,
                        SoldCurrency = fromCurrency,
                        Amount = amount,
                        ExchangeRate = rate,
                        TransactionDate = DateTime.Now
                    });

                    db.SaveChanges();

                    return $"Success! Swapped {amount} {fromCurrency} for {Math.Round(convertedAmount, 2)} {toCurrency} (Rate: {rate}).";
                }
                catch (Exception ex)
                {
                    return "Transaction Failed: " + ex.Message;
                }
            }
        }

        // 5. GET TRANSACTION HISTORY
        public List<string> GetTransactionHistory(int userId)
        {
            using (var db = new ExchangeDbContext())
            {
                try
                {
                    // LINQ query to get the 10 most recent transactions
                    var history = db.Transactions
                        .Where(t => t.UserId == userId)
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(10)
                        .ToList();

                    List<string> lines = new List<string>();
                    foreach (var t in history)
                    {
                        lines.Add($"[{t.TransactionDate:yyyy-MM-dd HH:mm}] Sold: {t.Amount} {t.SoldCurrency} | Bought: {t.BoughtCurrency} | Rate: {t.ExchangeRate}");
                    }
                    return lines;
                }
                catch (Exception ex)
                {
                    return new List<string> { "Could not load history: " + ex.Message };
                }
            }
        }

        // 6. GET USER WALLETS (For the Dashboard)
        public List<string> GetUserWallets(int userId)
        {
            using (var db = new ExchangeDbContext())
            {
                try
                {
                    // Find all wallets belonging to this user
                    var wallets = db.Wallets.Where(w => w.UserId == userId).ToList();
                    List<string> lines = new List<string>();

                    if (wallets.Count == 0) return new List<string> { "No wallets found." };

                    // Format them nicely for the UI
                    foreach (var w in wallets)
                    {
                        lines.Add($"{w.CurrencyCode}: {Math.Round(w.Balance, 2)}");
                    }
                    return lines;
                }
                catch (Exception ex)
                {
                    return new List<string> { "Error loading wallets: " + ex.Message };
                }
            }
        }

        // 7. AUTHENTICATION GATEWAY
        public int AuthenticateUser(string username, string password)
        {
            using (var db = new ExchangeDbContext())
            {
                try
                {
                    // SECURITY NOTE: In a real enterprise system, passwords are NEVER stored as plain text. 
                    // They are hashed (e.g., using BCrypt or SHA256). 
                    // For this architecture lab, we are checking the raw string.

                    var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                    if (user != null)
                    {
                        return user.Id; // Success: Return the real User ID
                    }

                    return 0; // Failure: Invalid credentials
                }
                catch (Exception)
                {
                    return -1; // Database crash/error
                }
            }
        }
    }

    public class NbpResponse { public List<NbpRate> rates { get; set; } }
    public class NbpRate { public decimal mid { get; set; } }
}