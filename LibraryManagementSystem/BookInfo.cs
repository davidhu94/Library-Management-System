using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class BookInfo
    {
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }

        public string _copiesText;

        public string CopiesText
        {
            get
            {
                return _copiesText;
            }
            private set
            {
                _copiesText = value;
            }
        }

        public void UpdateCopiesText()
        {
            CopiesText = $"Available Copies: {AvailableCopies}/{TotalCopies}";
        }

        public override string ToString()
        {
            return $"{BookTitle} by {Author}";
        }
    }
}
