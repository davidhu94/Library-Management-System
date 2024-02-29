using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataBaseConnection dbConnection;
        private ObservableCollection<BookInfo> books;
        private Borrower borrower;

        public MainWindow(Borrower selectedBorrower)
        {
            InitializeComponent();
            dbConnection = new DataBaseConnection();
            borrower = selectedBorrower;

            books = new ObservableCollection<BookInfo>();
            bookListBox.ItemsSource = books;

            LoadBooks();
        }

        private void LoadBooks()
        {
            books.Clear();

            Dictionary<int, Books> fetchedBooks = dbConnection.GetBooks();

            foreach (var book in fetchedBooks.Values)
            {
                Author author = dbConnection.GetAuthorById(book.AuthorId);

                BookInfo bookInfo = new BookInfo
                {
                    BookId = book.Id, 
                    BookTitle = book.Title,
                    Author = author.AuthorName,
                    AvailableCopies = book.AvailableCopies,
                    TotalCopies = book.TotalCopies
                };

                books.Add(bookInfo);
                bookInfo.UpdateCopiesText();
            }
        }

        private void BorrowBook(BookInfo selectedBook)
        {
            if (selectedBook != null && selectedBook.AvailableCopies > 0)
            {
                
                int bookId = selectedBook.BookId;

                DateTime borrowDate = DateTime.Now;
                DateTime returnDate = borrowDate.AddDays(14);
                dbConnection.BorrowBook(bookId, borrower.Id, borrowDate, returnDate);
                dbConnection.UpdateAvailableCopies(selectedBook.BookTitle, selectedBook.AvailableCopies - 1);
                selectedBook.AvailableCopies--;

                MessageBox.Show($"{selectedBook.BookTitle} has been borrowed. {selectedBook.AvailableCopies}/{selectedBook.TotalCopies} copies left.");

                LoadBooks();
            }
            else
            {
                MessageBox.Show("No available copies for borrowing. Please check the available copies for the selected book.");
            }
        }

        private void ReturnBook(BookInfo selectedBook)
        {
            if (selectedBook != null)
            {
                if (selectedBook.AvailableCopies < selectedBook.TotalCopies)
                {
                    int bookId = selectedBook.BookId;
                    DateTime returnDate = DateTime.Now;
                    dbConnection.ReturnBook(bookId, borrower.Id, returnDate);

                    dbConnection.UpdateAvailableCopies(selectedBook.BookTitle, selectedBook.AvailableCopies + 1);
                    selectedBook.AvailableCopies++;

                    MessageBox.Show($"{selectedBook.BookTitle} has been returned. {selectedBook.AvailableCopies}/{selectedBook.TotalCopies} copies left.");
                }
                else
                {
                    MessageBox.Show("Can't return more copies than the maximum available.");
                }
            }
            else
            {
                MessageBox.Show("Please select a book to return.");
            }
        }

        private void borrowButton_Click(object sender, RoutedEventArgs e)
        {
            BorrowBook(bookListBox.SelectedItem as BookInfo);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnBook(bookListBox.SelectedItem as BookInfo);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow().Show();
            Hide();
        }
    }
}