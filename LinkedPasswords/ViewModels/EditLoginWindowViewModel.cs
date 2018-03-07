using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using LinkedPasswords.Dao;
using LinkedPasswords.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace LinkedPasswords.ViewModels
{
    class EditLoginWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Window window;
        private Entry existing;
        private bool isEdit;
        private IDataStore ds;
        private Dictionary<int, PasswordItem> passwordsMap;
        private CollectionViewSource cvsPasswords;

        public EditLoginWindowViewModel(Window window, IDataStore ds, Dictionary<int, PasswordItem> passwordsMap, Entry existing = null)
        {
            this.window = window;
            Cancelled = true;

            isEdit = existing != null;
            this.ds = ds;
            this.passwordsMap = passwordsMap;

            cvsPasswords = new CollectionViewSource();
            cvsPasswords.Source = passwordsMap.Values;
            cvsPasswords.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

            if (isEdit)
            {
                this.existing = existing;
                Name = existing.Name;
                URL = existing.URL;

                if (existing.PasswordId != null)
                    SelectedPassword = passwordsMap[existing.PasswordId.Value];
            }
        }

        public ICollectionView Passwords
        {
            get { return cvsPasswords.View; }
        }

        public bool Cancelled { get; private set; }

        public string WindowTitle
        {
            get { return (!isEdit ? "Create" : "Edit") + " Login"; }
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

        public string URL
        {
            get { return Get<string>(); }
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
                if (string.IsNullOrEmpty(Name))
                {
                    throw new Exception("Name is required.");
                }
                else if (SelectedPassword == null)
                {
                    throw new Exception("A password must be selected.");
                }

                Entry entry = new Entry();

                entry.URL = URL;
                entry.Name = Name;

                if (SelectedPassword == null)
                    entry.PasswordId = null;
                else
                    entry.PasswordId = SelectedPassword.ID;


                if (isEdit)
                {
                    entry.ID = existing.ID;
                    ds.UpdateEntry(entry);
                }
                else
                    ds.AddEntry(entry);

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
