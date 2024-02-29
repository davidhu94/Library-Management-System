using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class BorrowedBooks
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public int BorrowerId { get; set; }
        public string BorrowDate { get; set; }
        public string ReturnDate { get; set; }

    }
}
