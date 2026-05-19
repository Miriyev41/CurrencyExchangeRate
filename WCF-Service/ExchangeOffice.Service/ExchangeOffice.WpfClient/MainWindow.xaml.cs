using System;
using System.Windows;
using ExchangeOffice.WpfClient.ExchangeService;

namespace ExchangeOffice.WpfClient
{
    public partial class MainWindow : Window
    {
        // Holds the ID of the currently logged-in user
        private int currentUserId;

        public MainWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId; // Save the logged-in user's ID
        }

        // This triggers automatically when the window opens
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDashboard();
        }

        // The centralized method to fetch live data from the WCF Service
        private void RefreshDashboard()
        {
            try
            {
                using (Service1Client client = new Service1Client())
                {
                    // Fetch Wallets
                    var wallets = client.GetUserWallets(currentUserId);
                    lstWallets.ItemsSource = wallets;

                    // Fetch Transaction History
                    var history = client.GetTransactionHistory(currentUserId);
                    lstHistory.ItemsSource = history;
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Dashboard Sync Error: {ex.Message}";
            }
        }

        // NEW: The Top-Up / Deposit Logic
        private void btnTopUp_Click(object sender, RoutedEventArgs e)
        {
            string currency = txtTopUpCurrency.Text.Trim();
            string amountText = txtTopUpAmount.Text.Trim();

            if (string.IsNullOrEmpty(currency) || !decimal.TryParse(amountText, out decimal amount))
            {
                txtResult.Text = "Top-Up Error: Please enter a valid currency code and amount.";
                return;
            }

            try
            {
                using (Service1Client client = new Service1Client())
                {
                    // Call the backend to add the money
                    string result = client.TopUpWallet(currentUserId, currency, amount);

                    if (result == "Success")
                    {
                        txtResult.Text = $"Successfully deposited {amount} {currency.ToUpper()}!";
                        RefreshDashboard(); // Instantly update the UI balances

                        // Clear the input boxes
                        txtTopUpCurrency.Text = "";
                        txtTopUpAmount.Text = "";
                    }
                    else
                    {
                        txtResult.Text = result;
                    }
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Connection Error: {ex.Message}";
            }
        }

        // The Exchange Button Logic
        private void btnExchange_Click(object sender, RoutedEventArgs e)
        {
            string fromCurr = txtFromCurrency.Text.Trim();
            string toCurr = txtToCurrency.Text.Trim();
            string amountText = txtAmount.Text.Trim();

            if (string.IsNullOrEmpty(fromCurr) || string.IsNullOrEmpty(toCurr))
            {
                txtResult.Text = "Validation Error: Please enter both currency codes.";
                return;
            }

            if (!decimal.TryParse(amountText, out decimal amount))
            {
                txtResult.Text = "Validation Error: Invalid amount format.";
                return;
            }

            txtResult.Text = "Processing transaction over secure WCF connection...";

            try
            {
                using (Service1Client client = new Service1Client())
                {
                    // Execute the trade using the actual logged-in user's ID
                    string result = client.PerformExchange(currentUserId, fromCurr, toCurr, amount);
                    txtResult.Text = result;

                    // If the trade was successful, refresh the dashboard!
                    if (result.StartsWith("Success"))
                    {
                        RefreshDashboard();

                        // Clear the input boxes for the next trade
                        txtAmount.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = $"System Error: {ex.Message}";
            }
        }
    }
}