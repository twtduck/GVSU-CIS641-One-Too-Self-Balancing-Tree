using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MaterialDesignColors;

namespace OneTooCalendar
{
	public class ThemeHelper
	{
		public static Color CalendarDividerColor { get; } = (Color)ColorConverter.ConvertFromString("#999999");
		public static Brush CalendarDivider { get; } = new SolidColorBrush(CalendarDividerColor);

		private static Color[] PrimaryColors { get; } = Enum.GetValues(typeof(PrimaryColor)).Cast<PrimaryColor>().Cast<int>()
			.Cast<MaterialDesignColor>().Select(x => SwatchHelper.Lookup[x]).ToArray();

		private static List<string> _calendars = new List<string>();

		public static Color ColorFromCalendar(CalendarDataModel calendar)
		{
			if (!_calendars.Contains(calendar.Id))
				_calendars.Add(calendar.Id);

			return PrimaryColors[_calendars.IndexOf(calendar.Id) % PrimaryColors.Length];
		}
	}
}
