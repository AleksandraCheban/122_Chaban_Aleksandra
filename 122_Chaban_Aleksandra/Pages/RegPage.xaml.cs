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
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {

        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;
        }
        public static string GetHash(String password)
        {
            using (var hash = SHA1.Create())
            {
                return
               string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLogHitn.Visibility = string.IsNullOrEmpty(txtbxLog.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblFioHitn.Visibility = string.IsNullOrEmpty(txtbxFIO.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassHitn.Visibility = string.IsNullOrEmpty(passBxFrst.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassSecHitn.Visibility = string.IsNullOrEmpty(passBxScnd.Password) ? Visibility.Visible : Visibility.Hidden;
        }


        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }


        private void lblLogHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxLog.Focus();
        }

        private void lblPassHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxFrst.Focus();
        }

        private void lblPassSecHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxScnd.Focus();
        }

        private void lblFioHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxFIO.Focus();
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) ||
                string.IsNullOrEmpty(txtbxFIO.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) ||
                string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (passBxFrst.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return;
            }

            bool en = true;
            bool number = false;

            for (int i = 0; i < passBxFrst.Password.Length; i++)
            {
                if (passBxFrst.Password[i] >= '0' && passBxFrst.Password[i] <= '9')
                    number = true;
                else if (!((passBxFrst.Password[i] >= 'A' && passBxFrst.Password[i] <= 'Z') ||
                           (passBxFrst.Password[i] >= 'a' && passBxFrst.Password[i] <= 'z')))
                    en = false;
            }

            if (!en)
            {
                MessageBox.Show("Используйте только английскую раскладку!");
                return;
            }

            if (!number)
            {
                MessageBox.Show("Добавьте хотя бы одну цифру!");
                return;
            }

            using (Entities db = new Entities())
            {
                var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Login == txtbxLog.Text);
                if (user != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                Users userObject = new Users
                {
                    FIO = txtbxFIO.Text,
                    Login = txtbxLog.Text,
                    Password = GetHash(passBxFrst.Password),
                    Role = comboBxRole.Text
                };

                db.Users.Add(userObject);
                db.SaveChanges();
                MessageBox.Show("Пользователь успешно зарегистрирован!");
            }

            txtbxLog.Clear();
            passBxFrst.Clear();
            passBxScnd.Clear();
            txtbxFIO.Clear();
            comboBxRole.SelectedIndex = 0;
        }


    }


}
