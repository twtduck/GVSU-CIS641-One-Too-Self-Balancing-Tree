using System;
using System.Windows.Input;

namespace OneTooCalendar
{
    public class CalendarControlsBarViewModel : ViewModelBase
    {
        public CalendarControlsBarViewModel(Action showNextWeek, Action showPreviousWeek)
        {
            PreviousWeekButtonCommand = new OneTooCalendarCommand(_ => showPreviousWeek());
            NextWeekButtonCommand = new OneTooCalendarCommand(_ => showNextWeek());
        }
        public ICommand PreviousWeekButtonCommand { get; }

        public ICommand NextWeekButtonCommand { get; }
    }
}