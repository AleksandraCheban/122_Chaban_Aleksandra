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

            // Добавляем проверку на пустой выбор
            if (paymentForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите платежи для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {paymentForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = Entities.GetContext();
                    int deletedCount = 0;

                    foreach (var payment in paymentForRemoving)
                    {
                        // Находим сущность в контексте по ID (используйте правильное название поля)
                        var paymentToDelete = context.Paymant.Find(payment.ID); // Замените PaymentId на правильное поле
                        if (paymentToDelete != null)
                        {
                            context.Paymant.Remove(paymentToDelete);
                            deletedCount++;
                        }
                    }

                    // Сохраняем изменения только если что-то удаляли
                    if (deletedCount > 0)
                    {
                        context.SaveChanges();
                        MessageBox.Show($"Данные успешно удалены! Удалено платежей: {deletedCount}");
                        DataGridPayment.ItemsSource = context.Paymant.ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}\n\nДетали: {ex.InnerException?.Message}");
                }
            }
        }



        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddPaymentPage((sender as Button).DataContext as Paymant));
        }
    }
}
