using System;

namespace OneTooCalendar
{
    public class CalendarViewModel : ViewModelBase
    {
        private CalendarWeekViewModel _calendarWeekViewModel;
        private CalendarControlsBarViewModel _calendarControlsBarViewModel;
        private BacklogViewModel _backlogViewModel;

        public CalendarViewModel()
        {
            var firstDayThisWeek = DateTime.Today.Subtract(TimeSpan.FromDays((int)DateTime.Today.DayOfWeek));
            _calendarWeekViewModel = new CalendarWeekViewModel(firstDayThisWeek);
            _calendarControlsBarViewModel = new CalendarControlsBarViewModel(() => { }, () => { });
            _backlogViewModel = new BacklogViewModel();
        }

        public CalendarWeekViewModel CalendarWeekViewModel
        {
            get => _calendarWeekViewModel;
            set
            {
                _calendarWeekViewModel = value;
                OnPropertyChanged();
            }
        }
        
        public CalendarControlsBarViewModel CalendarControlsBarViewModel
        {
            get => _calendarControlsBarViewModel;
            set
            {
                _calendarControlsBarViewModel = value;
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