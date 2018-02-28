using LinkedPasswords.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedPasswords.Dao
{
    interface IDataStore
    {
        void Open();
        void Close();

        List<PasswordItem> GetPasswords();
        void AddPassword(PasswordItem i);
        void UpdatePassword(PasswordItem i);
        void DeletePassword(PasswordItem i);

        List<Entry> GetEntries();
        void AddEntry(Entry i);
        void UpdateEntry(Entry i);
        void DeleteEntry(Entry i);

    }
}
