using System;
using System.Windows.Media;
using Google.Apis.Calendar.v3.Data;

namespace OneTooCalendar
{
    public static class ThemeCustomization
    {
        public static Color CalendarDividerColor { get; } = (Color)ColorConverter.ConvertFromString("#999999");
        public static Brush CalendarDivider { get; } = new SolidColorBrush(CalendarDividerColor);
    }

    public class CalendarEvent : IEventViewModel
    {
        private readonly Calendar _sourceCalendar;

        public CalendarEvent(Calendar sourceCalendar)
        {
            _sourceCalendar = sourceCalendar;
        }

        public bool AllDayEvent { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public string Title { get; init; }
        public string Location { get; init; }
    }

    public class CalendarFeed
    {

    }
}
