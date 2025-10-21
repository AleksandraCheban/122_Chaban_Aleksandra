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
    /// Логика взаимодействия для ChangePassPage.xaml
    /// </summary>
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void txtbxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLoginHint.Visibility = string.IsNullOrEmpty(txtbxLogin.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void passBxCurrent_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblCurrentPassHint.Visibility = string.IsNullOrEmpty(passBxCurrent.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void passBxNew_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblNewPassHint.Visibility = string.IsNullOrEmpty(passBxNew.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void passBxConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblConfirmPassHint.Visibility = string.IsNullOrEmpty(passBxConfirm.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void changePassButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения всех полей
            if (string.IsNullOrEmpty(txtbxLogin.Text) ||
                string.IsNullOrEmpty(passBxCurrent.Password) ||
                string.IsNullOrEmpty(passBxNew.Password) ||
                string.IsNullOrEmpty(passBxConfirm.Password))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }

            // Проверка на правильность введенных данных аккаунта
            string hashedCurrentPass = GetHash(passBxCurrent.Password);
            var user = Entities.GetContext().Users.FirstOrDefault(u => u.Login == txtbxLogin.Text && u.Password == hashedCurrentPass);

            if (user == null)
            {
                MessageBox.Show("Текущий пароль или логин неверный!");
                return;
            }

            // Проверка на совпадение нового пароля и его подтверждения
            if (passBxNew.Password != passBxConfirm.Password)
            {
                MessageBox.Show("Новый пароль и его подтверждение не совпадают!");
                return;
            }

            // Проверка длины нового пароля
            if (passBxNew.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return;
            }

            // Проверка на английскую раскладку и наличие цифр
            bool en = true;
            bool number = false;

            for (int i = 0; i < passBxNew.Password.Length; i++)
            {
                if (passBxNew.Password[i] >= '0' && passBxNew.Password[i] <= '9')
                    number = true;
                else if (!((passBxNew.Password[i] >= 'A' && passBxNew.Password[i] <= 'Z') ||
                           (passBxNew.Password[i] >= 'a' && passBxNew.Password[i] <= 'z')))
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

            // Сохранение нового пароля
            user.Password = GetHash(passBxNew.Password);
            Entities.GetContext().SaveChanges();
            MessageBox.Show("Пароль успешно изменен!");
            NavigationService?.Navigate(new AuthPage());
        }
    }
}