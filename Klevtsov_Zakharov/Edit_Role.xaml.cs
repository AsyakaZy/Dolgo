using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Klevtsov_Zakharov
{
    /// <summary>
    /// Логика взаимодействия для Edit_Role.xaml
    /// </summary>
    public partial class Edit_Role : Window
    {
        private Users user;
        private Roles userRole;
        private Offices userOffice;
        //private AMONIC_Airlines_Automation_System mainSystem; // Поле mainSystem удалено

        public Edit_Role(Users user)
        {
            InitializeComponent();
            this.user = user;
            var a = User05Entities5.GetContext().Roles.FirstOrDefault(x => x.Users.Any(y => y.Id == user.Id)).Id;
            var context = User05Entities5.GetContext();
            // Получаем роль и офис пользователя
            GetUserRole(context);
            GetUserOffice(context);

            if (user != null)
            {
                textBoxEmailAddress.Text = user.Email;
                textBoxFirstName.Text = user.FirstName;
                textBoxLastName.Text = user.LastName;

                if (a == 1)
                {
                    radioButtonAdministrator.IsChecked = true;
                }
                else
                {
                    radioButtonManager.IsChecked = true;
                }

                // Заполняем комбо бокс с офисами
                if (userOffice != null)
                {
                    comboBoxOffice.ItemsSource = new Offices[] { userOffice };
                    comboBoxOffice.DisplayMemberPath = "title";
                    comboBoxOffice.SelectedIndex = 0;
                }
            }
        }

        private void GetUserRole(User05Entities5 context)
        {
            userRole = context.Roles.FirstOrDefault(r => r.Users.Any(u => u.Id == user.Id));
        }

        // Метод для получения офиса пользователя
        private void GetUserOffice(User05Entities5 context)
        {
            userOffice = context.Offices.FirstOrDefault(o => o.Users.Any(u => u.Id == user.Id));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AMONIC_Airlines_Automation_System aMONIC_Airlines_Automation_System = new AMONIC_Airlines_Automation_System();
            aMONIC_Airlines_Automation_System.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную роль
            string selectedRole = radioButtonAdministrator.IsChecked == true ? "Admin" : "Manager";

            // Получаем выбранный офис
            Offices selectedOffice = comboBoxOffice.SelectedItem as Offices;

            if (user != null && selectedOffice != null)
            {
                var userToUpdate = User05Entities5.GetContext().Users.FirstOrDefault(u => u.Id == user.Id);
                if (userToUpdate != null)
                {
                    userToUpdate.Roles.Clear();
                    userToUpdate.Roles.Add(User05Entities5.GetContext().Roles.FirstOrDefault(r => r.Title == selectedRole));

                    userToUpdate.Offices.Clear();
                    userToUpdate.Offices.Add(selectedOffice);

                    User05Entities5.GetContext().SaveChanges();
                    AMONIC_Airlines_Automation_System aMONIC = new AMONIC_Airlines_Automation_System();
                    aMONIC.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя и офис.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
