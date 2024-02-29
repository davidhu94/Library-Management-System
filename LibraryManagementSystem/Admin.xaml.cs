using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        private DataBaseConnection dbConnection;
        private Dictionary<int, Books> books;
        private Dictionary<int, Author> authors;

        private ObservableCollection<BookInfo> bookListBoxItems;


        public Admin()
        {
            InitializeComponent();
            dbConnection = new DataBaseConnection();

            bookListBoxItems = new ObservableCollection<BookInfo>(); 
            bookListBox.ItemsSource = bookListBoxItems;

            LoadBooks();
            LoadAuthors();
           

        }

        private void LoadAuthors()
        {
            authors = dbConnection.GetAuthors();
        }

        private void LoadBooks()
        {

            books = dbConnection.GetBooks();

            foreach (var book in books.Values)
            {
                Author author = dbConnection.GetAuthorById(book.AuthorId);

                BookInfo bookInfo = new BookInfo
                {
                    BookTitle = book.Title,
                    Author = author.AuthorName,
                    AvailableCopies = book.AvailableCopies,
                    TotalCopies = book.TotalCopies
                };

                bookListBoxItems.Add(bookInfo);

                bookInfo.UpdateCopiesText();
            }
        }
        private void addBookButton_Click(object sender, RoutedEventArgs e)
        {
            string authorName = authorTextBox.Text;
            string bookTitle = bookTitleTextBox.Text;
            string totalCopiesText = totalCopiesTextBox.Text;

            if(!string.IsNullOrWhiteSpace(authorName) && 
                !string.IsNullOrWhiteSpace(bookTitle) && 
                int.TryParse(totalCopiesText, out int totalCopies))
            {
                Author existingAuthor = dbConnection.GetAuthorByName(authorName);

                if (existingAuthor == null) 
                {
                    int newAuthorId = dbConnection.AddAuthor(authorName);
                    existingAuthor = new Author { AuthorId = newAuthorId, AuthorName = authorName };
                }

                dbConnection.AddBook(bookTitle, existingAuthor.AuthorId, totalCopies);

                LoadBooks();

                MessageBox.Show($"{bookTitle} has been added to the library.");
            }

            else
            {
                MessageBox.Show("Please provide valid input for author, book title and total copies.");
            }
        }

       

        private void removebookButton_Click(object sender, RoutedEventArgs e)
        {
            BookInfo selectedBook = bookListBox.SelectedItem as BookInfo;

            if (selectedBook != null)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to remove {selectedBook.BookTitle}?",
                    "Confirmation",MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    dbConnection.RemoveBook(selectedBook.BookTitle);
                    bookListBoxItems.Remove(selectedBook);
                }
            }
            else
            {
                MessageBox.Show("Please select a book to remove.");
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = searchTextBox.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                List<BookInfo> searchResults = dbConnection.SearchBooks(searchText);

                UpdateBookList(searchResults);
            }
            else
            {
                LoadBooks();
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(searchTextBox.Text))
            {
                LoadBooks();
            }
        }
        private void UpdateBookList(IEnumerable<BookInfo> books)
        {
            bookListBoxItems.Clear();

            foreach (var book in books)
            {
                bookListBoxItems.Add(book);
                book.UpdateCopiesText(); 
            }
        }

        private void updateBookButton_Click(object sender, RoutedEventArgs e)
        {
            BookInfo selectedBook = bookListBox.SelectedItem as BookInfo;

            if (selectedBook != null)
            {
                
                if (int.TryParse(totalCopiesTextBox.Text, out int newTotalCopies))
                {
                    
                    dbConnection.UpdateBookInfo(selectedBook.BookTitle, newTotalCopies);

                    
                    selectedBook.TotalCopies = newTotalCopies;
                    selectedBook.UpdateCopiesText();
                    bookListBox.Items.Refresh();

                    MessageBox.Show($"{selectedBook.BookTitle} information has been updated.");
                }
                else
                {
                    MessageBox.Show("Please provide valid input for new total copies and new available copies.");
                }
            }
            else
            {
                MessageBox.Show("Please select a book to update.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            this.Hide();
        }
    }
}
