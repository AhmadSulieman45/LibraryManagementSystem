using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    class User
    {

        public int UserId 
        { 
            get;
            private set;
        }
        
        public string Name
        {
            get;
            private set;
        }

        public string PhoneNumber
        {
            get;
            private set;
        }

        public string Email
        {
            get;
            private set;
        }

        public User(int uId, string name, string phoneNum, string email)
        {
            UserId = uId;
            Name = name;
            PhoneNumber = phoneNum;
            Email = email;
        }
    }
}
