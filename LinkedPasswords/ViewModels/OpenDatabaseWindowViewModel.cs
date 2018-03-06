using System;
using LinkedPasswords.Dao;
using System.Windows.Controls;
using Innouvous.Utils.MVVM;
using System.Windows.Input;
using Innouvous.Utils;
using System.Collections;
using System.Linq;

namespace LinkedPasswords.ViewModels
{
    internal class OpenDatabaseWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly OpenDatabaseWindow openDatabaseWindow;
        private readonly PasswordBox pwdBox;

        private readonly Properties.Settings settings = Properties.Settings.Default;
        private SerializableList<string> pathHistory = new SerializableList<string>(new PathSerializer());

        private class PathSerializer : Serializer<string>
        {
            public string Deserialize(string obj)
            {
                return obj;
            }

            public string Serialize(string obj)
            {
                return obj.ToString();
            }
        }

        public OpenDatabaseWindowViewModel(OpenDatabaseWindow openDatabaseWindow, PasswordBox pwdBox)
        {
            this.openDatabaseWindow = openDatabaseWindow;
            this.pwdBox = pwdBox;
            Cancelled = true;

            pathHistory.LoadFromString(settings.History);

            Path = pathHistory.Items.FirstOrDefault();
        }

        public bool Cancelled { get; private set; }

        public IEnumerable PathHistory
        {
            get { return pathHistory.Items; }
        }

        public string Path
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public IDataStore DataStore
        {
            get { return Get<IDataStore>(); }
            private set { Set(value); }
        }

        public ICommand SelectDatabaseCommand
        {
            get
            {
                return new CommandHelper(SelectDatabase);
            }
        }

        private void SelectDatabase()
        {
            try
            {
                var dlg = DialogsUtility.CreateOpenFileDialog("Select/Create Database", checkFileExists: false);
                
                dlg.ShowDialog(openDatabaseWindow);
                
                if (!string.IsNullOrEmpty(dlg.FileName))
                    Path = dlg.FileName;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                return new CommandHelper(OpenDataStore);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new CommandHelper(() => openDatabaseWindow.Close());
            }
        }

        public SerializableList<string> PathHistory1 { get => pathHistory; set => pathHistory = value; }

        private void OpenDataStore()
        {
            try
            {
                if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(pwdBox.Password))
                {
                    throw new Exception("A path and password are required.");
                }

                IDataStore ds = new SQLiteDataStore(Path, pwdBox.Password);
                ds.Open();

                DataStore = ds;

                pathHistory.AddItem(Path);
                settings.History = pathHistory.SerializeToString();
                settings.Save();

                Cancelled = false;
                openDatabaseWindow.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}