using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    class Book
    {
        
        public string Title
        {
            get;
            private set;
        }

        public string Isbn
        {
            get;
            private set;
        }
        
        public string Author
        {
            get;
            private set;
        }

        public int PubYear
        {
            get;
            private set;
        }

        private List<string> _genres;
        public Book(string title, List<string> genres, string isbn, string author, int pubYear)
        {
            Title = title;
            this._genres = genres;
            Isbn = isbn;
            Author = author;
            PubYear = pubYear;
        }
        
    }
}
