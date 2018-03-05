using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedPasswords.Models
{
    public class PasswordItem
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
