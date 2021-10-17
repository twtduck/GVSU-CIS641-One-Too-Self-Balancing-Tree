using System.Windows.Media;

namespace OneTooCalendar
{
    public static class ThemeCustomization
    {
        public static Color CalendarDividerColor { get; } = (Color)ColorConverter.ConvertFromString("#999999");
        public static Brush CalendarDivider { get; } = new SolidColorBrush(CalendarDividerColor);
    }
}
