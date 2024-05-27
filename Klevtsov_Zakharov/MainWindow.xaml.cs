using Klevtsov_Zakharov.Resources;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Klevtsov_Zakharov
{
    /// <summary> 
    /// Логика взаимодействия для MainWindow.xaml 
    /// </summary> 
    public partial class MainWindow : Window
    {
        private readonly User05Entities5 context;
        private int CurrentUserId;

        public MainWindow()
        {
            InitializeComponent();
            context = User05Entities5.GetContext();

            // Подписываемся на событие выхода из приложения 
            Application.Current.Exit += new ExitEventHandler(OnExit);
        }

        public string Output()
        {
            try
            {
                string user = username.Text.Trim();
                string password = pass.Password.Trim();
                if (user.Length < 4 || password.Length < 5)
                {
                    username.Background = Brushes.Red;
                    pass.Background = Brushes.Red;
                    MessageBox.Show("Данные введены некорректно");
                }
                else
                {
                    var employee = User05Entities5.GetContext().Users
                        .Where(u => u.Email == user && u.Password == password)
                        .FirstOrDefault();

                    if (employee == null)
                    {
                        MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        DateTime currentMoscowTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Russian Standard Time");
                        
                        // Создание записи о сессии пользователя  
                        var session = new UserssInfo
                        {
                            UserId = employee.Id, // Установка UserId 
                            LoginTime = currentMoscowTime
                        };

                        // Сохранение UserId текущего пользователя 
                        SessionManager.CurrentUserId = employee.Id;
                        CurrentUserId = employee.Id;

                        // Добавление сессии в контекст и сохранение изменений 
                        context.UserssInfo.Add(session);
                        context.SaveChanges();

                        var roleId = employee.Roles.FirstOrDefault()?.Id;
                        if (roleId.HasValue)
                        {
                            switch (roleId.Value)
                            {
                                case 1:
                                    MessageBox.Show("Вы вошли под учётной записью администратора отдела по работе с клиентами", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                                    AMONIC_Airlines_Automation_System amonic = new AMONIC_Airlines_Automation_System();
                                    amonic.Show();
                                    this.Close();
                                    break;
                                case 2:
                                    MessageBox.Show("Вы вошли под учётной записью менеджера по работе с клиентами", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Manage_Flight_Schedules usw = new Manage_Flight_Schedules();
                                    usw.Show();
                                    this.Close();
                                    break;
                            }
                        }
                    }
                }
                return "Хорошо";
            }
            catch
            {
                return "Плохо";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Output();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void OnExit(object sender, ExitEventArgs e)
        {
            try
            {
                // Получаем текущую запись о сессии пользователя 
                var currentSession = context.UserssInfo
                    .Where(u => u.UserId == CurrentUserId)
                    .OrderByDescending(u => u.LoginTime)
                    .FirstOrDefault();

                if (currentSession != null)
                {
                    DateTime currentMoscowTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Russian Standard Time");
                    currentSession.LogoutTime = currentMoscowTime;

                    // Вычисляем время, проведенное в системе 
                    TimeSpan timeSpent = currentSession.LogoutTime.Value - currentSession.LoginTime.Value;

                    // Преобразуем TimeSpan в формат времени "часы:минуты:секунды" 
                    currentSession.TimeSpentOnSystem = TimeSpan.ParseExact(timeSpent.ToString(@"hh\:mm\:ss"), @"hh\:mm\:ss", null);

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений 
                MessageBox.Show("Ошибка при обновлении времени выхода: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}