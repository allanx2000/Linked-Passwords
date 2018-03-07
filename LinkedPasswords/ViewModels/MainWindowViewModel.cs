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

                    //TryAdd();


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
            if (logins.Count == 0 && passwords.Count == 0)
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

            StringBuilder sb = new StringBuilder();

            foreach (var entry in ds.GetEntries())
            {
                sb.Clear();

                if (!string.IsNullOrEmpty(entry.URL))
                    sb.AppendLine("URL: " + entry.URL);

                if (entry.PasswordId != null)
                    sb.AppendLine("Using: " + passwordsMap[entry.PasswordId.Value].Name);
                
                entry.Description = sb.ToString();

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
        public ICommand AddPasswordCommand
        {
            get { return new CommandHelper(AddPassword); }
        }

        private void AddPassword()
        {
            try
            {
                var dlg = new EditPasswordWindow(ds);
                dlg.Owner = mainWindow;
                dlg.ShowDialog();

                if (!dlg.Cancelled)
                    LoadLists();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand EditPasswordCommand
        {
            get { return new CommandHelper(EditPassword); }
        }

        private void EditPassword()
        {
            try
            {
                if (SelectedPassword == null)
                    return;

                try
                {
                    var dlg = new EditPasswordWindow(ds, SelectedPassword);
                    dlg.Owner = mainWindow;
                    dlg.ShowDialog();

                    if (!dlg.Cancelled)
                        LoadLists();
                }
                catch (Exception e)
                {
                    MessageBoxFactory.ShowError(e);
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

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

                //TODO: Should make PasswordOptional and if doesn't have, mark red 
                if (MessageBoxFactory.ShowConfirmAsBool("Delete " + SelectedPassword.Name + "? All logins will be deleted.",
                    "Delete Password", System.Windows.MessageBoxImage.Exclamation))
                {
                    ds.DeletePassword(SelectedPassword);
                    LoadLists();
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        #endregion

        #region Login Commands
        public ICommand AddLoginCommand
        {
            get { return new CommandHelper(AddLogin); }
        }

        private void AddLogin()
        {
            try
            {
                var dlg = new EditLoginWindow(ds, passwordsMap);
                dlg.Owner = mainWindow;
                dlg.ShowDialog();

                if (!dlg.Cancelled)
                    LoadLists();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand EditLoginCommand
        {
            get { return new CommandHelper(EditLogin); }
        }

        private void EditLogin()
        {
            try
            {
                if (SelectedLogin == null)
                    return;

                try
                {
                    var dlg = new EditLoginWindow(ds, passwordsMap, SelectedLogin);
                    dlg.Owner = mainWindow;
                    dlg.ShowDialog();

                    if (!dlg.Cancelled)
                        LoadLists();
                }
                catch (Exception e)
                {
                    MessageBoxFactory.ShowError(e);
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }


        public ICommand DeleteLoginCommand
        {
            get { return new CommandHelper(DeleteLogin); }
        }

        private void DeleteLogin()
        {
            try
            {
                if (SelectedLogin == null)
                    return;
                
                if (MessageBoxFactory.ShowConfirmAsBool("Delete " + SelectedLogin.Name + "?",
                    "Delete Login", System.Windows.MessageBoxImage.Exclamation))
                {
                    ds.DeleteEntry(SelectedLogin);
                    LoadLists();
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
        #endregion
    }
}
