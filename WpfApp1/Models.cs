using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class Models
    {
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsPaidUser { get; set; }
        }

        public class Document
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
    }
}
