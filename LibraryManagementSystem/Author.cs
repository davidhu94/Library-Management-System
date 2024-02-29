using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }

        public Author(int authorId, string authorName)
        {
            AuthorId = authorId;
            AuthorName = authorName;
        }

        public Author() { }
    }
}
