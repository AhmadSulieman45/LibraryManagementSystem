using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public static class StringMatcher
    {
        static public string Strip(string s)
        { 
            return s.Replace(" ", "").ToLower();
        }

        static public bool IsMatch(string s, string t)
        {
            t = Strip(t);
            string[] words = s.Split(' ');
            int cnt = 0;
            foreach (string word in words)
            {
                string tw = Strip(word);
                if (t.Contains(tw))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
