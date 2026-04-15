using System;
using System.Windows;
using ExchangeOffice.WpfClient.ExchangeService;

namespace ExchangeOffice.WpfClient
{
    public partial class MainWindow : Window
    {
        // We will hardcode User 1 for now
        private readonly int currentUserId = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        // 1. This triggers automatically when the window opens
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDashboard();
        }

        // 2. The centralized method to fetch live data from the WCF Service
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

        // 3. The Exchange Button Logic
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
                    // Execute the trade
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