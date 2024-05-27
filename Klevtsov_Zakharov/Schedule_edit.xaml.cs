using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class Schedule_edit : Window
    {
        private Schedules schedules;
        private Aircraft aircraft;
        private Airports airports;
        private Routes routes;
        private Airports airports1;

        public Schedule_edit(Schedules schedules)
        {
            InitializeComponent();
            this.schedules = schedules;

            var a = User05Entities5.GetContext().Aircraft.FirstOrDefault(x => x.Schedules.Any(y => y.Id == schedules.Id)).Id;
            var context = User05Entities5.GetContext();

            GetAirports(context);
            GetAircraft(context);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Получите данные из полей
            string date = dateBox.Text;
            string time = timeBox.Text;
            string economyPrice = priceBox.Text;
            // Получите данные об аэропортах и самолете из выпадающих списков

            // Обновите объект schedules с новыми данными

            // Сохраните изменения в базе данных

            MessageBox.Show("Schedule updated successfully.");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Manage_Flight_Schedules mfs = new Manage_Flight_Schedules();
            mfs.Show();
            this.Close();
        }
        private void GetAircraft(User05Entities5 context)
        {
            aircraft = context.Aircraft.FirstOrDefault(r => r.Schedules.Any(u => u.Id == schedules.Id));
        }

        // Метод для получения офиса пользователя
        private void GetAirports(User05Entities5 context)
        {
            airports = context.Airports.FirstOrDefault(o => o.AirportRoutes.Any(u => u.AirportId == routes.Id));
            airports1 = context.Airports.FirstOrDefault(o => o.AirportRoutes.Any(u => u.AiportsId == routes.Id));
        }
    }
}
