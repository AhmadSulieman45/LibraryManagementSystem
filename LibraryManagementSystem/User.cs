using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    [Serializable]
    public class User 
    {

        public int UserId 
        { 
            get;
            set;
        }
        
        public string Name
        {
            get;
            set;
        }

        public string PhoneNumber
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }
        
        public Role Role
        {
            get;
            set;
        }

        public User() { 
        }

        public User(int uId, string name, string phoneNum, string email, string pass, Role role)
        {
            UserId = uId;
            Name = name;
            PhoneNumber = phoneNum;
            Email = email;
            Password = pass;
            Role = role;
        }

        public override string ToString()
        {
            string role = (Role == Role.Admin ? "Admin" : Role == Role.Librarian ? "Librarian" : "Patreon");
            return $"Name: {Name},\t User ID:{UserId},\t Phone Number:{PhoneNumber},\t Email: {Email}, Role: {role}\n";
        }

    }
}
