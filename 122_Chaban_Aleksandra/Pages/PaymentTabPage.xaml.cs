using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для PaymentTabPage.xaml
    /// </summary>
    public partial class PaymentTabPage : Page
    {
        public PaymentTabPage()
        {
            InitializeComponent();
            DataGridPayment.ItemsSource =
           Entities.GetContext().Paymant.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }
        private void Page_IsVisibleChanged(object sender,
       DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridPayment.ItemsSource =
               Entities.GetContext().Paymant.ToList();
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddPaymentPage(null));
        }
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var paymentForRemoving = DataGridPayment.SelectedItems.Cast<Paymant>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {paymentForRemoving.Count()} элементов?",
                                "Внимание",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = Entities.GetContext())
                    {
                        foreach (var payment in paymentForRemoving)
                        {
                            context.Paymant.Attach(payment);
                            context.Paymant.Remove(payment);
                        }
                        context.SaveChanges();
                    }

                    MessageBox.Show("Данные успешно удалены!");
                    DataGridPayment.ItemsSource = Entities.GetContext().Paymant.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddPaymentPage((sender as Button).DataContext as Paymant));
        }
    }
}
