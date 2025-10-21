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


namespace _122_Chaban_Aleksandra
{
    /// <summary>
    /// Логика взаимодействия для AddPaymentPage.xaml
    /// </summary>
    public partial class AddPaymentPage : Page
    {
        private Paymant _currentPayment = new Paymant();
        public AddPaymentPage(Paymant selectedPayment)
        {
            InitializeComponent();
            CBCategory.ItemsSource = Entities.GetContext().Category.ToList(); CBCategory.DisplayMemberPath = "Name";
            CBUser.ItemsSource = Entities.GetContext().Users.ToList();
            CBUser.DisplayMemberPath = "FIO";
            if (selectedPayment != null)
                _currentPayment = selectedPayment;
            DataContext = _currentPayment;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentPayment.Date.ToString())) errors.AppendLine("Укажите дату!");
            if (string.IsNullOrWhiteSpace(_currentPayment.Num.ToString())) errors.AppendLine("Укажите количество!");
            if (string.IsNullOrWhiteSpace(_currentPayment.Price.ToString()))
                errors.AppendLine("Укажите цену");
            if (string.IsNullOrWhiteSpace(_currentPayment.UserID.ToString())) errors.AppendLine("Укажите клиента!");
            if
           (string.IsNullOrWhiteSpace(_currentPayment.CategoryID.ToString())) errors.AppendLine("Укажите категорию!"); if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentPayment.ID == 0)
                Entities.GetContext().Paymant.Add(_currentPayment);
            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBPaymentName.Text = "";
            TBAmount.Text = "";
            TBCount.Text = "";
            TBDate.Text = "";

            CBCategory.SelectedIndex = -1;
            CBUser.SelectedIndex = -1;

            _currentPayment = new Paymant();
            DataContext = _currentPayment;
        }
    }
}
