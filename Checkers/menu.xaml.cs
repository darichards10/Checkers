using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Data;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
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
        List<Piece> pieces;
        String usernameStr = App.usernameStr;
        String highScoreStr;
        int moves = 0;

        internal class Piece {

            string id;
            Button btn;    //Reference to the button 
            int direction; //1 or -1
            Boolean isKing;
            int side; //red = 1 -- black = 0

            public Piece(string id, Button btn, int direction, Boolean isKing, int side)
            {
                this.id = id;
                this.btn = btn;
                this.direction = direction;
                this.isKing = isKing;
                this.side = side;
            }

            public string GetId()
            {
                return id;
            }

            public Button GetButton()
            {
                return btn;
            }

            public int GetDirection()
            {
                return direction;
            }

            public bool IsKing()
            {
                return isKing;
            }

            public int GetSide()
            {
                return side;
            }
        }

       

        public menu()
        {
            InitializeComponent();
            // contentGrid.Children.Clear();

            //contentGrid.Background= null;
            pieces = makePieces();
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
            contentGrid.Background= null;
        }

        //Returns to mainWindow to change account
        private void changeAccount_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner = this;
            mainWindow.Show();
            this.Hide();
        }


        //Checks for a buttons/pieces valid moves and reurns which cells are valid
        private void validMove(Object sender, RoutedEventArgs e)
        {
            Button clickedBtn = (Button)sender;


            int index = -1;

            foreach (Piece piece in pieces)
            {
                if(piece.GetId().Equals(clickedBtn.Name))
                {
                    index = pieces.IndexOf(piece);
                }
                
            }

           
            //A list of open positions at begining of game

            List<Point> validMoves = new List<Point>();
            Piece clickedPiece = pieces[index];


            if ((this.moves % 2 == 0 && clickedPiece.GetSide() == 0) || (this.moves % 2 == 1 && clickedPiece.GetSide() == 1))
            {
                int curRow = Grid.GetRow(clickedBtn);
                int curColumn = Grid.GetColumn(clickedBtn);

                for (int row = 0; row <= 7; row++)
                {
                    for (int column = 0; column <= 7; column++)
                    {
                        // Calculate the distance between the current cell and the clicked piece's current position
                        int rowDiff = Math.Abs(row - curRow);
                        int columnDiff = Math.Abs(column - curColumn);



                        // Check if the distance is valid for the clicked piece
                        if (rowDiff == 1 && columnDiff == 1 && clickedPiece.IsKing())
                        {
                            // King can move in any direction diagonally
                            validMoves.Add(new Point(column, row));
                        }
                        else if (clickedPiece.GetSide() == 1 && row > curRow && rowDiff == 1 && columnDiff == 1)
                        {

                            // Side 1 pieces can only move diagonally down-left or down-right
                            validMoves.Add(new Point(column, row));
                        }
                        else if (clickedPiece.GetSide() == 0 && row < curRow && rowDiff == 1 && columnDiff == 1)
                        {
                            // Side 2 pieces can only move diagonally up-left or up-right
                            validMoves.Add(new Point(column, row));
                        }

                    }
                }
                this.moves++;
            } else
            {
                MessageBoxResult mbox = MessageBox.Show("Not your turn!!");
            }

            

            //Shows valid moves and returns a list of moves of buttons

            List<Button> moves =  showValidMoves(validMoves, clickedPiece);
           
        }


        //Showing the valid moves list and removing any previous moveButtons
        private List<Button> showValidMoves(List<Point> validMoves, Piece curPiece)
        {
            
            List<Button> moves = new List<Button>();
            removePossibleMoves();

            int oppositeSide = curPiece.GetSide() == 0 ? 1 : 0; // Get the opposite side


            foreach (var move in validMoves)
            {
                // Check if there is already a button in the grid cell
                bool hasButton = false;
                foreach (Button child in contentGrid.Children)
                {
                    if (Grid.GetRow(child) == (int)move.Y && Grid.GetColumn(child) == (int)move.X && child is Button)
                    {
                        // Check if the button belongs to the opposite side
                        foreach (Piece piece in pieces)
                        {
                            if (piece.GetButton() == child && piece.GetSide() == oppositeSide)
                            {
                                hasButton = true;
                                break;
                            }
                        }

                        break;
                    }
                }

                if (!hasButton)
                {
                    Button button = new Button();
                    button.Width = button.Height = 50;
                    button.Background = Brushes.Green;                  
                    Grid.SetColumn(button, (int)move.X);
                    Grid.SetRow(button, (int)move.Y);
                    button.Click += (sender, e) =>
                    {
                        ExecuteMove(curPiece, move);
                       
                    };
                    contentGrid.Children.Add(button);
                    moves.Add(button);
                }
            }
            return moves;
        }

        //Moves piece and clears possible moves
       private void ExecuteMove(Piece piece, Point newSpot)
        {
            Grid.SetColumn(piece.GetButton(), (int)newSpot.X);
            Grid.SetRow(piece.GetButton(), (int)newSpot.Y);
            removePossibleMoves();

        }

        private void removePossibleMoves()
        {
            var buttonsToRemove = contentGrid.Children.OfType<Button>().Where(b => b.Background == Brushes.Green).ToList();
            foreach (var button in buttonsToRemove)
            {
                contentGrid.Children.Remove(button);
            }

        }


        private List<Piece> makePieces()
        {
            List<Piece> pieces = new List<Piece>();

            pieces.Add(new Piece("r1", r1, 1, false, 1));
            pieces.Add(new Piece("r2", r2, 1, false, 1));
            pieces.Add(new Piece("r3", r3, 1, false, 1));
            pieces.Add(new Piece("r4", r4, 1, false, 1));
            pieces.Add(new Piece("r5", r5, 1, false, 1));
            pieces.Add(new Piece("r6", r6, 1, false, 1));
            pieces.Add(new Piece("r7", r7, 1, false, 1));
            pieces.Add(new Piece("r8", r8, 1, false, 1));
            pieces.Add(new Piece("r9", r9, 1, false, 1));
            pieces.Add(new Piece("r10", r10, 1, false, 1));
            pieces.Add(new Piece("r11", r11, 1, false, 1));
            pieces.Add(new Piece("r12", r12, 1, false, 1));

            pieces.Add(new Piece("b1", b1, -1, false, 0));
            pieces.Add(new Piece("b2", b2, -1, false, 0));
            pieces.Add(new Piece("b3", b3,- 1, false, 0));
            pieces.Add(new Piece("b4", b4, -1, false, 0));
            pieces.Add(new Piece("b5", b5, -1, false, 0));
            pieces.Add(new Piece("b6", b6, -1, false, 0));
            pieces.Add(new Piece("b7", b7, -1, false, 0));
            pieces.Add(new Piece("b8", b8, -1, false, 0));
            pieces.Add(new Piece("b9", b9, -1, false, 0));
            pieces.Add(new Piece("b10", b10, -1, false, 0));
            pieces.Add(new Piece("b11", b11, -1, false, 0));
            pieces.Add(new Piece("b12", b12, -1, false, 0));

            return pieces;
        }

        //Displays the history stored in the database of a user
        private async void history_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            contentGrid.Background= null;

            var con = connection();
            String history = "";

            historyPage historyPage = new historyPage();

            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "ID";
            column1.Binding = new Binding("ID");

            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "Score";
            column2.Binding = new Binding("Score");

            DataGridTextColumn column3 = new DataGridTextColumn();
            column3.Header = "Time";
            column3.Binding = new Binding("Time");

            historyPage.historyDG.Columns.Add(column1);
            historyPage.historyDG.Columns.Add(column2);
            historyPage.historyDG.Columns.Add(column3);


            
            using (var command = con.CreateSelectCommand("SELECT ID, score, time FROM history WHERE username = @user"))
            {
                command.Parameters.Add("user", SpannerDbType.String).Value = "Username";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var idVal = reader.GetFieldValue<int>("ID");
                        var score = reader.GetFieldValue<int>("score");
                        var time = reader.GetFieldValue<string>("time");

                        MessageBoxResult box = MessageBox.Show(idVal + " - " + score + " - " + time);
                    }
                }

            }



                MessageBoxResult mbox = MessageBox.Show(history);
           //set text in history page to history string

        }

       
    }

}
