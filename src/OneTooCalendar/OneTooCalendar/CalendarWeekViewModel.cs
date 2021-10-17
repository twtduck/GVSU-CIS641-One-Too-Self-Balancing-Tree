using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Google.Apis.Calendar.v3.Data;

namespace OneTooCalendar
{
    public class CalendarWeekViewModel : ViewModelBase
    {
        private readonly GoogleCalendarService _googleCalendarService;
        public const int DaysInAWeek = 7;

        public CalendarWeekViewModel(DateTime startDate, GoogleCalendarService googleCalendarService)
        {
            _googleCalendarService = googleCalendarService;
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
            Enumerable.Range(0, 24)
            .Select(hourNumber => new HourLabelViewModel(hourNumber))
            .ToList();

        public async Task<bool> TryRefreshEventsAsync(CancellationToken token)
        {
            var events = await _googleCalendarService.GetEventsForDateRangeAsync(StartDate, StartDate.AddDays(DaysInAWeek), token);
            if (events is null)
                return false;
            foreach (var dateViewModel in DateViewModels)
            {
                dateViewModel.UpdateFromEventsList(events);
            }
            return true;

        }
    }

    public class HourLabelViewModel : ViewModelBase
    {
        public HourLabelViewModel(int hourNumber)
        {
            LabelContent = DateTime.Today.AddHours(hourNumber).ToShortTimeString();
        }

        public string LabelContent { get; }
    }

    public class DateViewModel : ViewModelBase
    {
        private readonly DateTime _dateTime;

        public DateViewModel(DateTime dateTime)
        {
            _dateTime = dateTime;

            EventGridList = new ObservableCollection<Grid>(new[] { BuildEventGrid() });
        }

        private static Grid BuildEventGrid()
        {
            var eventGrid = new Grid();
            for (int i = 0; i < 24 * 4; i++)
            {
                eventGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(12) });
            }

            for (int i = 0; i < 24 * 4; i++)
            {
                if (i % 4 == 0)
                {
                    var dividerGrid = new Grid();
                    dividerGrid.SetValue(Grid.RowProperty, i);
                    dividerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
                    dividerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(11) });
                    var border = new Border
                    {
                        BorderBrush = ThemeCustomization.CalendarDivider,
                        BorderThickness = new Thickness(0, 1, 0, 0)
                    };
                    border.SetValue(Grid.RowProperty, 0);
                    dividerGrid.Children.Add(border);
                    eventGrid.Children.Add(dividerGrid);
                }
            }

            return eventGrid;
        }

        public string DayOfTheWeek => CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames[(int)_dateTime.DayOfWeek];

        public int DayNumber => _dateTime.Day;

        public ObservableCollection<Grid> EventGridList { get; }
        public double BorderOpacity { get; set; }

        public void UpdateFromEventsList(IList<IEventViewModel> events)
        {
            EventGridList.Clear();
            EventGridList.Add(BuildEventGrid());
        }
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