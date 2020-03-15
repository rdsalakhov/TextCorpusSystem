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
using TextCorpusSystem;

namespace View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            //var mainWindow = new MainAppWindow();
            //mainWindow.Show();
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUpDialog = new SignUpWindow();

            if (signUpDialog.ShowDialog() == true)
            {
                try
                {
                    UserAccountManager.SignUpUser(signUpDialog.Login, signUpDialog.Password);
                }
                catch (UserAlreadyExistException)
                {
                    MessageBox.Show("Пользователь с таким адресом электронной почты уже зарегистрирован.", "Ошибка регистрации");
                }
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == "" || PasswordBox.Text == "")
                MessageBox.Show("Не все поля заполнены. Введите электронную почту и пароль", "Ошибка входа");
            else
            {
                try
                {
                    int userId = UserAccountManager.SignInUser(LoginBox.Text, PasswordBox.Text);
                    MessageBox.Show("Success!");
                    var mainWindow = new MainAppWindow(userId);
                    mainWindow.Show();
                    this.Close();
                }
                catch (InvalidPasswordException)
                {
                    MessageBox.Show("Неверный пароль!", "Ошибка входа");
                }
            }
        }
    }
}
