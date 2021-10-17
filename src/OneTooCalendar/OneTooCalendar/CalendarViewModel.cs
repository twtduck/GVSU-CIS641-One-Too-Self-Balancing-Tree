using System;
using System.Globalization;
using System.Threading;
using System.Windows.Media.Imaging;

namespace OneTooCalendar
{
    public class CalendarViewModel : ViewModelBase
    {
        private readonly GoogleCalendarService _googleCalendarService;
        private CalendarWeekViewModel _calendarWeekViewModel;
        private BacklogViewModel _backlogViewModel;
        private string _currentMonthAndYear = string.Empty;

        public CalendarViewModel(GoogleCalendarService googleCalendarService)
        {
            _googleCalendarService = googleCalendarService;
            _calendarWeekViewModel = new CalendarWeekViewModel(GetFirstDayOfCurrentWeek(), _googleCalendarService);
            UpdateCurrentMonthAndYear();
            _backlogViewModel = new BacklogViewModel();
            PreviousWeekButtonCommand = new OneTooCalendarCommand(_ => CalendarWeekViewModel =
                new CalendarWeekViewModel(CalendarWeekViewModel.StartDate.AddDays(-CalendarWeekViewModel.DaysInAWeek), _googleCalendarService));
            NextWeekButtonCommand = new OneTooCalendarCommand(_ => CalendarWeekViewModel =
                new CalendarWeekViewModel(CalendarWeekViewModel.StartDate.AddDays(CalendarWeekViewModel.DaysInAWeek), _googleCalendarService));
            TodayButtonCommand = new OneTooCalendarCommand(_ =>
                CalendarWeekViewModel = new CalendarWeekViewModel(GetFirstDayOfCurrentWeek(), _googleCalendarService));
            RefreshButtonCommand = new OneTooCalendarCommand(_ =>
                OnRefreshButtonClicked()
                );

            DateTime GetFirstDayOfCurrentWeek()
            {
                return DateTime.Today.Subtract(TimeSpan.FromDays((int)DateTime.Today.DayOfWeek));
            }
        }

        private void OnRefreshButtonClicked()
        {
            TemporarilySetSynchronizationView.before();
            var refreshTask = CalendarWeekViewModel.TryRefreshEventsAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token)
                .RunCatchingFailure().ContinueWith(completedTask => TemporarilySetSynchronizationView.after()); // TODO handle completed task result
        }

        public OneTooCalendarCommand NextWeekButtonCommand { get; }

        public OneTooCalendarCommand PreviousWeekButtonCommand { get; }

        public OneTooCalendarCommand TodayButtonCommand { get; }
        public OneTooCalendarCommand RefreshButtonCommand { get; }

        public CalendarWeekViewModel CalendarWeekViewModel
        {
            get => _calendarWeekViewModel;
            set
            {
                TemporarilySetSynchronizationView.before.Invoke();
                _calendarWeekViewModel = value;
                UpdateCurrentMonthAndYear();
                OnPropertyChanged();
                CalendarWeekViewModel.TryRefreshEventsAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token)
                    .RunCatchingFailure()
                    .ContinueWith(_ => TemporarilySetSynchronizationView.after.Invoke()); // TODO handle false
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

        public (Action before, Action after) TemporarilySetSynchronizationView { get; set; }
    }
}