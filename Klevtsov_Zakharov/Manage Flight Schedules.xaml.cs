using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;

namespace Klevtsov_Zakharov
{
    public partial class Manage_Flight_Schedules : Window
    {
        public Manage_Flight_Schedules()
        {
            InitializeComponent();
            using (var context = new User05Entities5())
            {
                // Получить список всех аэропортов
                var airports = context.Airports.ToList();

                // Заполнить ComboBox элементы списком аэропортов
                fromComboBox.ItemsSource = airports;
                toComboBox.ItemsSource = airports;

                // Установить свойство для отображаемого текста и значения для каждого элемента ComboBox
                fromComboBox.DisplayMemberPath = "Name"; // Имя аэропорта для отображения
                fromComboBox.SelectedValuePath = "IATACode"; // Код IATA для значения

                toComboBox.DisplayMemberPath = "Name"; // Имя аэропорта для отображения
                toComboBox.SelectedValuePath = "IATACode"; // Код IATA для значения
            }

            using (var context = new User05Entities5())
            {
                var schedulesData = from schedule in context.Schedules
                                    join route in context.Routes on schedule.Routes.FirstOrDefault().Id equals route.Id into Routes
                                    from r in Routes.DefaultIfEmpty()
                                    join departureAirportRoute in context.AirportRoutes on r.Id equals departureAirportRoute.RoutesId into DepartureAirports
                                    from da in DepartureAirports.DefaultIfEmpty()
                                    join departureAirport in context.Airports on da.AiportsId equals departureAirport.Id into DepartureAirportData
                                    from d in DepartureAirportData.DefaultIfEmpty()
                                    join arrivalAirportRoute in context.AirportRoutes on r.Id equals arrivalAirportRoute.RoutesId into ArrivalAirports
                                    from aa in ArrivalAirports.DefaultIfEmpty()
                                    join arrivalAirport in context.Airports on aa.AirportId equals arrivalAirport.Id into ArrivalAirportData
                                    from a in ArrivalAirportData.DefaultIfEmpty()
                                    join aircraft in context.Aircraft on schedule.Aircraft.FirstOrDefault().Id equals aircraft.Id into Aircrafts
                                    from ac in Aircrafts.DefaultIfEmpty()
                                    select new
                                    {
                                        Date = schedule.Date,
                                        Time = schedule.Time,
                                        From = d != null ? d.IATACode.ToString() : "No Departure Airport",
                                        To = a != null ? a.IATACode.ToString() : "No Arrival Airport",
                                        FlightNumber = schedule.FlightNumber != null ? schedule.FlightNumber.ToString() : "No Flight Number",
                                        Aircraft = ac != null ? ac.Name : "No Aircraft",
                                        schedule.EconomyPrice,
                                        BusinessPrice = schedule.EconomyPrice * 2, // BusinessPrice is twice the EconomyPrice
                                        FirstclassPrice = schedule.EconomyPrice * 3
                                    };
                datagrid.ItemsSource = schedulesData.ToList();
            }

        }
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string fromAirport = fromComboBox.SelectedValue?.ToString();
            string toAirport = toComboBox.SelectedValue?.ToString();
            string sortBy = sortByComboBox.SelectedItem?.ToString();
            string date = dateTextBox.Text;
            string flightNumber = flightNumberTextBox.Text;

            using (var context = new User05Entities5())
            {
                var schedulesData = (from schedule in context.Schedules
                                     join route in context.Routes on schedule.Routes.FirstOrDefault().Id equals route.Id into Routes
                                     from r in Routes.DefaultIfEmpty()
                                     join departureAirportRoute in context.AirportRoutes on r.Id equals departureAirportRoute.RoutesId into DepartureAirports
                                     from da in DepartureAirports.DefaultIfEmpty()
                                     join departureAirport in context.Airports on da.AiportsId equals departureAirport.Id into DepartureAirportData
                                     from d in DepartureAirportData.DefaultIfEmpty()
                                     join arrivalAirportRoute in context.AirportRoutes on r.Id equals arrivalAirportRoute.RoutesId into ArrivalAirports
                                     from aa in ArrivalAirports.DefaultIfEmpty()
                                     join arrivalAirport in context.Airports on aa.AirportId equals arrivalAirport.Id into ArrivalAirportData
                                     from a in ArrivalAirportData.DefaultIfEmpty()
                                     join aircraft in context.Aircraft on schedule.Aircraft.FirstOrDefault().Id equals aircraft.Id into Aircrafts
                                     from ac in Aircrafts.DefaultIfEmpty()
                                     select new
                                     {
                                         Date = schedule.Date,
                                         Time = schedule.Time,
                                         From = d != null ? d.IATACode.ToString() : "No Departure Airport",
                                         To = a != null ? a.IATACode.ToString() : "No Arrival Airport",
                                         FlightNumber = schedule.FlightNumber != null ? schedule.FlightNumber.ToString() : "No Flight Number",
                                         Aircraft = ac != null ? ac.Name : "No Aircraft",
                                         schedule.EconomyPrice,
                                         BusinessPrice = schedule.EconomyPrice * 2, // BusinessPrice is twice the EconomyPrice
                                         FirstclassPrice = schedule.EconomyPrice * 3
                                     }).ToList();

                var filteredSchedules = schedulesData;

                // Apply filtering based on selected criteria
                if (!string.IsNullOrEmpty(fromAirport))
                    filteredSchedules = filteredSchedules.Where(schedule => schedule.From == fromAirport || schedule.From == "No Departure Airport").ToList();

                if (!string.IsNullOrEmpty(toAirport))
                    filteredSchedules = filteredSchedules.Where(schedule => schedule.To == toAirport || schedule.To == "No Arrival Airport").ToList();

                if (!string.IsNullOrEmpty(date))
                {
                    DateTime parsedDate;
                    if (DateTime.TryParse(date, out parsedDate))
                        filteredSchedules = filteredSchedules.Where(schedule => schedule.Date.HasValue && schedule.Date.Value.Date == parsedDate.Date).ToList();
                    else
                        MessageBox.Show("Invalid date format. Please enter a valid date.");
                }

                if (!string.IsNullOrEmpty(flightNumber))
                {
                    int flightNumberValue;
                    if (int.TryParse(flightNumber, out flightNumberValue))
                    {
                        filteredSchedules = filteredSchedules.Where(schedule => schedule.FlightNumber == flightNumberValue.ToString()).ToList();
                    }
                }

                // Apply sorting
                if (sortBy == "Date")
                    filteredSchedules = filteredSchedules.OrderBy(schedule => schedule.Date).ToList();
                else if (sortBy == "Time")
                    filteredSchedules = filteredSchedules.OrderBy(schedule => schedule.Time).ToList();

                // Update the DataGrid with the filtered and sorted schedules
                datagrid.ItemsSource = filteredSchedules;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли какой-либо элемент в DataGrid
            if (datagrid.SelectedItem != null)
            {
                // Получаем выбранный элемент (ваш тип данных)
                dynamic selectedSchedule = datagrid.SelectedItem;

                // Создаем окно редактирования, передавая в него выбранный элемент
                Schedule_edit scheduleEditWindow = new Schedule_edit(selectedSchedule);

                // Отображаем окно редактирования
                scheduleEditWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a schedule to edit.");
            }
        }

        private void CancelFlightButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли какой-либо элемент в DataGrid
            if (datagrid.SelectedItem != null)
            {
                // Получаем выбранный элемент (ваш тип данных)
                dynamic selectedSchedule = datagrid.SelectedItem;

                // Вызываем метод удаления из базы данных, передавая выбранный элемент
                //context.CancelFlight(selectedSchedule); // Здесь context - ваш объект контекста базы данных
                MessageBox.Show("Flight canceled successfully."); // Оповещение об успешном удалении
            }
            else
            {
                MessageBox.Show("Please select a schedule to cancel.");
            }
        }


    }
}
