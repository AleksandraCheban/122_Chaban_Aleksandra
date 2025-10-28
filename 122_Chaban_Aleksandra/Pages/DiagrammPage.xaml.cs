using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using WinFormsCharting = System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;

namespace _122_Chaban_Aleksandra
{
    /// <summary>
    /// Логика взаимодействия для DiagrammPage.xaml
    /// </summary>
    public partial class DiagrammPage : Page
    {
        private Entities _context = new Entities();
        public DiagrammPage()
        {
            InitializeComponent();
            ChartPayments.ChartAreas.Add(new WinFormsCharting.ChartArea("Main"));
            var currentSeries = new WinFormsCharting.Series("Платежи")
            {
                IsValueShownAsLabel = true
            };
            ChartPayments.Series.Add(currentSeries);
            ComboUsers.ItemsSource = _context.Users.ToList(); //ФИО пользователей
            ComboChartTypes.ItemsSource = Enum.GetValues(typeof(WinFormsCharting.SeriesChartType));
        }


        private void BtnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allUsers = _context.Users.ToList().OrderBy(p => p.FIO).ToList();

                var application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Add();

                // Создаем один лист для всех пользователей
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                worksheet.Name = "Все платежи";

                int rowIndex = 1;

                // Заголовки
                worksheet.Cells[rowIndex, 1] = "Пользователь";
                worksheet.Cells[rowIndex, 2] = "Дата платежа";
                worksheet.Cells[rowIndex, 3] = "Категория";
                worksheet.Cells[rowIndex, 4] = "Название";
                worksheet.Cells[rowIndex, 5] = "Стоимость";
                worksheet.Cells[rowIndex, 6] = "Количество";
                worksheet.Cells[rowIndex, 7] = "Сумма";

                rowIndex++;

                // Данные
                foreach (var user in allUsers)
                {
                    foreach (var payment in user.Paymant.OrderBy(p => p.Date))
                    {
                        worksheet.Cells[rowIndex, 1] = user.FIO;
                        worksheet.Cells[rowIndex, 2] = payment.Date.ToString("dd.MM.yyyy HH:mm");
                        worksheet.Cells[rowIndex, 3] = payment.Category?.Name;
                        worksheet.Cells[rowIndex, 4] = payment.Name;
                        worksheet.Cells[rowIndex, 5] = payment.Price;
                        worksheet.Cells[rowIndex, 6] = payment.Num;
                        worksheet.Cells[rowIndex, 7].Formula = $"=E{rowIndex}*F{rowIndex}";

                        rowIndex++;
                    }
                }

                worksheet.Columns.AutoFit();
                application.Visible = true;
                MessageBox.Show("Данные успешно экспортированы в Excel!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (ComboUsers.SelectedItem is Users currentUser && ComboChartTypes.SelectedItem is WinFormsCharting.SeriesChartType currentType)
            {
                WinFormsCharting.Series currentSeries = ChartPayments.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();
                var categoriesList = _context.Category.ToList();
                foreach (var category in categoriesList)
                {
                    currentSeries.Points.AddXY(category.Name,
                    _context.Paymant.ToList().Where(u => u.Users == currentUser && u.Category == category).Sum(u => u.Price * u.Num));
                }
            }
        }




        private void BtnExportToWord_Click(object sender, RoutedEventArgs e)
        {
            var allUsers = _context.Users.ToList();
            var allCategories = _context.Category.ToList();

            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            foreach (var user in allUsers)
            {
                Word.Paragraph userParagraph = document.Paragraphs.Add();
                Word.Range userRange = userParagraph.Range;
                userRange.Text = user.FIO;
                userParagraph.set_Style("Заголовок");
                userRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                userRange.InsertParagraphAfter();
                document.Paragraphs.Add(); //Пустая строка 

                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;
                Word.Table paymentsTable = document.Tables.Add(tableRange, allCategories.Count() + 1, 2);
                paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle =
                 Word.WdLineStyle.wdLineStyleSingle;
                paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                Word.Range cellRange;
                cellRange = paymentsTable.Cell(1, 1).Range;
                cellRange.Text = "Категория";
                cellRange = paymentsTable.Cell(1, 2).Range;
                cellRange.Text = "Сумма расходов";
                paymentsTable.Rows[1].Range.Font.Name = "Times New Roman"; paymentsTable.Rows[1].Range.Font.Size = 14;
                paymentsTable.Rows[1].Range.Bold = 1;
                paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 0; i < allCategories.Count(); i++)
                {
                    var currentCategory = allCategories[i];
                    cellRange = paymentsTable.Cell(i + 2, 1).Range;
                    cellRange.Text = currentCategory.Name;
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;
                    cellRange = paymentsTable.Cell(i + 2, 2).Range;
                    cellRange.Text = user.Paymant.ToList().
                   Where(u => u.Category == currentCategory).Sum(u => u.Num * u.Price).ToString("N2") + " руб.";
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;
                } //завершение цикла по строкам таблицы 
                document.Paragraphs.Add(); //пустая строка 

                Paymant maxPayment = user.Paymant.OrderByDescending(u => u.Price * u.Num).FirstOrDefault();
                if (maxPayment != null)
                {
                    Word.Paragraph maxPaymentParagraph = document.Paragraphs.Add(); Word.Range maxPaymentRange = maxPaymentParagraph.Range; maxPaymentRange.Text = $"Самый дорогостоящий платеж -  { maxPayment.Name} за { (maxPayment.Price * maxPayment.Num).ToString("N2")} " + $"руб.от { maxPayment.Date.ToString("dd.MM.yyyy")}  "; 
                    maxPaymentParagraph.set_Style("Подзаголовок");
                    maxPaymentRange.Font.Color = Word.WdColor.wdColorDarkRed;
                    maxPaymentRange.InsertParagraphAfter();
                }
                document.Paragraphs.Add(); //пустая строка 

                Paymant minPayment = user.Paymant.OrderBy(u => u.Price * u.Num).FirstOrDefault();
                if (maxPayment != null)
                {
                    Word.Paragraph minPaymentParagraph = document.Paragraphs.Add(); Word.Range minPaymentRange = minPaymentParagraph.Range; minPaymentRange.Text = $"Самый дешевый платеж - {minPayment.Name}   за { (minPayment.Price * minPayment.Num).ToString("N2")} " + $"руб.от  { minPayment.Date.ToString("dd.MM.yyyy")} "; 
                minPaymentParagraph.set_Style("Подзаголовок");
                    minPaymentRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                    minPaymentRange.InsertParagraphAfter();
                }
                if (user != allUsers.LastOrDefault()) document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);

                

            }
            application.Visible = true;


        }
        }
}