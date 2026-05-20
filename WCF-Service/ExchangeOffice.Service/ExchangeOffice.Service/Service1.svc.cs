using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeOffice.Service
{
    public class Service1 : IService1
    {
        // 1.  Connection Test
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
                    return nbpData.Rates[0].Mid;
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
                    // 1. Get the rate for BOTH currencies 
                    decimal fromRate = (fromCurrency == "PLN") ? 1.0m : GetExchangeRate(fromCurrency);
                    decimal toRate = (toCurrency == "PLN") ? 1.0m : GetExchangeRate(toCurrency);

                    if (fromRate <= 0) return $"Error: Could not find rate for {fromCurrency}.";
                    if (toRate <= 0) return $"Error: Could not find rate for {toCurrency}.";

                    var sourceWallet = db.Wallets.FirstOrDefault(w => w.UserId == userId && w.CurrencyCode == fromCurrency);
                    var targetWallet = db.Wallets.FirstOrDefault(w => w.UserId == userId && w.CurrencyCode == toCurrency);

                    if (sourceWallet == null) return $"Error: Wallet for {fromCurrency} does not exist.";
                    if (sourceWallet.Balance < amount) return $"Error: Insufficient funds in {fromCurrency}.";

                    // Note: If target wallet doesn't exist, we should probably create it!
                    if (targetWallet == null)
                    {
                        targetWallet = new Wallet { UserId = userId, CurrencyCode = toCurrency, Balance = 0 };
                        db.Wallets.Add(targetWallet);
                    }

                    // 2. Convert to PLN first, then divide by the target rate
                    decimal amountInPLN = amount * fromRate;
                    decimal convertedAmount = amountInPLN / toRate;

                    // Calculate the actual exchange rate 
                    decimal finalExchangeRate = fromRate / toRate;

                    sourceWallet.Balance -= amount;
                    targetWallet.Balance += convertedAmount;

                    db.Transactions.Add(new Transaction
                    {
                        UserId = userId,
                        BoughtCurrency = toCurrency,
                        SoldCurrency = fromCurrency,
                        Amount = amount,
                        ExchangeRate = finalExchangeRate,
                        TransactionDate = DateTime.Now
                    });

                    db.SaveChanges();

                    return $"Success! Swapped {amount} {fromCurrency} for {Math.Round(convertedAmount, 2)} {toCurrency} (Rate: {Math.Round(finalExchangeRate, 4)}).";
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


                    var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                    if (user != null)
                    {
                        return user.Id; // Success: Return the real User ID
                    }

                    return 0; // Failure:
                }
                catch (Exception)
                {
                    return -1; // error during authentication process
                }
            }
        }

        public int Login(string username, string password)
        {
            using (var db = new ExchangeDbContext())
            {
                // Find a user that matches both username AND password
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null) return user.Id; // Success! Return their ID
                return 0; // Failed login
            }
        }

        public int Register(string username, string password)
        {
            using (var db = new ExchangeDbContext())
            {
                // Check if username is taken
                if (db.Users.Any(u => u.Username == username)) return -1;

                // Create the new user
                var newUser = new User { Username = username, Password = password };
                db.Users.Add(newUser);
                db.SaveChanges(); // Save to get the new ID

                db.Wallets.Add(new Wallet { UserId = newUser.Id, CurrencyCode = "PLN", Balance = 0 });
                db.Wallets.Add(new Wallet { UserId = newUser.Id, CurrencyCode = "USD", Balance = 0 });
                db.SaveChanges();

                return newUser.Id;
            }
        }
        public string TopUpWallet(int userId, string currencyCode, decimal amount)
        {
            if (amount <= 0) return "Amount must be greater than zero.";

            // Standardize the currency code (e.g., pln -> PLN)
            currencyCode = currencyCode.ToUpper();

            using (var db = new ExchangeDbContext())
            {
                // Find the user's wallet for this specific currency
                var wallet = db.Wallets.FirstOrDefault(w => w.UserId == userId && w.CurrencyCode == currencyCode);

                if (wallet != null)
                {
                    // Add the money to their existing balance
                    wallet.Balance += amount;
                }
                else
                {
                    // If they don't have a wallet for this currency yet, make one!
                    wallet = new Wallet { UserId = userId, CurrencyCode = currencyCode, Balance = amount };
                    db.Wallets.Add(wallet);
                }

                db.SaveChanges();
                return "Success";
            }
        }
        public string GetHistoricalRate(string currencyCode, DateTime date)
        {
            if (currencyCode.ToUpper() == "PLN") return "1 PLN is always 1 PLN.";

            string dateString = date.ToString("yyyy-MM-dd");
            string url = $"http://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}/{dateString}/?format=json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonResult = client.GetStringAsync(url).Result;
                    var nbpData = JsonConvert.DeserializeObject<NbpResponse>(jsonResult);

                    return $"Archive Data: 1 {currencyCode.ToUpper()} = {nbpData.Rates[0].Mid} PLN on {dateString}";
                }
            }
            catch
            {

                return $"No data for {dateString}. (Markets are closed on weekends/holidays).";
            }
        }
    }

    
}