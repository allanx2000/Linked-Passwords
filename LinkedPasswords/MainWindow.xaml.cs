﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LinkedPasswords.Dao;
using LinkedPasswords.Models;

namespace LinkedPasswords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Test();
        }

        private void Test()
        {
            IDataStore client = new SQLiteDataStore("test.db", "test", false);
            client.Open();

            var pwd = new PasswordItem() { Name = "test", Username = "test", Password = "test" };

            (var passwords, var entries) = GetData(client);

            client.AddPassword(pwd);
            client.AddEntry(new Entry() { Name = "test", PasswordId = pwd.ID });
            
            (passwords, entries) = GetData(client);

            client.DeletePassword(pwd);

            (passwords, entries) = GetData(client);

        }

        private (List<PasswordItem>, List<Entry>) GetData(IDataStore client)
        {
            return (client.GetPasswords(), client.GetEntries());
        }
    }
}
