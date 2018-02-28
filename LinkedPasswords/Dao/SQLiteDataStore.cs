using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedPasswords.Models;
using Innouvous.Utils.Data;
using System.IO;

namespace LinkedPasswords.Dao
{
    class SQLiteDataStore : IDataStore
    {
        private readonly string dbPath;
        private readonly string dbPwd;

        private SQLiteClient client;

        public SQLiteDataStore(string dbPath, string dbPwd)
        {
            this.dbPath = dbPath;
            this.dbPwd = dbPwd;
        }
        
        public void Open()
        {
            var isNew = !File.Exists(dbPath);

            this.client = new SQLiteClient(dbPath, !isNew);

            var connection = client.GetConnection();
            if (isNew)
            {
                connection.ChangePassword(dbPwd);
                CreateTables();
            }
            else
            {
                connection.Close();
                connection.SetPassword(dbPwd);
                connection.Open();
                //connection.ChangePassword(dbPwd);
            }

            client.ExecuteNonQuery("select * from " + TablePasswords + " limit 1");
        }

        private const string TableEntries = "tblEntries";
        private const string TablePasswords = "tblPasswords";

        private void CreateTables()
        {
            StringBuilder sb = new StringBuilder();
            string sql;

            //EntryTable
            sb.AppendLine("CREATE TABLE " + TableEntries + " (");
            sb.AppendLine("Id           integer PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("Name         text NOT NULL,");
            sb.AppendLine("Url          text,");
            sb.AppendLine("PasswordId   integer NOT NULL,");
            sb.AppendLine("UNIQUE (Name),");
            sb.AppendLine("FOREIGN KEY (PasswordId) REFERENCES " + TablePasswords  + "(Id) ON DELETE CASCADE ON UPDATE CASCADE");
            sb.AppendLine(");");

            sql = sb.ToString();
            client.ExecuteNonQuery(sql);

            //PasswordsTable
            sb.Clear();
            sb.AppendLine("CREATE TABLE " + TablePasswords + " (");
            sb.AppendLine("Id           integer PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("Name         text NOT NULL,");
            sb.AppendLine("Username     text,");
            sb.AppendLine("Password     text NOT NULL,");
            sb.AppendLine("UNIQUE (Name)");
            sb.AppendLine(");");

            sql = sb.ToString();
            client.ExecuteNonQuery(sql);
        }

        public void Close()
        {
            client.Close();
        }


        public void AddEntry(Entry i)
        {
            throw new NotImplementedException();
        }

        public void AddPassword(PasswordItem i)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntry(Entry i)
        {
            throw new NotImplementedException();
        }

        public void DeletePassword(PasswordItem i)
        {
            throw new NotImplementedException();
        }

        public List<Entry> GetEntries()
        {
            throw new NotImplementedException();
        }

        public List<PasswordItem> GetPasswords()
        {
            throw new NotImplementedException();
        }

        
        public void UpdateEntry(Entry i)
        {
            throw new NotImplementedException();
        }

        public void UpdatePassword(PasswordItem i)
        {
            throw new NotImplementedException();
        }
    }
}
