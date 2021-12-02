using System;
using System.Windows.Media;

namespace OneTooCalendar
{
	public class CalendarEvent : IEventDataModel
	{
		private readonly CalendarDataModel _sourceCalendar;
		private readonly Color _calendarBackgroundColor;

		public CalendarEvent(CalendarDataModel sourceCalendar, string eventId)
		{
			_sourceCalendar = sourceCalendar;
			_calendarBackgroundColor = ThemeHelper.GetCalendarBackgroundColor(sourceCalendar);
			SyncInfo = new EventSynchronizationInfo(sourceCalendar.Id, eventId);
		}

		public bool AllDayEvent { get; init; }
		public DateTime StartTime { get; init; }
		public DateTime EndTime { get; init; }
		public string Title { get; init; } = "";
		public string Location { get; init; } = "";
		public string Description { get; init; } = "";
		public Color Color => ThemeHelper.TryGetEventBackgroundColor(CustomEventColorId ?? _sourceCalendar.ColorId) ?? _calendarBackgroundColor;
		public EventSynchronizationInfo SyncInfo { get; }
		public int? CustomEventColorId { get; init; }
	}
}