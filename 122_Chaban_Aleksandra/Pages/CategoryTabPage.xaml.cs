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
    /// Логика взаимодействия для CategoryTabPage.xaml
    /// </summary>
    public partial class CategoryTabPage : Page
    {
        public CategoryTabPage()
        {
            InitializeComponent();
            DataGridCategory.ItemsSource =
           Entities.GetContext().Category.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }
        private void Page_IsVisibleChanged(object sender,
       DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridCategory.ItemsSource =
               Entities.GetContext().Category.ToList();
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddCategoryPage(null));
        }
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var categoryForRemoving = DataGridCategory.SelectedItems.Cast<Category>().ToList();

            if (categoryForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите категории для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {categoryForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = Entities.GetContext();

                    foreach (var category in categoryForRemoving)
                    {
                        context.Category.Remove(category);
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    DataGridCategory.ItemsSource = context.Category.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCategoryPage((sender as Button).DataContext as Category));
        }
    }
}
