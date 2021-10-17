using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;

namespace OneTooCalendar
{
    public static class ThemeCustomization
    {
        public static Color CalendarDividerColor { get; } = (Color)ColorConverter.ConvertFromString("#999999");
        public static Brush CalendarDivider { get; } = new SolidColorBrush(CalendarDividerColor);
    }
}
