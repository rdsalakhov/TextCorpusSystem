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
    /// Логика взаимодействия для CreateQueryWindow.xaml
    /// </summary>
    public partial class CreateQueryWindow : Window
    {
        private MainAppWindow _parent;
        public CreateQueryWindow()
        {
            InitializeComponent();
        }

        public CreateQueryWindow(MainAppWindow parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ExactFormTextBox.Text != "")
            {
                _parent.DisplayQueryResults(QueryManager.FindExactForm(ExactFormTextBox.Text));
            }
            else
            {
                MessageBox.Show("Введена пустая строка поиска!", "Ошибка поиска");
            }
        }

        private void BindTagNameComboBox(ComboBox comboBox)
        {
            DataSet tagsDS = TextManager.GetLemmaTagsDataSet();

            comboBox.ItemsSource = tagsDS.Tables[0].DefaultView;
            comboBox.DisplayMemberPath = tagsDS.Tables[0].Columns["tagname"].ToString();
            comboBox.SelectedValuePath = tagsDS.Tables[0].Columns["tagname"].ToString();
        }

        private void FirstTagNameComboBox_DropDownOpened(object sender, EventArgs e)
        {
            BindTagNameComboBox(FirstTagNameComboBox);
        }

        private void SecondTagNameComboBox_DropDownOpened(object sender, EventArgs e)
        {
            BindTagNameComboBox(SecondTagNameComboBox);
        }

        private void LemmaSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckLemmaSearch())
            {
                _parent.DisplayQueryResults(QueryManager.FindLemmaPairAtRange(FirstLemmaTextBox.Text, SecondLemmaTextBox.Text,
                    (int)LemmaInRangeUpDown.Value, (int)LemmaOutRangeUpDown.Value));
            }
        }

        private void TagnameSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTagnameSearch())
            {
                string firstTag = (string)FirstTagNameComboBox.SelectedValue;
                string secondTag = (string)SecondTagNameComboBox.SelectedValue;
                _parent.DisplayQueryResults(QueryManager.FindTagPairAtRange(firstTag, secondTag,
                    (int)TagInRangeUpDown.Value, (int)TagOutRangeUpDown.Value));
            }
        }

        private bool CheckLemmaSearch()
        {
            if (FirstLemmaTextBox.Text == "" || SecondLemmaTextBox.Text == "")
            {
                MessageBox.Show("Введите обе леммы", "Ошибка поиска");
                return false;
            }
            if (LemmaInRangeUpDown.Value == null || LemmaOutRangeUpDown == null)
            {
                MessageBox.Show("Введите расстояние", "Ошибка поиска");
                return false;
            }
            return true;
        }

        private bool CheckTagnameSearch()
        {
            if (FirstTagNameComboBox.SelectedItem == null || SecondTagNameComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите оба грамматических признака", "Ошибка поиска");
                return false;
            }
            if (TagInRangeUpDown.Value == null || TagOutRangeUpDown.Value == null)
            {
                MessageBox.Show("Введите расстояние", "Ошибка поиска");
                return false;
            }
            return true;
        }
    }
}
