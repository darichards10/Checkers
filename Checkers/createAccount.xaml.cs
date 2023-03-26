using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for createAccount.xaml
    /// </summary>
    public partial class createAccount : Window
    {
        public createAccount()
        {
            InitializeComponent();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner = this;
            mainWindow.Show();
            this.Hide();
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            String user = username.Text;
            String pwrd = password1.Text;

            
            
            try {

                if (password1.ToString().Equals(password2.ToString()))
                {

                    insertAccount(user, pwrd);

                }
                else
                {
                    MessageBoxResult mBox = MessageBox.Show("Passwords do not match!!");
                }
            } 
            catch (Exception exc){
                MessageBox.Show(exc.Message);
            }
           

            
            
        }

        //Inserts user and password into table
        private async void insertAccount(String user, String pwrd)
        {
            if (checkUser(user))
            {
                MessageBoxResult mBox = MessageBox.Show("Username already taken");
            }
            else
            {

                var connection = conn();
                try
                {
                    await connection.RunWithRetriableTransactionAsync(async transaction =>
                    {
                        // user = hash(user);
                        pwrd = hash(pwrd);

                        

                        var cmd = connection.CreateInsertCommand("users", new SpannerParameterCollection
                        {
                           
                            {"username", SpannerDbType.String, user},
                            {"password", SpannerDbType.String, pwrd}
                        });

                        cmd.Transaction= transaction;
                        await cmd.ExecuteNonQueryAsync();

                        transaction.Commit();

                    });
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        
        //Reuturns true if username entered is already in database
        private Boolean checkUser(String user)
        {
            
            var connection = conn();
            int count;

           using (var cmd = connection.CreateSelectCommand(
                "SELECT COUNT(username) FROM users " +
                "WHERE username = " + user)) {

                cmd.Parameters.Add("username", SpannerDbType.String).Value= user;

                var reader = cmd.ExecuteReader();

                reader.Read();

                count = reader.GetInt32(0);

            }

            return count > 0;
           
        }


        //Makes spanner connection to table and returns spanner connection
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
            SHA256 sHA256= SHA256.Create();
            byte[] hashedBytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(key));
            return Convert.ToBase64String(hashedBytes);

        }

        private void username_GotFocus(object sender, RoutedEventArgs e)
        {
            username.Text = string.Empty;
        }

        private void password1_GotFocus(object sender, RoutedEventArgs e)
        {
            password1.Text = string.Empty;
        }

        private void password2_GotFocus(object sender, RoutedEventArgs e)
        {
            password2.Text = string.Empty;
        }
    }
}
