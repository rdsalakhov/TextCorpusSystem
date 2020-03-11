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
using System.Text.RegularExpressions;

namespace View
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public string Login { get => EmailBox.Text; }

        public string Password { get => PasswordBox.Text; }

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            if (!IsValidEmail())
            {
                e.Handled = true;
                MessageBox.Show("Некорректный адрес электронной почты");
            }
            else if (!IsValidPassword())
            {
                e.Handled = true;
                MessageBox.Show("Пароль должен содержать минимум 8 символов");
            }
            else if (!IsRepeatedCorrectly())
            {
                e.Handled = true;
                MessageBox.Show("Пароли не совпадают");
            }
            else this.DialogResult = true;
        }

        private bool IsValidEmail()
        {
            string email = EmailBox.Text;

            string emailPattern = @"\A[^@]+@([^@\.]+\.)+[^@\.]+\z";
            if (Regex.IsMatch(email, emailPattern))
                return true;
            else return false;
        }

        private bool IsValidPassword()
        {
            if (PasswordBox.Text.Length >= 8)
                return true;
            else return false;
        }

        private bool IsRepeatedCorrectly()
        {
            if (PasswordBox.Text == RepeatPasswordBox.Text)
                return true;
            else return false;
        }
    }
}
