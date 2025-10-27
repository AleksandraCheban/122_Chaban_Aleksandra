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
    }
}