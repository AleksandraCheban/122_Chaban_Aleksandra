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
    /// Логика взаимодействия для UsersTabPage.xaml
    /// </summary>
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();

            DataGridUser.ItemsSource = Entities.GetContext().Users.ToList(); this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender,
DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridUser.ItemsSource = Entities.GetContext().Users.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddUserPage(null));
        }
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DataGridUser.SelectedItems.Cast<Users>().ToList();

            // Добавляем проверку на пустой выбор
            if (usersForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите пользователей для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {usersForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = Entities.GetContext();
                    int deletedCount = 0;

                    foreach (var user in usersForRemoving)
                    {
                        // Находим сущность в контексте по ID (используйте правильное название поля)
                        var userToDelete = context.Users.Find(user.ID); // Замените UserId на правильное поле
                        if (userToDelete != null)
                        {
                            context.Users.Remove(userToDelete);
                            deletedCount++;
                        }
                    }

                    // Сохраняем изменения только если что-то удаляли
                    if (deletedCount > 0)
                    {
                        context.SaveChanges();
                        MessageBox.Show($"Данные успешно удалены! Удалено пользователей: {deletedCount}");
                        DataGridUser.ItemsSource = context.Users.ToList();
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
            NavigationService.Navigate(new AddUserPage((sender as Button).DataContext as Users));
        }
    }
} 

    
