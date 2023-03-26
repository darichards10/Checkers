using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for menu.xaml
    /// </summary>
    public partial class menu : Window
    {
        
        public menu()
        {
            InitializeComponent();
           

        }

        //Prompts user if they are sure they want to close application
        private void close_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to exit?", "Confirm",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        //Displays the checkerboard with a game ready to start 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void play_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            
            checkerBoard checkerBoard = new checkerBoard();

            contentGrid.Children.Add(checkerBoard);
          
        }

        private SpannerConnection connection()
        {
            string projectId = "tough-line-378822";
            string instanceId = "checkersdb";
            string databaseId = "checkers";
            string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/"
            + $"databases/{databaseId}";

            return new SpannerConnection(connectionString);
           
        }
        
        //Give the user options to change board bg (325,325) boards
        private void changeBoard_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
        }

        private void changeAccount_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner= this;
            mainWindow.Show();
            this.Hide();
        }
    }
}
