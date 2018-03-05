using System;
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
using System.Windows.Shapes;
using LinkedPasswords.Dao;
using LinkedPasswords.ViewModels;

namespace LinkedPasswords
{
    /// <summary>
    /// Interaction logic for OpenDatabaseWindow.xaml
    /// </summary>
    public partial class OpenDatabaseWindow : Window
    {
        private readonly OpenDatabaseWindowViewModel vm;

        public OpenDatabaseWindow()
        {
            InitializeComponent();
            this.vm = new OpenDatabaseWindowViewModel(this, PasswordInput);
            DataContext = vm;
        }

        public bool Cancelled { get { return vm.Cancelled; } }

        public IDataStore GetDataStore()
        {
            return vm.DataStore;
        }
    }
}
