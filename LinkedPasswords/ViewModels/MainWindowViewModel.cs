using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using LinkedPasswords.Dao;
using LinkedPasswords.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace LinkedPasswords.ViewModels
{
    class MainWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Properties.Settings settings = Properties.Settings.Default;
        private readonly MainWindow mainWindow;

        private IDataStore ds;

        private CollectionViewSource cvsPasswords;
        private CollectionViewSource cvsEntries;

        private readonly ObservableCollection<PasswordItem> passwords = new ObservableCollection<PasswordItem>();
        private readonly Dictionary<int, PasswordItem> passwordsMap = new Dictionary<int, PasswordItem>();

        private readonly ObservableCollection<Entry> entries = new ObservableCollection<Entry>();
        
        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            cvsPasswords = new CollectionViewSource();
            cvsPasswords.Source = passwords;
            cvsPasswords.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            
            cvsEntries = new CollectionViewSource();
            cvsEntries.Source = entries;
            cvsEntries.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        }

        public ICollectionView Passwords
        {
            get { return cvsPasswords.View; }
        }

        public ICollectionView Logins
        {
            get { return cvsEntries.View; }
        }
        
        public void OpenDatabase()
        {
            try
            {
                var dlg = new OpenDatabaseWindow();
                dlg.Owner = mainWindow;
                dlg.ShowDialog();

                if (dlg.Cancelled)
                    return;

                var newDs = dlg.GetDataStore();
                if (newDs != null)
                {
                    if (ds != null)
                      ds.Close();

                    ds = newDs;
                    LoadLists();

                    TryAdd();


                    StatusMessage = "Opened: " + dlg.Path;
                }

            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand OpenDatabaseCommand
        {
            get { return new CommandHelper(OpenDatabase); }
        }

        private void TryAdd()
        {
            if (passwords.Count == 0)
            {
                var pwd = new PasswordItem() { Name = "test", Username = "test", Password = "test" };
                ds.AddPassword(pwd);
                ds.AddEntry(new Entry() { Name = "test", PasswordId = pwd.ID });

                LoadLists();
            }
        }
        
        private void LoadLists()
        {
            passwords.Clear();
            entries.Clear();

            passwordsMap.Clear();

            foreach (var pwd in ds.GetPasswords())
            {
                passwords.Add(pwd);
                passwordsMap[pwd.ID] = pwd;
            }

            foreach (var entry in ds.GetEntries())
            {
                entries.Add(entry);
            }
        }

        public string StatusMessage
        {
            get { return Get<string>(); }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }
        
        public void Close()
        {
            try
            {
                if (ds != null)
                    ds.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}
