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

namespace Ivanov_NP_Cloud_Clients
{
    public delegate void RegistrationDelegate(string login, string password);

    public partial class Registration : Window
    {
        public event RegistrationDelegate PerformRegistrarion;

        public Registration()
        {
            InitializeComponent();
        }

        //cancel
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //ok
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != PasswordBox2.Password)
                MessageBox.Show("Password and confirmation password doesn't match.");
            else if (PasswordBox.Password.Length <= 7)
                MessageBox.Show("Password must be at least 8 characters.");
            else
            {
                PerformRegistrarion?.Invoke(NameTextBox.Text, PasswordBox.Password);
                this.Close();
            }
        }
    }
}
