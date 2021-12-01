using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Google.Apis.Calendar.v3.Data;
using MaterialDesignColors;

namespace OneTooCalendar
{
	public class ThemeHelper
	{
		public static Color CalendarDividerColor { get; } = (Color)ColorConverter.ConvertFromString("#999999");
		public static Brush CalendarDivider { get; } = new SolidColorBrush(CalendarDividerColor);

		private static Color[] PrimaryColors { get; } = Enum.GetValues(typeof(MaterialDesignColors.PrimaryColor)).Cast<PrimaryColor>().Cast<int>()
			.Cast<MaterialDesignColor>().Select(x => MaterialDesignColors.SwatchHelper.Lookup[x]).ToArray();

		private static List<string> _calendars = new List<string>();

		public static Color ColorFromCalendar(Calendar calendar)
		{
			if (!_calendars.Contains(calendar.Id))
				_calendars.Add(calendar.Id);

			return PrimaryColors[_calendars.IndexOf(calendar.Id) % PrimaryColors.Length];
		}

	}

	public class CalendarEvent : IEventDataModel
	{
		private readonly Calendar _sourceCalendar;

		public CalendarEvent(Calendar sourceCalendar)
		{
			_sourceCalendar = sourceCalendar;
			Color = ThemeHelper.ColorFromCalendar(_sourceCalendar);
		}

		public bool AllDayEvent { get; init; }
		public DateTime StartTime { get; init; }
		public DateTime EndTime { get; init; }
		public string Title { get; init; }
		public string Location { get; init; }
		public Color Color { get; }
	}

	public class CalendarFeed
	{

	}
}
