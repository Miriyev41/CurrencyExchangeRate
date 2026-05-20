using System;
using System.Windows;
using ExchangeOffice.WpfClient.ExchangeService;

namespace ExchangeOffice.WpfClient
{
    public partial class MainWindow : Window
    {
        private int currentUserId;

        public MainWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            try
            {
                using (Service1Client client = new Service1Client())
                {
                    lstWallets.ItemsSource = client.GetUserWallets(currentUserId);
                    lstHistory.ItemsSource = client.GetTransactionHistory(currentUserId);
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Dashboard Sync Error: {ex.Message}";
            }
        }

        private void btnTopUp_Click(object sender, RoutedEventArgs e)
        {
            string currency = txtTopUpCurrency.Text.Trim();
            string amountText = txtTopUpAmount.Text.Trim();

            if (string.IsNullOrEmpty(currency) || !decimal.TryParse(amountText, out decimal amount))
            {
                txtResult.Text = "Top-Up Error: Please enter valid currency and amount.";
                return;
            }

            try
            {
                using (Service1Client client = new Service1Client())
                {
                    string result = client.TopUpWallet(currentUserId, currency, amount);
                    if (result == "Success")
                    {
                        txtResult.Text = $"Successfully deposited {amount} {currency.ToUpper()}!";
                        RefreshDashboard();
                        txtTopUpCurrency.Text = "";
                        txtTopUpAmount.Text = "";
                    }
                    else txtResult.Text = result;
                }
            }
            catch (Exception ex) { txtResult.Text = $"Connection Error: {ex.Message}"; }
        }

        private void btnExchange_Click(object sender, RoutedEventArgs e)
        {
            string fromCurr = txtFromCurrency.Text.Trim();
            string toCurr = txtToCurrency.Text.Trim();
            string amountText = txtAmount.Text.Trim();

            if (string.IsNullOrEmpty(fromCurr) || string.IsNullOrEmpty(toCurr) || !decimal.TryParse(amountText, out decimal amount))
            {
                txtResult.Text = "Validation Error: Please check your input fields.";
                return;
            }

            try
            {
                using (Service1Client client = new Service1Client())
                {
                    string result = client.PerformExchange(currentUserId, fromCurr, toCurr, amount);
                    txtResult.Text = result;
                    if (result.StartsWith("Success"))
                    {
                        RefreshDashboard();
                        txtAmount.Text = "";
                    }
                }
            }
            catch (Exception ex) { txtResult.Text = $"System Error: {ex.Message}"; }
        }

        // Historical Rates Button Logic
        private void btnCheckHistory_Click(object sender, RoutedEventArgs e)
        {
            string currency = txtHistoryCurrency.Text.Trim();

            if (string.IsNullOrEmpty(currency) || !dpHistoryDate.SelectedDate.HasValue)
            {
                txtHistoryResult.Text = "Error: Please enter a currency and pick a date.";
                return;
            }

            DateTime selectedDate = dpHistoryDate.SelectedDate.Value;

            if (selectedDate > DateTime.Now)
            {
                txtHistoryResult.Text = "Error: Cannot fetch future rates!";
                return;
            }

            txtHistoryResult.Text = "Searching NBP Archives...";

            try
            {
                using (Service1Client client = new Service1Client())
                {
                    string result = client.GetHistoricalRate(currency, selectedDate);
                    txtHistoryResult.Text = result;
                }
            }
            catch (Exception ex) { txtHistoryResult.Text = "Connection Error: " + ex.Message; }
        }

        private void txtFromCurrency_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void lstHistory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}