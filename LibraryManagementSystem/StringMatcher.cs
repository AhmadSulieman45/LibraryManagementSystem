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
            s = s.Replace(" ", "");
            s = s.ToLower();
            return s;
        }

        static public bool IsMatch(string s, string t)
        {
            Strip(t);
            string[] words = s.Split(' ');
            int cnt = 0;
            foreach (string word in words)
            {
                if (t.Contains(word))
                {
                    cnt += 1;
                }
            }
            int m = s.Length;
            return cnt >= (m + 2) / 3;
        }
    }
}
