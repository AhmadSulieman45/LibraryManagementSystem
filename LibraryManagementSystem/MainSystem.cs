using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LibraryManagementSystem
{
    public enum Role { 
        Admin,
        Librarian,
        User
    }

    public enum RegisterStatus
    {
        EmailTaken,
        EmailInvalid,
        PasswordWeak,
        InvalidName,
        InvalidNumber,
        Success
    }

    public enum LoginStatus
    {
        Success,
        WrongPassword,
        Email404,
        EmailInvalid,
    }

    public class MainSystem
    {
        private int _uId;
        private List<User> _users;
        private Books _database;
        private FineManager _fineManager;
        private List<Message> _messages;

        public Books DataBase 
        {
            get { return _database;  }
        }
        private bool Load<T>(string path, ref T obj)
        {
            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    obj = (T)bf.Deserialize(fs);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        public void SendMessage(User sender, User rec, string txt)
        {
            _messages.Add(new Message(sender, rec, txt));
            Debug.WriteLine(_messages.Last());
            Debug.WriteLine(sender);
            Debug.WriteLine(rec);
            Debug.WriteLine(txt);
            Debug.WriteLine(_messages);
            Save(Constants.Messages, _messages);
        }

        private void SendRemainders()
        {
            List<Request> reqs = _database.GetDueRequests();
            _fineManager.TrackFees(reqs);
            foreach (Request req in reqs)
            {
                TimeSpan elapsed = DateTime.Now - req.RemindedTime;
                int days = (int)elapsed.TotalDays;
                if (days >= Constants.RemindFreq)
                {
                    SendMessage(_users.First(), req.User, $"You have due fees on book ({req.BookTitle}), and it was due for {req.GetTimeBorrowed().TotalDays} days.");
                    Request temp = req;
                    temp.RemindedTime = DateTime.Now;
                    _database.ChangeRequest(req, temp);
                }
            }
        }

        public List<User> GetUsers()
        {
            return _users;
        }

        public MainSystem()
        {
            _uId = 0;
            if (!Load(Constants.UsersPath, ref _users))
            {
                _users = new List<User>
                {
                    new User(0, "admin", "0777442233", "admin@admin.com", "admin", Role.Admin)
                };
            }
            else
            {
                Load(Constants.UID, ref _uId);
            }

            if (!Load(Constants.DataBasePath, ref _database))
            {
                _database = new Books();
            }

            if (!Load(Constants.Messages, ref _messages))
            {
                Debug.WriteLine("No messages loaded");
                _messages = new List<Message>();
            }

            _fineManager = new FineManager();
            _fineManager.TrackFees(_database.TheRequests);
            SendRemainders();
        }

        public Dictionary<int, int> GetFines()
        {
            return _fineManager.DueFees;
        }
        public List<Request> GetDueReqs()
        {
            return DataBase.GetDueRequests();
        }
        public List<Message> GetMessages(User user)
        {
            List<Message> ret = new List<Message>();
            foreach (Message msg in _messages) {
                if (msg.Receiver.UserId == user.UserId)
                {
                    ret.Add(msg);
                }
            }
            return ret;
        }

        public RegisterStatus AddUser(User user)
        {
            if (!Validator.CheckEmail(user.Email)) return RegisterStatus.EmailInvalid;
            
            if (!Validator.CheckName(user.Name)) return RegisterStatus.InvalidName;

            if (!Validator.CheckPassword(user.Password)) return RegisterStatus.PasswordWeak;

            if (!Validator.CheckNumber(user.PhoneNumber)) return RegisterStatus.InvalidNumber;

            foreach (User usr in _users)
            {
                if (usr.Email == user.Email)
                {
                    return RegisterStatus.EmailTaken;
                }
            }

            user.UserId = ++_uId;

            _users.Add(user);
            Save(Constants.UsersPath, _users);


            return RegisterStatus.Success;
        }

        public Tuple<LoginStatus, User> LoginUser(string email, string password)
        {
            if (!Validator.CheckEmail(email)) return new Tuple<LoginStatus, User>(LoginStatus.EmailInvalid, null);
            email = email.ToLower();
            bool foundEmail = false;
            foreach (User user in _users)
            {
                if (user.Email == email)
                {
                    foundEmail = true;
                    if (user.Password == password)
                    {
                        return new Tuple<LoginStatus, User> (LoginStatus.Success, user);
                    }
                }
            }
            
            if (foundEmail)
            {
                return new Tuple<LoginStatus, User>(LoginStatus.WrongPassword, null);
            }

            return new Tuple<LoginStatus, User>(LoginStatus.Email404, null);
        }

        public void ReturnBook(Request req)
        {
            _database.ReturnBook(req);
            _database.AddCopy(req.Book.Isbn);
            Save(Constants.DataBasePath, _database);
        }

        public void AddCopy(string isbn)
        {
            _database.AddCopy(isbn);
            Save(Constants.DataBasePath, _database);
        }

        public List<Request> GetRequests(User user, RequestType reqType)
        {
            List<Request> ret = new List<Request>();
            foreach (Request req in _database.TheRequests)
            {
                if (req.UserID != user.UserId || req.CurrentStatus != reqType) continue;
                ret.Add(req);
            }
            return ret;
        }

        public List<User> GetUsers(Role role)
        {
            List<User> ret = new List<User>();
            foreach (User user in _users)
            {
                if (user.Role == role) {
                    ret.Add(user);
                }
            }
            return ret;
        }

        public List<Request> GetDueReqs(User user)
        {
            List<Request> reqs = DataBase.GetDueRequests();
            List<Request> ret = new List<Request>();
            foreach (Request req in reqs)
            {
                if (req.UserID == user.UserId)
                {
                    ret.Add(req);
                }
            }
            return ret;
        }

        public int GetFines(User user)
        {
            _fineManager.TrackFees(_database.TheRequests);
            if (_fineManager.DueFees.ContainsKey(user.UserId))
            {
                return _fineManager.DueFees[user.UserId];
            }
            return 0;
        }

        public bool DeleteUser(int uId)
        {
            int idx = _users.FindIndex(usr => usr.UserId == uId);
            if (idx == -1) return false;
            _users.RemoveAt(idx);
            Save(Constants.DataBasePath, _users);
            return true;
        }

        public void UpdateUser(User from, User to)
        {
            int idx = _users.FindIndex(usr => usr == from);
            _users[idx] = to;
            Save(Constants.UsersPath, _users);
        }
        public bool SetRole(ref User user, Role role)
        {
            if (user.Role == Role.Admin) return false;
            user.Role = role;
            Save(Constants.UsersPath, _users);
            return true;
        }

        public void CheckOutBook(Request req)
        {
            Request temp = req;
            req.CurrentStatus = RequestType.Given;
            req.GivenDate = DateTime.Now;
            DataBase.ChangeRequest(temp, req);

            Save(Constants.DataBasePath, DataBase);
        }

        public List<Book> SearchTitle(string txt)
        {
            return _database.SearchTitle(txt);
        }

        public List<Book> SearchGenre(string txt)
        {
            return _database.SearchGenre(txt);
        }

        public void AddBook(Book book)
        {
            DataBase.AddBook(book);
            Save(Constants.DataBasePath, DataBase);
        }

        public void DeleteBook(string isbn)
        {
            DataBase.DeleteBook(isbn);
            Save(Constants.DataBasePath, DataBase);
        }
        private void Save<T>(string path, T obj)
        {
            using (Stream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);

            }
        }

        public List<Request> GetBorrowedBy(User user)
        {
            return _database.GetBorrowedBy(user);
        }

        public bool PayFine(User user, int amount)
        {
            _fineManager.TrackFees(_database.GetDueRequests());
            if(_fineManager.PayFine(user.UserId, amount) != PaymentStatus.Invalid)
            {
                _database.DeleteDue(user);
                return true;
            }
            Save(Constants.DataBasePath, DataBase);
            return false;
        }

        public ReqStatus RequestBook(User user, Book book, RequestType reqType)
        {
            Save(Constants.DataBasePath, _database);
            ReqStatus status = _database.AddRequest(user, book, reqType);
            Save(Constants.DataBasePath, _database);
            return status;
        }

        public void GiveLastRequestDate(DateTime date)
        {
            DataBase.Reservations.Last().ReqDate = date;
            Save(Constants.DataBasePath, DataBase);
        }

        ~MainSystem()
        {
            Save(Constants.UsersPath, _users);
            Save(Constants.DataBasePath, _database);
            Save(Constants.Messages, _messages);
        }
    }
}
