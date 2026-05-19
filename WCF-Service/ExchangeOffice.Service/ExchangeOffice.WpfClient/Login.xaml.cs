using System;
using System.Windows;
using ExchangeOffice.WpfClient.ExchangeService; // Make sure this matches your service reference name

namespace ExchangeOffice.WpfClient
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password; // PasswordBox uses .Password instead of .Text

            try
            {
                using (Service1Client client = new Service1Client())
                {
                    int userId = client.Login(username, password);
                    if (userId > 0)
                    {
                        OpenDashboard(userId);
                    }
                    else
                    {
                        txtMessage.Text = "Invalid username or password.";
                    }
                }
            }
            catch (Exception ex)
            {
                txtMessage.Text = "Connection error: " + ex.Message;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Please fill in all fields.";
                return;
            }

            try
            {
                using (Service1Client client = new Service1Client())
                {
                    int userId = client.Register(username, password);
                    if (userId > 0)
                    {
                        MessageBox.Show("Registration successful! Logging in...", "Success");
                        OpenDashboard(userId);
                    }
                    else if (userId == -1)
                    {
                        txtMessage.Text = "Username is already taken.";
                    }
                }
            }
            catch (Exception ex)
            {
                txtMessage.Text = "Connection error: " + ex.Message;
            }
        }

        private void OpenDashboard(int userId)
        {
            // We will pass the real User ID to the dashboard!
            MainWindow dashboard = new MainWindow(userId);
            dashboard.Show();
            this.Close(); // Closes the login screen
        }
    }
}