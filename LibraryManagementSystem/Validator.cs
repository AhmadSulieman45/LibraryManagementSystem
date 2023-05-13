using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace LibraryManagementSystem
{
    static class Validator
    {
        internal static bool CheckEmail(string email)
        {
            Regex r = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return r.IsMatch(email);
        }

        internal static bool CheckName(string name)
        {
            Regex r = new Regex(@"\b([A-ZÀ-ÿ][-,a-z. ']+[ ]*)+");
            return r.IsMatch(name);
        }

        internal static bool CheckPassword(string password)
        {
            Regex r = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])(?=.{8,})");
            return r.IsMatch(password);
        }

        internal static bool CheckNumber(string phoneNumber)
        {
            Regex r = new Regex(@"((079)|(078)|(077)){1}[0-9]{7}");
            return r.IsMatch(phoneNumber);
        }
    }
}
