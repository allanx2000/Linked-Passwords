using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedPasswords.Models
{
    class Entry
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int PasswordId { get; set; }
    }
}
