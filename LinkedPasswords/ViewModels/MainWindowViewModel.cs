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
        private CollectionViewSource cvsLogins;

        private readonly ObservableCollection<PasswordItem> passwords = new ObservableCollection<PasswordItem>();
        private readonly Dictionary<int, PasswordItem> passwordsMap = new Dictionary<int, PasswordItem>();

        private readonly ObservableCollection<Entry> logins = new ObservableCollection<Entry>();
        
        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            cvsPasswords = new CollectionViewSource();
            cvsPasswords.Source = passwords;
            cvsPasswords.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            
            cvsLogins = new CollectionViewSource();
            cvsLogins.Source = logins;
            cvsLogins.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        }

        public bool DatabaseLoaded
        {
            get { return ds != null; }
            private set {
                RaisePropertyChanged();
            }
        }

        public Entry SelectedLogin
        {
            get { return Get<Entry>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public PasswordItem SelectedPassword
        {
            get { return Get<PasswordItem>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }
        

        public ICollectionView Passwords
        {
            get { return cvsPasswords.View; }
        }

        public ICollectionView Logins
        {
            get { return cvsLogins.View; }
        }

        public bool HasPasswords
        {
            get { return Get<bool>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
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

                DatabaseLoaded = true;

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
            logins.Clear();

            passwordsMap.Clear();

            foreach (var pwd in ds.GetPasswords())
            {
                passwords.Add(pwd);
                passwordsMap[pwd.ID] = pwd;
            }

            foreach (var entry in ds.GetEntries())
            {
                logins.Add(entry);
            }

            HasPasswords = passwords.Count > 0;
        }

        public void Close()
        {
            try
            {
                if (ds != null)
                    ds.Close();
                
                DatabaseLoaded = false;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        #region Password Commands
        public ICommand DeletePasswordCommand
        {
            get { return new CommandHelper(DeletePassword); }
        }

        private void DeletePassword()
        {
            try
            {
                if (SelectedPassword == null)
                    return;

                ds.DeletePassword(SelectedPassword);
                LoadLists();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        #endregion

        #region Login Commands
        #endregion
    }
}
