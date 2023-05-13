using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public static class Constants
    {
        public const int BorrowTime = 14; // in days
        public const int DuePenalty = 1; // dollars or something
        public const int RemindFreq = 1; // in days
 /*
        public const string UsersPath = @"\Users\Public\Documents\LibData\users.xml";
        public const string DataBasePath = @"\Users\Public\Documents\LibData\database.xml";
        public const string Messages = @"\Users\Public\Documents\LibData\messages.xml";
        public const string UID = @"\Users\Public\Documents\LibData\uid.xml";*/
        public const string UsersPath = @"users.dat";
        public const string DataBasePath = @"database.dat";
        public const string Messages = @"messages.dat";
        public const string UID = @"uid.dat";
    }
}
