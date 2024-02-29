using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for BorrowersWindow.xaml
    /// </summary>
    public partial class BorrowersWindow : Window
    {
        private DataBaseConnection dbConnection;
        private List<Borrower> borrowers;
        public BorrowersWindow()
        {
            InitializeComponent();
            dbConnection = new DataBaseConnection();
            LoadBorrowers();
        }

        private void LoadBorrowers()
        {
            borrowers = dbConnection.GetBorrowers();
            BorrowersListBox.ItemsSource = borrowers;
        }

        

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (BorrowersListBox.SelectedItem is Borrower selectedBorrower)
            {
                MainWindow mainWindow = new MainWindow(selectedBorrower);
                mainWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Please select a borrower to continue.");
            }

        }
    }
}
