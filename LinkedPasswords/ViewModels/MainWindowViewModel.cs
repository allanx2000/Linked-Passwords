using LinkedPasswords.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedPasswords.ViewModels
{
    class MainWindowViewModel
    {
        private readonly Properties.Settings settings = Properties.Settings.Default;
        private readonly MainWindow mainWindow;
        private readonly IDataStore ds;


        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void Open()
        {
            var dlg = new 
            ds = 
        }

        public void Close()
        {
            if (ds != null)
                ds.Close();
        }
    }
}
