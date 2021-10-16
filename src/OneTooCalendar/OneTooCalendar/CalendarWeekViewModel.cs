using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Documents;

namespace OneTooCalendar
{
    public class CalendarWeekViewModel : ViewModelBase
    {
        public CalendarWeekViewModel(DateTime startDate)
        {
            for (var i = 0; i < DaysInAWeek; i++)
            {
                DateViewModels.Add(new DateViewModel(startDate + TimeSpan.FromDays(i)));
            }
        }

        private const int DaysInAWeek = 7;
        public List<DateViewModel> DateViewModels { get; } = new();

        public List<string> TimeLabels => DateViewModels.First().HourPeriods
            .Select(hourPeriodViewModel => hourPeriodViewModel.QuarterHourPeriods.First().ToString()).ToList();
    }

    public class DateViewModel : ViewModelBase
    {
        private readonly DateTime _dateTime;

        public DateViewModel(DateTime dateTime)
        {
            _dateTime = dateTime;
            for (var hourTime = _dateTime; hourTime.Date == _dateTime; hourTime = hourTime.AddHours(1))
            {
                HourPeriods.Add(new HourPeriodViewModel(hourTime));
            }
        }

        public string DayOfTheWeek => CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames[(int)_dateTime.DayOfWeek];

        public int DayNumber => _dateTime.Day;

        public List<HourPeriodViewModel> HourPeriods { get; } = new List<HourPeriodViewModel>();
    }

    public class HourPeriodViewModel : ViewModelBase
    {
        public HourPeriodViewModel(DateTime hourTime)
        {
            for (var time = hourTime; time.Hour == hourTime.Hour; time = time.AddMinutes(15))
            {
                QuarterHourPeriods.Add(new QuarterHourPeriodViewModel(time));
            }
        }

        public List<QuarterHourPeriodViewModel> QuarterHourPeriods { get; } = new List<QuarterHourPeriodViewModel>();
    }

    public class QuarterHourPeriodViewModel : ViewModelBase
    {
        private readonly DateTime _time;

        public QuarterHourPeriodViewModel(DateTime time)
        {
            _time = time;
        }

        public override string ToString()
        {
            return _time.ToShortTimeString();
        }
    }
}