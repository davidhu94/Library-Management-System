using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class Books
    {
        public int Id {  get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }
        public Author BookAuthor { get; set; }

        public Books(int id, string title, int authorId, int availableCopies, int totalCopies)
        {
            Id = id;
            Title = title;
            AuthorId = authorId;
            AvailableCopies = availableCopies;
            TotalCopies = totalCopies;
        }
        public Books() { }
    }
}
