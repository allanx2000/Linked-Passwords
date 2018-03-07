using LinkedPasswords.Dao;
using LinkedPasswords.Models;
using LinkedPasswords.ViewModels;
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

namespace LinkedPasswords
{
    /// <summary>
    /// Interaction logic for EditPasswordWindow.xaml
    /// </summary>
    public partial class EditPasswordWindow : Window
    {
        private EditPasswordWindowViewModel vm;

        public EditPasswordWindow(IDataStore ds, PasswordItem existing = null)
        {
            InitializeComponent();

            vm = new EditPasswordWindowViewModel(this, ds, existing);
            DataContext = vm;
        }

        public bool Cancelled { get { return vm.Cancelled; } }
    }
}
