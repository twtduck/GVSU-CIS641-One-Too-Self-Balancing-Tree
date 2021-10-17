using System;
using System.Globalization;
using System.Linq;

namespace OneTooCalendar
{
    public class CalendarViewModel : ViewModelBase
    {
        private CalendarWeekViewModel _calendarWeekViewModel;
        private BacklogViewModel _backlogViewModel;
        private string _currentMonthAndYear = string.Empty;

        public CalendarViewModel()
        {
            _calendarWeekViewModel = new CalendarWeekViewModel(GetFirstDayOfCurrentWeek());
            UpdateCurrentMonthAndYear();
            _backlogViewModel = new BacklogViewModel();
            PreviousWeekButtonCommand = new OneTooCalendarCommand(_ => CalendarWeekViewModel =
                new CalendarWeekViewModel(CalendarWeekViewModel.StartDate.AddDays(-CalendarWeekViewModel.DaysInAWeek)));
            NextWeekButtonCommand = new OneTooCalendarCommand(_ => CalendarWeekViewModel =
                new CalendarWeekViewModel(CalendarWeekViewModel.StartDate.AddDays(CalendarWeekViewModel.DaysInAWeek)));
            TodayButtonCommand = new OneTooCalendarCommand(_ =>
                CalendarWeekViewModel = new CalendarWeekViewModel(GetFirstDayOfCurrentWeek()));

            DateTime GetFirstDayOfCurrentWeek()
            {
                return DateTime.Today.Subtract(TimeSpan.FromDays((int)DateTime.Today.DayOfWeek));
            }
        }

        public OneTooCalendarCommand NextWeekButtonCommand { get; }

        public OneTooCalendarCommand PreviousWeekButtonCommand { get; }

        public OneTooCalendarCommand TodayButtonCommand { get; }

        public CalendarWeekViewModel CalendarWeekViewModel
        {
            get => _calendarWeekViewModel;
            set
            {
                _calendarWeekViewModel = value;
                UpdateCurrentMonthAndYear();
                OnPropertyChanged();
            }
        }

        private void UpdateCurrentMonthAndYear()
        {
            var date = CalendarWeekViewModel.StartDate;
            var months = CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames;
            CurrentMonthAndYear = $"{months[date.Month - 1]} {date.Year}";
        }

        public string CurrentMonthAndYear
        {
            get => _currentMonthAndYear;
            set
            {
                _currentMonthAndYear = value;
                OnPropertyChanged();
            }
        }

        public BacklogViewModel BacklogViewModel
        {
            get => _backlogViewModel;
            set
            {
                _backlogViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}