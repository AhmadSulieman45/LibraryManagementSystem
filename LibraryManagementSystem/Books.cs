using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public enum ReqStatus
    {
        Empty,
        Succes,
        Requested
    }
    [Serializable]
    public class Books
    {
        public List<Book> TheBooks
        {
            get;
            set;
        }
        public Dictionary<string, int> CopiesCount
        {
            get;
            set;
        }
        public List<Request> Requests
        {
            get;
            set;
        }
        public List<Request> Reservations
        {
            get;
            set;
        }

        public Books()
        {
            TheBooks = new List<Book>();
            CopiesCount = new Dictionary<string, int>();
            Requests = new List<Request>();
            Reservations = new List<Request>();
        }


        public List<Request> GetBorrowRequests()
        {
            List<Request> ret = new List<Request>();
            foreach (Request req in Requests)
            {
                if (req.CurrentStatus == RequestType.Requested)
                {
                    ret.Add(req);
                }
            }
            return ret;
        }


        private bool CheckIfBorrowed(User user, Book book)
        {
            foreach (Request req in Requests)
            {
                if (req.UserID == user.UserId) return true;
            }
            return false;
        }
        
        private int CalculateReserved(Book book)
        {
            int ret = 0;
            foreach (Request req in Reservations)
            {
                if (req.Isbn == book.Isbn)
                {
                    ret += 1;
                }
            }
            return ret;
        }

        public List<Request> GetBorrowedBy(User user)
        {
            List<Request> ret = new List<Request>();
            foreach (Request req in Requests)
            {
                if (req.UserID == user.UserId && (req.CurrentStatus != RequestType.Denied && req.CurrentStatus != RequestType.ReturnDue))
                {
                    ret.Add(req);
                }
            }
            return ret;
        }

        public ReqStatus AddRequest(User user, Book book, RequestType reqType)
        {
            if (CheckIfBorrowed(user, book))
            {
                return ReqStatus.Requested;
            }

            if (CopiesCount[book.Isbn] - CalculateReserved(book) == 0)
            {
                return ReqStatus.Empty;
            }

            CopiesCount[book.Isbn] -= 1;

            int ReqCnt = 0;
            if (Requests.Count > 0)
            {
                ReqCnt = Requests.Last().Id;
            }

            if (reqType == RequestType.Reservation)
            {
                Reservations.Add(new Request(user, book, DateTime.Now, reqType, ReqCnt++));
            } else
            {
                Requests.Add(new Request(user, book, DateTime.Now, reqType, ReqCnt++));
            }

            return ReqStatus.Succes;
        }

        public void AddBook(Book book)
        {
            if (!CopiesCount.ContainsKey(book.Isbn))
            {
                CopiesCount[book.Isbn] = 0;
                TheBooks.Add(book);
            }
            CopiesCount[book.Isbn] += 1;
        }

        public void DeleteBook(string isbn)
        {
            int index = TheBooks.FindIndex(book => book.Isbn == isbn);
            Debug.Assert(index != -1, "The book doesn't exist");
            TheBooks.RemoveAt(index);
        }

        private void TrackRequest()
        {
            for (int i = 0; i < Requests.Count; ++i) 
            {
                if (Requests[i].CurrentStatus != RequestType.Given)
                {
                    continue;
                }
                TimeSpan elapsed = Requests[i].GetTimeBorrowed();
                int days = (int)elapsed.TotalDays;
                if (days < 14)
                {
                    continue;
                }
                Requests[i].CurrentStatus = RequestType.Due;
            }
        }
        
        public void ReturnBook(Request req)
        {
            TrackRequest();
            if(req.CurrentStatus == RequestType.Due)
            {
                req.CurrentStatus = RequestType.ReturnDue;
                return;
            }
            Requests.RemoveAt(Requests.IndexOf(req));
            AddBook(req.Book);
        }

        public void AddCopy(string isbn)
        {
            CopiesCount[isbn] += 1;
        }

        public void ChangeRequest(Request from, Request to)
        {
            int index = Requests.IndexOf(from);
            Requests[index] = to;
        }

        public bool UpdateBook(string Isbn, Book book)
        {
            int index = TheBooks.FindIndex(b => book.Isbn == b.Isbn);
            
            if (index == -1)
            {
                return false;
            }

            TheBooks[index] = book;

            return true;
        }

        public List<Request> TheRequests
        {
            get
            {
                return Requests;
            }
        }
        
        public List<Request> ReservationRequest
        {
            get
            {
                return Reservations;
            }
        }

        public List<Request> GetBorrowedBooks() {
            List<Request> ret = new List<Request>();
            foreach (Request req in Requests)
            {
                if (req.CurrentStatus == RequestType.Given || req.CurrentStatus == RequestType.Due)
                {
                    ret.Add(req);
                }
            }
            return ret;
        }

        public List<Request> GetDueRequests()
        {
            List<Request> ret = new List<Request>();
            foreach (Request req in Requests)
            {
                if (req.CurrentStatus == RequestType.ReturnDue || req.CurrentStatus == RequestType.Due)
                {
                    ret.Add(req);
                }
            }
            return ret;
        }


        public List<Book> SearchTitle(string txt)
        {
            List<Book> ret = new List<Book>();
            string tmp = StringMatcher.Strip(txt);
            foreach (Book book in TheBooks)
            {
                if (StringMatcher.IsMatch(txt, book.Title) || StringMatcher.IsMatch(book.Title, txt))
                {
                    ret.Add(book);
                    continue;
                }
                foreach (string genre in book.Genres)
                {
                    if (tmp.Contains(StringMatcher.Strip(genre)))
                    {
                        ret.Add(book);
                        break;
                    }
                }
            }
            return ret;
        }

        public List<Book> SearchGenre(string txt)
        {

            List<Book> ret = new List<Book>();
            string tmp = StringMatcher.Strip(txt);
            foreach (Book book in TheBooks)
            {
                foreach (string genre in book.Genres)
                {
                    if (tmp.Contains(StringMatcher.Strip(genre)))
                    {
                        ret.Add(book);
                        break;
                    }
                }
            }
            return ret;
        }

        internal void DeleteDue(User user)
        {
            Requests.RemoveAll(req => req.UserID == user.UserId && (req.CurrentStatus == RequestType.Due || req.CurrentStatus == RequestType.ReturnDue));
        }

        public override string ToString()
        {
            string ret = "";
            for (int i = 0; i < TheBooks.Count; ++i)
            {
                ret += ($"{i + 1}. {TheBooks[i]}");
            }
            return ret;
        }
    }
}
