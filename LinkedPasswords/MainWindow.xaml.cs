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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            Dao.SQLiteDataStore client = new Dao.SQLiteDataStore("test.db", "test");
            client.Open();

            var pwd = new Models.PasswordItem() { Name = "test", Username = "test", Password = "test" };
            client.AddPassword(pwd);
            client.AddEntry(new Models.Entry() { Name = "test", PasswordId = pwd.ID });
            client.DeletePassword(pwd);
        }
    }
}
