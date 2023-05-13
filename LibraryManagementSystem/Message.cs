using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    [Serializable]
    public class Message
    {
        public User Receiver
        {
            get;
            set;
        }

        public User Sender
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public Message()
        {

        }

        public Message(User sender, User rec, string txt)
        {
            Sender = sender;
            Receiver = rec;
            Text = txt;
            Date = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Sender.Name} ({Date.ToString("dddd, dd MMMM yyyy")}): {Text}\n";
        }
    }
}
