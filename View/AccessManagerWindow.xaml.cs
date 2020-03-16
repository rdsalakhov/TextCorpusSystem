using System;
using System.Collections.Generic;
using System.Data;
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
using TextCorpusSystem;

namespace View
{
    /// <summary>
    /// Логика взаимодействия для AccessManagerWindow.xaml
    /// </summary>
    public partial class AccessManagerWindow : Window
    {
        public AccessManagerWindow()
        {
            InitializeComponent();
        }

        private void BindNonAdminsComboBox(ComboBox comboBox)
        {
            DataSet nonAdminsDS = UserAccountManager.GetNonAdminsDataSet();

            comboBox.ItemsSource = nonAdminsDS.Tables[0].DefaultView;
            comboBox.DisplayMemberPath = nonAdminsDS.Tables[0].Columns["login"].ToString();
            comboBox.SelectedValuePath = nonAdminsDS.Tables[0].Columns["id"].ToString();
        }

        private void BindTextsComboBox(ComboBox comboBox)
        {
            DataSet textsDS = TextManager.GetTextsDataSet();

            comboBox.ItemsSource = textsDS.Tables[0].DefaultView;
            comboBox.DisplayMemberPath = textsDS.Tables[0].Columns["textname"].ToString();
            comboBox.SelectedValuePath = textsDS.Tables[0].Columns["id"].ToString();
        }

        private void NonAdminTextComboBox_DropDownOpened(object sender, EventArgs e)
        {
            BindNonAdminsComboBox(NonAdminTextComboBox);
        }

        private void TextsComboBox_DropDownOpened(object sender, EventArgs e)
        {
            BindTextsComboBox(TextsComboBox);
        }

        private void NonAdminsComboBox_DropDownOpened(object sender, EventArgs e)
        {
            BindNonAdminsComboBox(NonAdminsComboBox);
        }

        private void GrantTextAccess_Click(object sender, RoutedEventArgs e)
        {
            
            if (NonAdminTextComboBox.SelectedValue == null || TextsComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите пользователя и текст!", "Ошибка");
            }
            else
            {
                int selectedUserId = (int)NonAdminTextComboBox.SelectedValue;
                int selectedTextId = (int)TextsComboBox.SelectedValue;

                if (!UserAccountManager.GrantAccessToText(selectedUserId, selectedTextId))
                {
                    MessageBox.Show("У данного пользователя уже есть доступ к этому тексту!", "Ошибка");
                }
                else
                {
                    MessageBox.Show("Доступ предоставлен!", "Успешно");
                }
            }
        }

        private void GranrAdminStatus_Click(object sender, RoutedEventArgs e)
        {
            if (NonAdminsComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите пользователя!", "Ошибка");
            }
            else
            {
                int selectedUserId = (int)NonAdminsComboBox.SelectedValue;
                UserAccountManager.GrantAdminStatus(selectedUserId);
                MessageBox.Show("Статус администратора предоставлен!", "Успешно");
            }
        }

        private void RemoveTextAccessButton_Click(object sender, RoutedEventArgs e)
        {
            if (NonAdminTextComboBox.SelectedValue == null || TextsComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите пользователя и текст!", "Ошибка");
            }
            else
            {
                int selectedUserId = (int)NonAdminTextComboBox.SelectedValue;
                int selectedTextId = (int)TextsComboBox.SelectedValue;

                if (!UserAccountManager.RemoveAccessToText(selectedUserId, selectedTextId))
                {
                    MessageBox.Show("Данный пользователь не имеет доступа к этому тексту!", "Ошибка");
                }
                else
                {
                    MessageBox.Show("Доступ отменен!", "Успешно");
                }
            }
        }
    }

    
}
