using Klevtsov_Zakharov.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Klevtsov_Zakharov
{
    /// <summary> 
    /// Логика взаимодействия для AMONIC_Airlines_Automation_System.xaml 
    /// </summary> 
    public partial class AMONIC_Airlines_Automation_System : Window
    {
        private readonly User05Entities5 context;
        private string selectedRole;
        private Offices selectedOffice;
        private Users selectedUser;
        public AMONIC_Airlines_Automation_System()
        {
            InitializeComponent();
            officesort.ItemsSource = User05Entities5.GetContext().Offices.ToList();
            context = User05Entities5.GetContext();
            officesort.SelectedIndex = 0;

            var query = from user in User05Entities5.GetContext().Users.ToList()
                        let role = user.Roles.FirstOrDefault()
                        let office = user.Offices.FirstOrDefault()
                        select new
                        {
                            Id = user.Id, // Включаем идентификатор пользователя 
                            user.FirstName,
                            user.LastName,
                            user.Birthday,
                            RoleName = role != null ? role.Title : "No Role",
                            user.Email,
                            Office = office != null ? office.title : "No Office"
                        };
            AllUsers.ItemsSource = query.ToList();
            AllUsers.SelectedIndex = 0;

        }
        private void officesort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получение выбранного офиса 
            var selectedOffice = officesort.SelectedItem as Offices;

            if (selectedOffice != null)
            {
                // Фильтрация пользователей по выбранному офису 
                var context = User05Entities5.GetContext();
                // Фильтрация пользователей по выбранному офису 
                var usersInSelectedOffice = from user in context.Users
                                            where user.Offices.Any(office => office.Id == selectedOffice.Id)
                                            select new
                                            {
                                                user.Id,
                                                user.FirstName,
                                                user.LastName,
                                                user.Birthday,
                                                RoleName = user.Roles.FirstOrDefault() != null ? user.Roles.FirstOrDefault().Title : "No Role",
                                                user.Email,
                                                Office = selectedOffice.title
                                            };


                AllUsers.ItemsSource = usersInSelectedOffice.ToList();
            }
        }
        private void AllUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, есть ли выбранный пользователь 
            if
                (AllUsers.SelectedItem != null)
            {
                // Получаем выбранный объект пользователя 
                dynamic selectedData = AllUsers.SelectedItem;

                // Получаем ID выбранного пользователя 
                int selectedUserId = selectedData.Id;

                selectedUser = User05Entities5.GetContext().Users.FirstOrDefault(u => u.Id == selectedUserId);

            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Проверка, что выбран пользователь 
            if (selectedUser != null)
            {
                // Передача данных выбранного пользователя в новое окно Edit_Role 
                Edit_Role editRoleWindow = new Edit_Role(selectedUser);
                editRoleWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Please select a user to change the role.");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Add_User add = new Add_User(); // this - текущий экземпляр AMONIC_Airlines_Automation_System
            add.Show();
            this.Close();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            if (mw != null)
            {
                if (context != null) // Проверяем, что контекст был успешно инициализирован
                {
                    int currentUserId = SessionManager.CurrentUserId;

                    // Загрузка сессии пользователя с включением связанных сущностей (при необходимости)
                    var currentSession = context.UserssInfo
                                                .Where(s => s.Id == currentUserId && s.LogoutTime == null)
                                                .OrderByDescending(s => s.LoginTime)
                                                .FirstOrDefault();

                    if (currentSession != null)
                    {
                        // Получаем информацию о московском часовом поясе
                        TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

                        // Получаем текущее московское время
                        DateTime moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);

                        // Устанавливаем LogoutTime в текущее московское время
                        currentSession.LogoutTime = moscowTime;

                        // Устанавливаем состояние объекта как измененное
                        context.Entry(currentSession).State = EntityState.Modified;

                        // Сохраняем изменения
                        context.SaveChanges();
                    }
                }
                mw.Show();
                this.Close();
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            User_Window users_Window = new User_Window();
            users_Window.Show();
            this.Hide();
        }
    }
}