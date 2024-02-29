using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class DataBaseConnection
    {
        string server = "localhost";
        string database = "library_management_system";
        string username = "library_app";
        string password = "password";

        string connectionString = "";


        public DataBaseConnection()
        {
            connectionString =
                "SERVER=" + server + ";" +
                "DATABASE=" + database + ";" +
                "UID=" + username + ";" +
                "PASSWORD=" + password + ";";
        }
        public DataBaseConnection(string server, string database, string username, string password)
        {
            connectionString =
                "SERVER=" + server + ";" +
                "DATABASE=" + database + ";" +
                "UID=" + username + ";" +
                "PASSWORD=" + password + ";";
        }


        public Dictionary<int, Books> GetBooks()
        {
            Dictionary<int, Books> books = new Dictionary<int, Books>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string booksQuery = "SELECT * FROM books;";
                MySqlCommand booksCommand = new MySqlCommand(booksQuery, connection);
                MySqlDataReader booksReader = booksCommand.ExecuteReader();

                while (booksReader.Read())
                {
                    int id = Convert.ToInt32(booksReader["book_id"]);
                    string title = booksReader["book_title"].ToString();
                    int authorId = Convert.ToInt32(booksReader["author_id"]);
                    int availableCopies = Convert.ToInt32(booksReader["available_copies"]);
                    int totalCopies = Convert.ToInt32(booksReader["total_copies"]);

                    Books book = new Books(id, title, authorId, availableCopies, totalCopies);
                    books.Add(id, book);
                }
            }
            return books;
        }

        public Author GetAuthorById(int authorId)
        {
            Author author = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string authorQuery = $"SELECT * FROM author WHERE author_id = {authorId}";
                MySqlCommand authorCommand = new MySqlCommand(authorQuery, connection);
                MySqlDataReader authorReader = authorCommand.ExecuteReader();

                if (authorReader.Read())
                {
                    int id = Convert.ToInt32(authorReader["author_id"]);
                    string name = authorReader["author_name"].ToString();
                    author = new Author { AuthorId = id, AuthorName = name };
                }
            }

            return author;
        }

        public void UpdateAvailableCopies(string bookTitle, int newAvailableCopies)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = $"UPDATE books SET available_copies = {newAvailableCopies} WHERE book_title = '{bookTitle}'";
                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                updateCommand.ExecuteNonQuery();
            }
        }
        public int AddAuthor(string authorName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string addAuthorQuery = $"INSERT INTO author (author_name) VALUES ('{authorName}'); SELECT LAST_INSERT_ID();";
                MySqlCommand addAuthorCommand = new MySqlCommand(addAuthorQuery, connection);
                int newAuthorId = Convert.ToInt32(addAuthorCommand.ExecuteScalar());

                return newAuthorId;

            }
        }

        public void AddBook(string bookTitle, int authorId, int totalCopies)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string addBookQuery = $"INSERT INTO books (book_title, author_id, available_copies, total_copies) " +
                    $"VALUES ('{bookTitle}', {authorId}, {totalCopies}, {totalCopies});";

                MySqlCommand addBookCommand = new MySqlCommand(addBookQuery, connection);
                addBookCommand.ExecuteNonQuery();
            }
        }
        public int GetAuthorIdByName(string authorName)
        {
            int authorId = -1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string authorQuery = $"SELECT author_id FROM author WHERE author_name = '{authorName}'";
                MySqlCommand authorCommand = new MySqlCommand(authorQuery, connection);

                var result = authorCommand.ExecuteScalar();
                if (result != null)
                {
                    authorId = Convert.ToInt32(result);
                }
            }

            return authorId;
        }

        public Dictionary<int, Author> GetAuthors()
        {
            Dictionary<int, Author> authors = new Dictionary<int, Author>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string authorQuery = "SELECT * FROM author;";
                MySqlCommand authorCommand = new MySqlCommand(authorQuery, connection);
                MySqlDataReader authorReader = authorCommand.ExecuteReader();

                while (authorReader.Read())
                {
                    int id = Convert.ToInt32(authorReader["author_id"]);
                    string authorName = authorReader["author_name"].ToString();

                    Author author = new Author(id, authorName);
                    authors.Add(id, author);
                }
            }
            return authors;
        }

        
        public void RemoveBook(string bookTitle)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int bookId = GetBookIdByTitle(bookTitle);

                            string deletePublishedByQuery = $"DELETE FROM published_by WHERE book_id = {bookId};";
                            using (MySqlCommand deletePublishedByCommand = new MySqlCommand(deletePublishedByQuery, connection, transaction))
                            {
                                deletePublishedByCommand.ExecuteNonQuery();
                            }

                            string deleteBookQuery = $"DELETE FROM books WHERE book_id = {bookId};";
                            using (MySqlCommand deleteBookCommand = new MySqlCommand(deleteBookQuery, connection, transaction))
                            {
                                deleteBookCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            Console.WriteLine($"Book '{bookTitle}' has been successfully removed.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error (Number: {ex.Number}): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private int GetBookIdByTitle(string bookTitle)
        {
            int bookId = -1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string authorQuery = $"SELECT book_id FROM books WHERE book_title = '{bookTitle}'";
                MySqlCommand bookCommand = new MySqlCommand(authorQuery, connection);
                MySqlDataReader bookReader = bookCommand.ExecuteReader();

                if (bookReader.Read())
                {
                    bookId = Convert.ToInt32(bookReader["book_id"]);
                }
            }

            return bookId;
        }

        public Author GetAuthorByName(string authorName)
        {
            Author author = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string authorQuery = $"SELECT * FROM AuthorsView WHERE AuthorName = '{authorName}'";
                MySqlCommand authorCommand = new MySqlCommand(authorQuery, connection);
                MySqlDataReader authorReader = authorCommand.ExecuteReader();

                if (authorReader.Read())
                {
                    int id = Convert.ToInt32(authorReader["AuthorId"]);

                    author = new Author { AuthorId = id, AuthorName = authorName };
                }
            }

            return author;
        }

        public List<BookInfo> SearchBooks(string searchText)
        {
            List<BookInfo> searchResults = new List<BookInfo>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand searchCommand = new MySqlCommand("SearchBooks", connection))
                {
                    searchCommand.CommandType = CommandType.StoredProcedure;

                    searchCommand.Parameters.AddWithValue("@search_text", searchText);

                    using (MySqlDataReader searchReader = searchCommand.ExecuteReader())
                    {
                        while (searchReader.Read())
                        {
                            int bookId = Convert.ToInt32(searchReader["book_id"]);
                            string bookTitle = searchReader["book_title"].ToString();
                            int authorId = Convert.ToInt32(searchReader["author_id"]);
                            string authorName = searchReader["author_name"].ToString();
                            int availableCopies = Convert.ToInt32(searchReader["available_copies"]);
                            int totalCopies = Convert.ToInt32(searchReader["total_copies"]);

                            BookInfo book = new BookInfo
                            {
                                BookId = bookId,
                                BookTitle = bookTitle,
                                AuthorId = authorId,
                                AuthorName = authorName,
                                AvailableCopies = availableCopies,
                                TotalCopies = totalCopies,
                                _copiesText = $"{availableCopies}/{totalCopies}"
                            };

                            searchResults.Add(book);
                        }
                    }
                }
            }

            return searchResults;
        }

        public void UpdateBookInfo(string bookTitle, int totalCopies)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = $"UPDATE books SET total_copies = {totalCopies}, available_copies = {totalCopies} WHERE book_title = '{bookTitle}'";
                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                updateCommand.ExecuteNonQuery();
            }
        }

        public List<Borrower> GetBorrowers()
        {
            List<Borrower> borrowers = new List<Borrower>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM borrowers";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Borrower borrower = new Borrower
                            {
                                Id = Convert.ToInt32(reader["borrower_id"]),
                                Name = reader["borrower_name"].ToString()
                            };

                            borrowers.Add(borrower);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
            }

            return borrowers;
        }
        public void BorrowBook(int bookId, int borrowerId, DateTime borrowDate, 
        DateTime returnDate)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();


                string checkExistingQuery = $"SELECT COUNT(*) FROM borrowed_books WHERE book_id = {bookId} AND borrower_id = {borrowerId}";
                using (MySqlCommand checkExistingCommand = new MySqlCommand(checkExistingQuery, connection))
                {
                    int existingCount = Convert.ToInt32(checkExistingCommand.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        Console.WriteLine($"Book with ID {bookId} has already been borrowed by borrower with ID {borrowerId}.");

                        return;
                    }
                }

                string borrowDateStr = borrowDate.ToString();
                string returnDateStr = returnDate.ToString();

                string insertQuery = $"INSERT INTO borrowed_books (book_id, borrower_id, borrow_date, return_date) " +
                                     $"VALUES ({bookId}, {borrowerId}, '{borrowDateStr}', '{returnDateStr}');";

                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public void ReturnBook(int bookId, int borrowerId, DateTime returnDate)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                
                string checkExistingQuery = $"SELECT COUNT(*) FROM borrowed_books WHERE book_id = {bookId} AND borrower_id = {borrowerId}";
                using (MySqlCommand checkExistingCommand = new MySqlCommand(checkExistingQuery, connection))
                {
                    int existingCount = Convert.ToInt32(checkExistingCommand.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        
                        string returnQuery = $"DELETE FROM borrowed_books WHERE book_id = {bookId};";



                        using (MySqlCommand returnCommand = new MySqlCommand(returnQuery, connection))
                        {
                            returnCommand.ExecuteNonQuery();
                        }

                        
                        string updateCopiesQuery = $"UPDATE books SET available_copies = available_copies + 1 WHERE book_id = {bookId}";
                        using (MySqlCommand updateCopiesCommand = new MySqlCommand(updateCopiesQuery, connection))
                        {
                            updateCopiesCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine($"Book with ID {bookId} has been returned by borrower with ID {borrowerId}.");
                    }
                    else
                    {
                        
                        Console.WriteLine($"Book with ID {bookId} has not been borrowed by borrower with ID {borrowerId}.");
                    }
                }
            }
        }

    }

    }

