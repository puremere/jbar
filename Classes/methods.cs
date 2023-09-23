using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jbar.Classes
{
    public static class methods
    {
        public static string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}