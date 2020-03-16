using System.IO;
using System.Threading;
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
using TextCorpusSystem;
using System.Data;
using Microsoft.Win32;

namespace View
{
    /// <summary>
    /// Логика взаимодействия для MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        private int _currentUserId;

        public MainAppWindow()
        {
            InitializeComponent();
            BindComboBox(TextsNamesComboBox);
            this._currentUserId = 1;
        }

        public MainAppWindow(int userId)
        {
            InitializeComponent();
            BindComboBox(TextsNamesComboBox);
            this._currentUserId = userId;
        }

        private void OpenTextButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenTextButton_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void BindComboBox(ComboBox comboBox)
        {
            DataSet textsDS = TextManager.GetTextsDataSet();

            comboBox.ItemsSource = textsDS.Tables[0].DefaultView;
            comboBox.DisplayMemberPath = textsDS.Tables[0].Columns["textname"].ToString();
            comboBox.SelectedValuePath = textsDS.Tables[0].Columns["id"].ToString();
        }

        private void TextsNamesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)TextsNamesComboBox.SelectedItem;
            if (selectedRow != null)
            {
                var selectedId = (int)selectedRow.Row.ItemArray[1];
                TextHighlighter.GetHighlightedText(selectedId, HighlightedTextRichTB, HighligthedTagsRichTB);
                DeleteTextButton.IsEnabled = true;
                ExportTextButton.IsEnabled = true;
                UpdateAnnotationButton.IsEnabled = true;
                SearchMenuItem.IsEnabled = true;
            }
            else
            {
                DeleteTextButton.IsEnabled = false;
                ExportTextButton.IsEnabled = false;
                UpdateAnnotationButton.IsEnabled = false;
            }
        }

        private void ImportTextButton_Click(object sender, RoutedEventArgs e)
        {

            var t = new Thread(delegate () { UploadFile(); });
            t.Start();
        }

        private void UploadFile()
        {
            var openTextFD = new OpenFileDialog();
            openTextFD.Filter = "Text files (*.txt)|*.txt";
            openTextFD.Title = "Выберите файл текста";
            openTextFD.Multiselect = false;

            var openAnnotationFD = new OpenFileDialog();
            openAnnotationFD.Filter = "Annotation files (*.ann)|*.ann";
            openAnnotationFD.Title = "Выберите файл аннотации";
            openAnnotationFD.Multiselect = false;
            if (openTextFD.ShowDialog() == true)
            {
                if (openAnnotationFD.ShowDialog() == true)
                {
                    var textSR = new StreamReader(openTextFD.FileName);
                    var annotationSR = new StreamReader(openAnnotationFD.FileName);
                    string textName = System.IO.Path.GetFileName(openTextFD.FileName);
                    try
                    {
                        TextManager.UploadText(textSR, annotationSR, textName);
                    }
                    catch (TextNameAlreadyExistException)
                    {
                        MessageBox.Show("Текст с таким именем уже существует!", "Ошибка загрузки");
                    } 
                    catch (InvalidAnnotationException)
                    {
                        MessageBox.Show("Некорректный формат аннотации!", "Ошибка загрузки");
                    }
                }
            }
        }

        private void TextsNamesComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
            BindComboBox(TextsNamesComboBox);
        }

        private void DeleteTextButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)TextsNamesComboBox.SelectedItem;
            if (selectedRow != null)
            {
                var selectedId = (int)selectedRow.Row.ItemArray[1];
                if (UserAccountManager.CheckAdminStatus(_currentUserId) || UserAccountManager.CheckUserAccess(_currentUserId, selectedId))
                {
                    TextManager.DeleteText(selectedId);
                    HighlightedTextRichTB.Document = new FlowDocument();
                    HighligthedTagsRichTB.Document = new FlowDocument();
                    BindComboBox(TextsNamesComboBox);
                }
                else
                {
                    MessageBox.Show("У вашего аккаунта нет доступа к изменению этого текста!", "Ошибка доступа");
                }
            }
        }

        private void UpdateAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)TextsNamesComboBox.SelectedItem;
            if (selectedRow != null)
            {
                var selectedId = (int)selectedRow.Row.ItemArray[1];
                if (UserAccountManager.CheckAdminStatus(_currentUserId) || UserAccountManager.CheckUserAccess(_currentUserId, selectedId))
                {
                    var openAnnotationFD = new OpenFileDialog();
                    openAnnotationFD.Filter = "Annotation files (*.ann)|*.ann";
                    openAnnotationFD.Title = "Выберите файл аннотации";
                    openAnnotationFD.Multiselect = false;
                    if (openAnnotationFD.ShowDialog() == true)
                    {
                        var annotationSR = new StreamReader(openAnnotationFD.FileName);
                        try
                        {
                            TextManager.UpdateTextAnnotation(selectedId, annotationSR);
                            TextHighlighter.GetHighlightedText(selectedId, HighlightedTextRichTB, HighligthedTagsRichTB);
                        }
                        catch (InvalidConstraintException)
                        {
                            MessageBox.Show("Некорректный файл аннотации!", "Ошибка загрузки");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("У вашего аккаунта нет доступа к изменению этого текста!", "Ошибка доступа");
                }
            }
        }

        private void ExportTextButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.DefaultExt = "*.rtf";
            saveFileDlg.Filter = "RTF Files|*.rtf";
            saveFileDlg.Title = "Экспорт файла";

            if (saveFileDlg.ShowDialog() == true)
            {
                var saveFS = new FileStream(saveFileDlg.FileName, FileMode.OpenOrCreate, FileAccess.Write);                
                var tr = new TextRange(HighlightedTextRichTB.Document.ContentStart, HighlightedTextRichTB.Document.ContentEnd);
                tr.Save(saveFS, DataFormats.Rtf);
                saveFS.Close();
            }
        }

        public void DisplayQueryResults(List<QueryResult> queryResults)
        {
            TextsNamesComboBox.SelectedItem = null;
            if (queryResults.Count != 0)
            {
                FlowDocument queryResultDoc = new FlowDocument();
                int resultIndex = 1;
                foreach (var result in queryResults)
                {
                    Run textNameRun = new Run($"{resultIndex++}. " + result.TextName);
                    TextRange tr = new TextRange(textNameRun.ContentStart, textNameRun.ContentEnd);
                    tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    Paragraph textNameParagraph = new Paragraph(textNameRun);
                    queryResultDoc.Blocks.Add(textNameParagraph);

                    Run displayTextRun = new Run(result.Text);
                    Paragraph displayTextParagraph = new Paragraph();
                    FlowDocument bodyDoc = new FlowDocument(new Paragraph(displayTextRun));
                    for (int i = 0; i < result.StartPositions.Count; i++)
                    {
                        bodyDoc = TextHighlighter.HighlighAtRange(bodyDoc, result.StartPositions[i], result.EndPositions[i]);
                    }

                    queryResultDoc.Blocks.Add(bodyDoc.Blocks.FirstBlock);
                }
                HighlightedTextRichTB.Document = queryResultDoc;
                HighligthedTagsRichTB.Document = new FlowDocument();
            }
            else
            {
                HighlightedTextRichTB.Document = new FlowDocument(new Paragraph(new Run("Совпадений не найдено")));
                HighligthedTagsRichTB.Document = new FlowDocument();
            }
        }

        private void CreateQueryButton_Click(object sender, RoutedEventArgs e)
        {
            CreateQueryWindow queryWindow = new CreateQueryWindow(this);
            queryWindow.Show();
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            UserAccountManager.DeleteAccount(_currentUserId);
            var newSignInWindow = new SignInWindow();
            newSignInWindow.Show();
            this.Close();
        }

        private void ManageAccessButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserAccountManager.CheckAdminStatus(_currentUserId))
            {
                AccessManagerWindow accessManagerWindow = new AccessManagerWindow();
                accessManagerWindow.Show();
            }
            else
            {
                MessageBox.Show("Доступно только для пользователей со статусом администратора!", "Ошибка");
            }
        }
    }
}
