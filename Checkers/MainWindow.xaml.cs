using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Close button
        private void close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Login button will attempt validation
        private void login_Click(object sender, RoutedEventArgs e)
        {
            if(validateUser(username.Text, password.Text))
            {
                successfulLogin();
            } else
            {
                MessageBoxResult mBox = MessageBox.Show("Either invalid username or password!!");
            }
            
        }

        //Open application window on validation
        private void successfulLogin()
        {
            
                menu menuWindow = new menu();
                menuWindow.Owner = this;
                menuWindow.Show();
                this.Hide();
        
        }

        private void createAccount_Click(object sender, RoutedEventArgs e)
        {
            createAccount caWindow = new createAccount();
            caWindow.Owner = this;
            caWindow.Show();
            this.Hide();
        }

        //Checks for valid user by finding uesrname then checking its pasword
        private Boolean validateUser(string username, string password)
        {
            var connection = conn();
            Boolean result = false;

            password = hash(password);

            try
            {
               
                using (var cmd = connection.CreateSelectCommand(
                     "SELECT password FROM users " +
                     "WHERE username = " + username))
                {

                    cmd.Parameters.Add("username", SpannerDbType.String).Value= username;

                    var reader = cmd.ExecuteReader();

                    reader.Read();

                    String pwrd = reader.GetString(0);

                    result = pwrd.Equals(password);
                    

                }

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }


            return result;
        }

        private SpannerConnection conn()
        {
            string projectId = "tough-line-378822";
            string instanceId = "checkersdb";
            string databaseId = "checkers";
            string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/"
            + $"databases/{databaseId}";

            return new SpannerConnection(connectionString);

        }

        //Utility method to hash strings
        private string hash(String key)
        {
            SHA256 sHA256 = SHA256.Create();
            byte[] hashedBytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(key));
            return Convert.ToBase64String(hashedBytes);

        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            password.Text = "";
        }

        private void username_GotFocus(object sender, RoutedEventArgs e)
        {
            username.Text = "";
        }
    }
}
