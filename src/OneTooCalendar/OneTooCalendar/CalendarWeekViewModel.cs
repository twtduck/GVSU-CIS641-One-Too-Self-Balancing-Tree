using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace OneTooCalendar
{
    public class CalendarWeekViewModel : ViewModelBase
    {
        public const int DaysInAWeek = 7;

        public CalendarWeekViewModel(DateTime startDate)
        {
            for (var i = 0; i < DaysInAWeek; i++)
            {
                DateViewModels.Add(new DateViewModel(startDate + TimeSpan.FromDays(i))
                {
                    BorderOpacity = i == 0 ? 0 : 1
                });
            }

            StartDate = startDate;
        }

        public DateTime StartDate { get; }

        public List<DateViewModel> DateViewModels { get; } = new();

        public List<HourLabelViewModel> TimeLabels =>
            DateViewModels.First().HourPeriods
            .Select(hourPeriod => new HourLabelViewModel(hourPeriod))
            .ToList();
    }

    public class HourLabelViewModel : ViewModelBase
    {
        public HourLabelViewModel(HourPeriodViewModel hourPeriod)
        {
            LabelContent = hourPeriod.QuarterHourPeriods.First().ToString();
        }

        public string LabelContent { get; }
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

        public List<HourPeriodViewModel> HourPeriods { get; } = new();
        public double BorderOpacity { get; set; }
    }

    public class HourPeriodViewModel : ViewModelBase
    {
        public HourPeriodViewModel(DateTime hourTime)
        {
            var separatorVisible = true;
            for (var time = hourTime; time.Hour == hourTime.Hour; time = time.AddMinutes(15))
            {
                QuarterHourPeriods.Add(new QuarterHourPeriodViewModel(time, separatorVisible));
                separatorVisible = false;
            }
        }

        public List<QuarterHourPeriodViewModel> QuarterHourPeriods { get; } = new();
    }

    public class QuarterHourPeriodViewModel : ViewModelBase
    {
        private readonly DateTime _time;
        private readonly bool _separatorIsVisible;

        public QuarterHourPeriodViewModel(DateTime time, bool separatorIsVisible)
        {
            _time = time;
            _separatorIsVisible = separatorIsVisible;
        }

        public Visibility SeparatorVisibility => _separatorIsVisible ? Visibility.Visible : Visibility.Hidden;

        public override string ToString()
        {
            return _time.ToShortTimeString();
        }
    }
}