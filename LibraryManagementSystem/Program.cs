using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{

    class Program
    {
        static MainSystem sys;
        static User curUser;

        static private void Print<T>(T s)
        {
            Console.WriteLine(s);
        }

        static private string Read()
        {
            return Console.ReadLine();
        }


        private static int ReadOption(int limit)
        {
            while (true)
            {
                Print($"Choose an option of the above [1, {limit}]:");
                string t = Read();
                try
                {
                    int op = Convert.ToInt32(t);
                    if (op < 1 || op > limit)
                    {
                        Print($"{op} is not in the range [1, {limit}]");
                        continue;
                    }
                    return op;
                }
                catch
                {
                    Print($"{t} is not a valid option.");
                    continue;
                }
            }
        }

        private static void MainMenu()
        {
            Console.Clear();
            int limit = 7;
            if (curUser.Role == Role.Librarian)
            {
                limit = 14;
            } else if (curUser.Role == Role.Admin)
            {
                limit = 15;
            }

            Print("1. List all books.");
            Print("2. Search books by title.");
            Print("3. Search books by genre.");
            Print("4. List my borrowed Books");
            Print("5. List my fines");
            Print("6. List my messages");
            Print("7. Pay my fines");
            if (curUser.Role != Role.User)
            {
                Print("8. List borrowing requests");
                Print("9. List reservation requests");
                Print("10. List all due requsts.");
                Print("11. List all patreons.");
                Print("12. Generate report on due fines.");
                Print("13. Add a book");
                Print("14. Remove a book.");
                if (curUser.Role == Role.Admin)
                {
                    Print("15. List all users");
                }
            }
            Print($"{limit + 1} Logout");
            int op = ReadOption(limit + 1);
            if (op == limit + 1)
            {
                WelcomePage();
                return;
            }
            switch (op)
            {
                case 1:
                    ListBooks();
                    break;
                case 2:
                    SearchBookTitle();
                    break;
                case 3:
                    SearchBookGenre();
                    break;
                case 4:
                    ListBorrowedBooks();
                    break;
                case 5:
                    ListFines();
                    break;
                case 6:
                    ListMessages();
                    break;
                case 7:
                    PayFines();
                    break;
                case 8:
                    ListBorrowingRequests();
                    break;
                case 9:
                    ListReservations();
                    break;
                case 10:
                    ListDueRequests();
                    break;
                case 11:
                    ListPatreons();
                    break;
                case 12:
                    GenerateFinesReport();
                    break;
                case 13:
                    AddBook();
                    break;
                case 14:
                    RemoveBook();
                    break;
                case 15:
                    ListUsers();
                    break;
            }
        }

        private static void RemoveBook()
        {
            Print("Enter ISBN of book:");
            string isbn = Read();
            sys.DataBase.DeleteBook(isbn);
            Print("Deleted Successfully");
            GoMain();
            MainMenu();
        }

        private static void AddBook()
        {
            Console.Clear();
            Print("Enter Book's title:");
            string name = Read();
            Print("Enter Book's Author:");
            string author = Read();
            Print("Enter Book's ISBN:");
            string isbn = Read();
            Print("Enter Pub Year");
            string year = Read();
            Print("Enter genres seperated by (,):");
            string g = Read();
            string[] gg = g.Split(',');
            List<string> genres = gg.ToList();
            sys.DataBase.AddBook(new Book(name, genres, isbn, author, year));
            Print("Added Successfully");
            GoMain();
            MainMenu();
        }

        private static void ListUsers()
        {

            Console.Clear();
            List<User> users = sys.GetUsers();
            Print(users);
            int n = users.Count;
            Print("1. Message a user");
            Print("2. Modify a user");
            Print("3. Go to the main menu");
            int op = ReadOption(3);
            if (op == 1)
            {
                int idx = ReadOption(n) - 1;
                SendMessage(users[idx]);
                return;
            }
            else if (op == 2)
            {
                int idx = ReadOption(n) - 1;
                if (idx == 0)
                {
                    Print("You can't modify your self");
                    GoMain();
                    MainMenu();
                }
                Print("Enter new email:");
                string email = Read();
                Print("Enter new name:");
                string name = Read();
                Print("Enter a new password:");
                string pass = Read();
                Print("Enter a new phone number:");
                string number = Read();
                Print("Enter a new Roles (1 = Admin,  2 = Librarian, 3 = Patreon): ");
                int r = ReadOption(3);
                Role role = Role.User;
                if (r == 1)
                {
                    role = Role.Admin;
                } else if (r == 2)
                {
                    role = Role.Librarian;
                }
                User temp = users[idx];
                sys.UpdateUser(temp, new User(temp.UserId, name, number, email, pass, role));
                Print("Modified Successfully");
                GoMain();
                MainMenu();
            } else
            {
                MainMenu();
            }
        }

        private static void GenerateFinesReport()
        {
            Console.Clear();
            Print(sys.GetDueReqs());
            Print(sys.GetFines());
        }

        private static void SendMessage(User rec)
        {
            Console.Clear();
            Print($"Sending a message to {rec}");
            Print("Please enter the text of the message:");
            string text = Read();
            sys.SendMessage(curUser, rec, text);
            Print("Message sent successfully.");
            GoMain();
            MainMenu();
        }

        private static void ListPatreons()
        {
            Console.Clear();
            List<User> users = sys.GetUsers(Role.User);
            Print(users);
            int n = users.Count;
            if (n == 0)
            {
                Print("There are no users at the moment.");
                GoMain();
                MainMenu();
            }
            Print("1. Message a user");
            Print("2. Go to the main menu");
            int op = ReadOption(2);
            if (op == 1)
            {
                int idx = ReadOption(n) - 1;
                SendMessage(users[idx]);
                return;
            } else
            {
                MainMenu();
            }
        }

        private static void ListDueRequests()
        {
            Print(sys.GetDueReqs());
            GoMain();
            MainMenu();
        }

        private static void ListReservations()
        {
            Print(sys.GetRequests(curUser, RequestType.Reservation));
            GoMain();
            MainMenu();
        }

        private static void ListBorrowingRequests()
        {
            List<Request> reqs = sys.DataBase.GetBorrowRequests();
            Print(reqs);
            int n = reqs.Count;
            if (n == 0)
            {
                GoMain();
                MainMenu();
                return;
            }
            Print("1. Checkout book.");
            Print("2. Go to the main menu.");
            int op = ReadOption(2);
            if (op == 1)
            {
                int idx = ReadOption(n) - 1;
                sys.CheckOutBook(reqs[idx]);
                Print("Changed Successfully");
                GoMain();
                MainMenu();
            }
            else
            {
                MainMenu();
            }
        }

        private static void PayFines()
        {
            Console.Clear();
            Print($"You have{sys.GetFines(curUser)} fines.");
            Print("Enter an amount to pay (you can only pay the whole amount):");
            string t = "";
            try
            {
                t = Read();
                int x = Convert.ToInt32(t);
                bool ret = sys.PayFine(curUser, x);
                if (ret)
                {
                    Print("Paid fines successfully.");
                } else
                { 
                    Print("An error occured.");
                }
                GoMain();
                MainMenu();
            }
            catch
            {
                Print($"{t} is not a valid number");
                PayFines();
            }
        }

        private static void ListMessages()
        {
            Console.Clear();
            Print(sys.GetMessages(curUser));
            GoMain();
            MainMenu();
        }

        private static void ListFines()
        {
            Console.Clear();
            Print(sys.GetDueReqs(curUser));
            GoMain();
            MainMenu();
        }

        private static void GoMain()
        {
            Print("Print 1 to go back to main menu");
            ReadOption(1);
        }

        private static void CompleteSearch(List<Book> books)
        {
            int n = books.Count;
            if (n == 0)
            {
                Print("No books matches your request. ");
                GoMain();
                MainMenu();
                return;
            }
            Print(books);
            Print("1. Borrow a book");
            Print("2. Reserva a book");
            Print("3. Go to the main menu");
            int op = ReadOption(3);
            if (op == 1)
            {
                int idx = ReadOption(n) - 1;
                ReqStatus ret = sys.RequestBook(curUser, books[idx], RequestType.Requested);
                if (ret == ReqStatus.Succes)
                {
                    Print("Requested the book successfully.");
                    GoMain();
                    MainMenu();
                    return;
                }
                else if (ret == ReqStatus.Empty)
                {
                    Print("No more copies of this book is available.");
                    CompleteSearch(books);
                }
                else
                {
                    Print("You already have a copy of this book");
                }
            } else if (op == 2)
            {
                int idx = ReadOption(n) - 1;
                DateTime userDate;
                Print("Please Enter a date (e.g 10/22/2022):");

                if (!DateTime.TryParse(Console.ReadLine(), out userDate) || DateTime.Now >= userDate)
                {
                    Print("Wrong Date");
                    CompleteSearch(books);
                    return;
                }

                ReqStatus ret = sys.RequestBook(curUser, books[idx], RequestType.Reservation);
                if (ret == ReqStatus.Succes)
                {
                    Print("Requested the book successfully.");
                    sys.GiveLastRequestDate(userDate);
                    GoMain();
                    MainMenu();
                    return;
                }
                else if (ret == ReqStatus.Empty)
                {
                    Print("No more copies of this book is available.");
                    CompleteSearch(books);
                }
                else
                {
                    Print("You already have a copy of this book");
                }
            } else
            {
                MainMenu();
                return;
            }
        }

        private static void SearchBookTitle()
        {
            Console.Clear();
            Print("Please enter a book title:");
            string text = Read();
            List<Book> books = sys.SearchTitle(text);
            CompleteSearch(books);
        }

        private static void SearchBookGenre()
        {
            Console.Clear();
            Print("Please enter a genre (i.e Drama):");
            string text = Read();
            List<Book> books = sys.SearchGenre(text);
            CompleteSearch(books);
        }

        static private void Print<T>(List<T> objs)
        {
            for (int i = 0; i < objs.Count; ++i)
            {
                Print($"{i + 1}. {objs[i]}\n");
            }
        }

        private static void ListBorrowedBooks()
        {
            Console.Clear();
            List<Request> req = sys.GetBorrowedBy(curUser);
            Print(req);
            int n = req.Count;
            if (n == 0)
            {
                Print("No books are available at the moment");
                GoMain();
                MainMenu();
                return;
            }
            Print("1. Return a book.");
            Print("2. Return to main menu");
            Print("Choose one of the above options.");
            int op = ReadOption(2);
            if (op == 1)
            {
                Print($"Enter the number of the book [1, {n}]:");
                int idx = ReadOption(n) - 1;
                sys.ReturnBook(req[idx]);
                ListBorrowedBooks();
                return;
            } else
            {
                MainMenu();
            }
        }


        private static void ListBooks()
        {
            Console.Clear();
            Print(sys.DataBase.TheBooks);
            GoMain();
            MainMenu();
        }

        static private void Login()
        {
            Console.Clear();
            while (true) {
                Print("Please enter an email:");
                string email = Read();
                Print("Please enter the password:");
                string pass = Read();
                Tuple<LoginStatus, User> ret = sys.LoginUser(email, pass);
                if (LoginStatus.Success == ret.Item1)
                {
                    curUser = ret.Item2;
                    Debug.WriteLine(curUser);
                    MainMenu();
                    return;
                } 
                else
                {
                    if (ret.Item1 == LoginStatus.Email404)
                    {
                        Print($"Email:{email} not found in the database.");
                    }
                    else if (ret.Item1 == LoginStatus.EmailInvalid)
                    {
                        Print($"Email: {email} is an inavlid email.");
                    } else
                    {
                        Print($"Password is wrong.");
                    }
                    Print("Try again.");
                }
            }
        }

        private static void Register()
        {

            while (true)
            {
                Print("Enter an email:");
                string email = Read();

                Print("Enter a name (first last):");
                string name = Read();

                Print("Enter a phone number (e.g : 0777445522):");
                string number = Read();

                Print("Enter an password (lowercase, uppercase, number, special character):");
                string password = Read();

                RegisterStatus ret = sys.AddUser(new User(1, name, number, email, password, Role.User));
                
                if (ret == RegisterStatus.Success)
                {
                    Print("Registered Successfully");
                    WelcomePage();
                    return;
                }

                if (ret == RegisterStatus.EmailInvalid)
                {
                    Print($"{email} is an invalid email.");
                }
                else if (ret == RegisterStatus.EmailTaken)
                {
                    Print($"{email} is already taken.");
                } 
                else if (ret == RegisterStatus.PasswordWeak)
                {
                    Print($"The password is weak.");
                } 
                else if (ret == RegisterStatus.InvalidNumber)
                {
                    Print($"{number} is not a valid jordanian mobile number");
                }
                else if (ret == RegisterStatus.InvalidName)
                {
                    Print($"{name} is not a valid name.");
                }
                Print("Please try again.");
            }
        }

        static private void WelcomePage()
        {
            Print("1. Login");
            Print("2. Register");
            int option = ReadOption(2);
            if (option == 1)
            {
                Login();
            } else
            {
                Register();
            }
        }


        static void Main(string[] args)
        {
            sys = new MainSystem();
            WelcomePage();
        }
    }
}
