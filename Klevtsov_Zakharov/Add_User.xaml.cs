using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klevtsov_Zakharov
{
    public partial class Add_User : Window
    {

        public Add_User()
        {
            InitializeComponent();
            // Заполнение ComboBox данными из таблицы Office
            officeComboBox.ItemsSource = User05Entities5.GetContext().Offices.Select(o => o.title).ToList();
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string newText = (sender as TextBox).Text + e.Text;
            if (!IsDigitsOnly(newText))
            {
                e.Handled = true;
            }
        }

        private void save(object sender, RoutedEventArgs e)
        {
            string email = add_email.Text;
            string firstName = fname1.Text;
            string lastName = sname1.Text;
            string selectedOffice = officeComboBox.SelectedItem as string; // Получаем выбранный офис из ComboBox
            string password = pass.Password;

            if (Regex.IsMatch(email, @"[@]") && Regex.IsMatch(email, @"\b(mail.ru|gmail.com)\b") && firstName.Length > 0 && lastName.Length > 0 && selectedOffice != null)
            {
                // Найдем выбранный офис в базе данных
                Offices office = User05Entities5.GetContext().Offices.FirstOrDefault(o => o.title == selectedOffice);
                Roles role = User05Entities5.GetContext().Roles.FirstOrDefault(x => x.Id == 2);
                // Создаем нового пользователя
                Users newUser = new Users
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    // Добавляем офис к пользователю
                    Offices = { office },
                    Roles = { role },
                    Password = password 
                };

                // Сохраняем пользователя в базе данных
                User05Entities5.GetContext().Users.Add(newUser);
                User05Entities5.GetContext().SaveChanges();

                // Переход обратно на главное окно
                AMONIC_Airlines_Automation_System aMONIC_Airlines_Automation_System = new AMONIC_Airlines_Automation_System();
                aMONIC_Airlines_Automation_System.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Текст не соответствует условиям проверки.");
            }
        }

        private void cancel(object sender, RoutedEventArgs e)
        {
            AMONIC_Airlines_Automation_System aMONIC_Airlines_Automation_System = new AMONIC_Airlines_Automation_System();
            aMONIC_Airlines_Automation_System.Show();
            this.Close();
        }
    }
}

