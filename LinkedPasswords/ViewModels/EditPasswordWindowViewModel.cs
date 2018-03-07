using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using LinkedPasswords.Dao;
using LinkedPasswords.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LinkedPasswords.ViewModels
{
    class EditPasswordWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly EditPasswordWindow window;
        private PasswordItem existing;
        private bool isEdit;
        private IDataStore ds;

        public EditPasswordWindowViewModel(EditPasswordWindow window, IDataStore ds, PasswordItem existing = null)
        {
            this.window = window;
            Cancelled = true;

            isEdit = existing != null;
            this.ds = ds;

            if (isEdit)
            {
                this.existing = existing;
                Name = existing.Name;
                Username = existing.Username;
                Password = existing.Password;
            }
        }

        public bool Cancelled { get; private set; }

        public string WindowTitle
        {
            get { return (!isEdit ? "Create" : "Edit") + " Password"; }
        }

        public string OKText
        {
            get { return (!isEdit ? "Create" : "Edit"); }
        }

        

        public string Name
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public string Username
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get { return window.PasswordBox.Password; }
            set
            {
                window.PasswordBox.Password = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CancelCommand
        {
            get { return new CommandHelper(() => window.Close()); }
        }

        public ICommand OKCommand
        {
            get { return new CommandHelper(Save); }
        }

        private void Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Name))
                {
                    throw new Exception("All fields are required.");
                }

                PasswordItem pwd = new PasswordItem();

                pwd.Username = Username;
                pwd.Password = Password;
                pwd.Name = Name;


                if (isEdit)
                {
                    pwd.ID = existing.ID;
                    ds.UpdatePassword(pwd);
                }
                else
                    ds.AddPassword(pwd);

                Cancelled = false;
                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}
