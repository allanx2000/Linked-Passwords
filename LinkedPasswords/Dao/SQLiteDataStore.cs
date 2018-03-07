using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedPasswords.Models;
using Innouvous.Utils.Data;
using System.IO;
using System.Data;

namespace LinkedPasswords.Dao
{
    class SQLiteDataStore : IDataStore
    {
        private readonly string dbPath;
        private readonly string dbPwd;
        private readonly bool usePassword;

        private SQLiteClient client;


        public SQLiteDataStore(string dbPath, string dbPwd, bool usePassword = true)
        {
            this.dbPath = dbPath;
            this.dbPwd = dbPwd;
            this.usePassword = usePassword;
        }

        public void Open()
        {
            var isNew = !File.Exists(dbPath);

            var args = new Dictionary<string, string>() { { "foreign keys", "true" } };
            this.client = new SQLiteClient(dbPath, !isNew, args);

            var connection = client.GetConnection();
            if (isNew)
            {
                if (usePassword)
                    connection.ChangePassword(dbPwd);

                CreateTables();
            }
            else
            {
                if (usePassword)
                {
                    connection.Close();
                    connection.SetPassword(dbPwd);
                    connection.Open();
                }
            }

            client.ExecuteNonQuery("select * from " + TablePasswords + " limit 1");
        }

        private const string TableEntries = "tblEntries";
        private const string TablePasswords = "tblPasswords";

        private void CreateTables()
        {
            StringBuilder sb = new StringBuilder();
            string sql;

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

            //EntryTable
            sb.Clear();
            sb.AppendLine("CREATE TABLE " + TableEntries + " (");
            sb.AppendLine("Id           integer PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("Name         text NOT NULL,");
            sb.AppendLine("Url          text,");
            sb.AppendLine("PasswordId   integer,");
            sb.AppendLine("UNIQUE (Name),");
            sb.AppendLine("FOREIGN KEY (PasswordId) REFERENCES " + TablePasswords + "(Id) \nON DELETE SET NULL \nON UPDATE CASCADE");
            sb.AppendLine(");");

            sql = sb.ToString();
            client.ExecuteNonQuery(sql);

        }

        public void Close()
        {
            client.Close();
        }


        private const string cAddEntry = "insert into {0} values(null,'{1}','{2}',{3})";

        public void AddEntry(Entry i)
        {
            string cmd = string.Format(cAddEntry, TableEntries,
                SQLUtils.SQLEncode(i.Name),
                SQLUtils.SQLEncode(i.URL),
                i.CredentialId
            );

            client.ExecuteNonQuery(cmd);
            i.ID = SQLUtils.GetLastInsertRow(client);
        }

        private const string cAddPassword = "insert into {0} values(null,'{1}','{2}','{3}')";

        public void AddPassword(PasswordItem i)
        {
            string cmd = string.Format(cAddPassword, TablePasswords,
                 SQLUtils.SQLEncode(i.Name),
                 SQLUtils.SQLEncode(i.Username),
                 SQLUtils.SQLEncode(i.Password)
             );

            client.ExecuteNonQuery(cmd);
            i.ID = SQLUtils.GetLastInsertRow(client);
        }



        public void DeleteEntry(Entry i)
        {
            client.ExecuteNonQuery("delete from " + TableEntries + " where Id = " + i.ID);
        }

        public void DeletePassword(PasswordItem i)
        {
            client.ExecuteNonQuery("delete from " + TablePasswords + " where Id = " + i.ID);
        }

        public List<Entry> GetEntries()
        {
            var results = client.ExecuteSelect("select * from " + TableEntries);

            List<Entry> entries = new List<Entry>();

            foreach (DataRow r in results.Rows)
            {
                var entry = new Entry();
                entry.ID = GetValue<int>(r["Id"]);
                entry.Name = GetValue<string>(r["Name"]);
                entry.URL = GetValue<string>(r["Url"]);

                var pwd = r["PasswordId"];
                if (!SQLUtils.IsNull(pwd))
                    entry.CredentialId = Convert.ToInt32(pwd);

                entries.Add(entry);
            }

            return entries;
        }

        public List<PasswordItem> GetPasswords()
        {
            var results = client.ExecuteSelect("select * from " + TablePasswords);

            List<PasswordItem> passwords = new List<PasswordItem>();

            foreach (DataRow r in results.Rows)
            {
                var pwd = new PasswordItem();
                pwd.ID = GetValue<int>(r["Id"]);
                pwd.Name = GetValue<string>(r["Name"]);
                pwd.Username = GetValue<string>(r["Username"]);
                pwd.Password = GetValue<string>(r["Password"]);

                passwords.Add(pwd);
            }

            return passwords;
        }

        private const string cUpdateEntry = "update {0} set Name = '{2}', Url='{3}', PasswordId={4} where Id = {1}";
        private const string cUpdatePassword = "update {0} set Name = {2}, Username = '{3}', Password = '{4}', where Id = {1}";

        public void UpdateEntry(Entry i)
        {
            string cmd = string.Format(cUpdateEntry, TableEntries, i.ID,
                SQLUtils.SQLEncode(i.Name),
                SQLUtils.SQLEncode(i.URL),
                i.CredentialId
            );

            client.ExecuteNonQuery(cmd);
        }

        public void UpdatePassword(PasswordItem i)
        {
            string cmd = string.Format(cUpdatePassword, TablePasswords, i.ID,
                    SQLUtils.SQLEncode(i.Name),
                     SQLUtils.SQLEncode(i.Username),
                     SQLUtils.SQLEncode(i.Password)
                 );

            client.ExecuteNonQuery(cmd);
        }

        private T GetValue<T>(object v)
        {
            if (SQLUtils.IsNull(v))
                return default(T);

            Type t = typeof(T);

            return (T)Convert.ChangeType(v, t);
        }
    }
}
