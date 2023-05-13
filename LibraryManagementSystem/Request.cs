using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
namespace LibraryManagementSystem
{
    public enum RequestType {
        Requested,
        Denied,
        Reservation,
        Given,
        Due,
        ReturnDue
    }

    [Serializable]
    public class Request
    {
        public DateTime GivenDate
        {
            get;
            set;
        }

        public DateTime ReqDate
        {
            get;
            set;
        }
    
        public DateTime RemindedTime
        {
            get;
            set;
        }

        public DateTime ReturnDate
        {
            get;
            set;
        }

        private User _user;
        private Book _book;

        public User User
        {
            get;
        }


        public int UserID
        {
            get 
            { 
                return _user.UserId;
            }
        }

        public string BookTitle
        {
            get 
            { 
                return _book.Title;
            }
        }

        public string Isbn
        {
            get 
            { 
                return _book.Isbn;
            }
        }

        public int Id
        {
            get;
            set;
        }

        public RequestType CurrentStatus
        {
            get;
            set;
        } 

        public Request()
        {

        }

        public Request(User user, Book book, DateTime date, RequestType reqType, int reqId)
        {
            this._user = user;
            this._book = book;
            ReqDate = date;
            Id = reqId;
            CurrentStatus = reqType;
            RemindedTime = date;
        }



        public TimeSpan GetTimeBorrowed()
        {
            Debug.Assert(CurrentStatus == RequestType.Denied || CurrentStatus == RequestType.Requested || CurrentStatus == RequestType.Reservation, "This request should be granted before checking remaining time");
            return (CurrentStatus == RequestType.ReturnDue ? ReturnDate : DateTime.Now) - GivenDate;
        }

        public override string ToString()
        {
            TimeSpan elapsed = GetTimeBorrowed();
            if (CurrentStatus == RequestType.Requested || CurrentStatus == RequestType.Reservation)
            {
                return $"Book: {BookTitle}, was {(CurrentStatus == RequestType.Reservation ? "Reserved" : "Requested")} on {ReqDate.ToString("dddd, dd MMMM yyyy")}.\n";
            }
            else if (CurrentStatus == RequestType.Denied)
            {
                return $"Book: {BookTitle}, was requested on {ReqDate.ToString("dddd, dd MMMM yyyy")} and denied.\n";
            }
            else if (CurrentStatus == RequestType.Due)
            {
                return $"Book: {BookTitle}, is due since {elapsed.TotalDays} days, fine is {(int)(elapsed.TotalDays - Constants.BorrowTime)* Constants.DuePenalty}.\n";
            }
            else {
                return $"Book: {BookTitle}, is borrowed since {(int)(elapsed.TotalDays - Constants.BorrowTime) * Constants.DuePenalty}.\n";
            }
        }
    }
}
