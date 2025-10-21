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

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {usersForRemoving.Count()} элементов?",
                                "Внимание",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = Entities.GetContext())
                    {
                        foreach (var user in usersForRemoving)
                        {
                            context.Users.Attach(user);
                            context.Users.Remove(user);
                        }
                        context.SaveChanges();
                    }

                    MessageBox.Show("Данные успешно удалены!");
                    DataGridUser.ItemsSource = Entities.GetContext().Users.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUserPage((sender as Button).DataContext as Users));
        }
    }
} 

    
