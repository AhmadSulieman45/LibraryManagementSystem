using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
namespace LibraryManagementSystem
{

    public enum Status {
        Requested,
        Denied,
        Given,
        Due
    }

    class Request
    {
        public DateTime GivenDate
        {
            get;
            private set;
        }

        public DateTime ReqDate
        {
            get;
            private set;
        }


        private User _user;
        private Book _book;
        
        public int UserID
        {
            get { return _user.UserId; }
        }

        public string BookTitle
        {
            get { return _book.Title; }
        }

        public int Id
        {
            get;
            private set;
        }

        public Status CurrentStatus
        {
            get;
            set;
        }

        public Request(User user, Book book, DateTime date, int reqId)
        {
            this._user = user;
            this._book = book;
            ReqDate = date;
            Id = reqId;
            CurrentStatus = Status.Requested;
        }

        public TimeSpan getRemainingTime()
        {
            Debug.Assert(CurrentStatus == Status.Denied || CurrentStatus == Status.Requested, "This request should be granted before checking remaining time");
            return DateTime.Now - GivenDate;
        }
    }
}
