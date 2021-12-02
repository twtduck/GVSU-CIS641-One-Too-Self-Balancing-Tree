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

		public static Dictionary<int, Color>? CalendarBackgroundColorMap { get; set; }
		public static Dictionary<int, Color>? EventBackgroundColorMap { get; set; }
		public static Dictionary<int, string> EventBackgroundColorNames => new Dictionary<int, string>
		{
			{ 1, "1" },
			{ 2, "2" },
			{ 3, "3" },
			{ 4, "4" },
			{ 5, "5" },
			{ 6, "6" },
			{ 7, "7" },
			{ 8, "8" },
			{ 9, "9" },
			{ 10, "10" },
			{ 11, "11" },
		};
		public static Dictionary<int, Color>? CalendarForegroundColorMap { get; set; }
		public static Dictionary<int, Color>? EventForegroundColorMap { get; set; }

		private static List<string> _calendars = new List<string>();

		public static Color? TryGetEventBackgroundColor(int eventColorId) =>
			EventBackgroundColorMap?.TryGetValue(eventColorId, out var color) == true ? color : default(Color?);
		public static Color? TryGetEventForegroundColor(int eventColorId) =>
			EventForegroundColorMap?.TryGetValue(eventColorId, out var color) == true ? color : default(Color?);

		public static Color GetCalendarBackgroundColor(CalendarDataModel calendar)
		{
			if (CalendarBackgroundColorMap?.TryGetValue(calendar.ColorId, out var color) ?? false)
				return color;

			if (!_calendars.Contains(calendar.Id))
				_calendars.Add(calendar.Id);

			return PrimaryColors[_calendars.IndexOf(calendar.Id) % PrimaryColors.Length];
		}
	}
}
