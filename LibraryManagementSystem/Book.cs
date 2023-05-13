using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    [Serializable]
    public class Book
    {

        public string Title
        {
            get;
            set;
        }

        public string Isbn
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string PubYear
        {
            get;
            set;
        }

        public List<string> Genres {
            get;
            set;
        }

        public Book()
        {

        }

        public Book(string title, List<string> genres, string isbn, string author, string pubYear)
        {
            Title = title;
            Genres = genres;
            Isbn = isbn;
            Author = author;
            PubYear = pubYear;
        }


        public override string ToString()
        {
            string gen = String.Join(", ", Genres);
            return $"Title: {Title}.\nGenres: {gen}.\nAuthor:{Author}.\nPublication Year:{PubYear}\nISBN:{Isbn}.\n";
        }

    }
}
