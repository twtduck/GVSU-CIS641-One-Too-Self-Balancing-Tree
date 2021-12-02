using System;
using System.Windows.Media;

namespace OneTooCalendar
{
	public class CalendarEvent : IEventDataModel
	{
		private readonly CalendarDataModel _sourceCalendar;

		public CalendarEvent(CalendarDataModel sourceCalendar, string eventId)
		{
			_sourceCalendar = sourceCalendar;
			Color = ThemeHelper.ColorFromCalendar(_sourceCalendar);
			SyncInfo = new EventSynchronizationInfo(_sourceCalendar.Id, eventId);
		}

		public bool AllDayEvent { get; init; }
		public DateTime StartTime { get; init; }
		public DateTime EndTime { get; init; }
		public string Title { get; init; } = "";
		public string Location { get; init; } = "";
		public string Description { get; init; } = "";
		public Color Color { get; }
		public EventSynchronizationInfo SyncInfo { get; }
	}
}