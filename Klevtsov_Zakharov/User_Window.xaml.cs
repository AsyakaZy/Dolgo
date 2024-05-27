using Klevtsov_Zakharov.Resources;
using System;
using System.Linq;
using System.Windows;

namespace Klevtsov_Zakharov
{
    public partial class User_Window : Window
    {
        private readonly User05Entities5 context;
        private Users currentUser;

        public User_Window()
        {
            InitializeComponent();
            context = User05Entities5.GetContext();
            LoadCurrentUser();  // Загружаем текущего пользователя  

            // Получаем последнее ненулевое значение TimeSpentOnSystem 
            TimeSpan? lastTimeSpent = GetLastNonNullTimeSpentOnSystem();

            // Если значение lastTimeSpent не равно null, присваиваем его содержимое тексту в Label  
            if (lastTimeSpent != null)
            {
                labelTimeSpent.Content = "Time spent on system: " + lastTimeSpent.Value.ToString(@"hh\:mm\:ss");
            }
            else
            {
                labelTimeSpent.Content = "Time spent on system: ";
            }

            var query = from user in User05Entities5.GetContext().UserssInfo.ToList()
                        select new
                        {
                            Id = user.Id, // Включаем идентификатор пользователя   
                            Date = user.LoginTime.HasValue ? user.LoginTime.Value.ToString("dd.MM.yyyy") : "", // Выводим только дату из LoginTime  
                            LoginTime = user.LoginTime.HasValue ? user.LoginTime.Value.ToString("HH:mm:ss") : "", // Выводим только время из LoginTime  
                            LogoutTime = user.LogoutTime.HasValue ? user.LogoutTime.Value.ToString("HH:mm:ss") : "", // Выводим только время из LogoutTime, если оно есть  
                            user.TimeSpentOnSystem,
                            UnsuccessfulLogoutReason = user.UnsuccesfullLogoutReason ?? "OK" // Устанавливаем значение "OK", если UnsuccessfulLogoutReason равен null  
                        };
            DataUsers.ItemsSource = query.ToList();
            DataUsers.SelectedIndex = 0;
        }

        private TimeSpan? GetLastNonNullTimeSpentOnSystem()
        {
            // Используем LINQ для получения последнего ненулевого значения TimeSpentOnSystem 
            var lastTimeSpent = context.UserssInfo
                .Where(st => st.TimeSpentOnSystem.HasValue)
                .OrderByDescending(st => st.Id) // Предполагаем, что Id - это первичный ключ или идентификатор строки 
                .Select(st => st.TimeSpentOnSystem)
                .FirstOrDefault();

            return lastTimeSpent;
        }

        private void LoadCurrentUser()
        {
            int currentUserId = SessionManager.CurrentUserId;

            // Поиск пользователя по ID   
            currentUser = context.Users.FirstOrDefault(u => u.Id == currentUserId);

            if (currentUser != null)
            {
                WelcomeLabel.Content = $"Hi {currentUser.FirstName}, Welcome to AMONIC Airlines.";
            }
            else
            {
                WelcomeLabel.Content = "Hi, Welcome to AMONIC Airlines.";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AMONIC_Airlines_Automation_System ami = new AMONIC_Airlines_Automation_System();
            ami.Show();
            this.Close();
        }
    }
}