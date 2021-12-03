using System;
using System.Windows.Media;
using Google.Apis.Calendar.v3.Data;

namespace OneTooCalendar
{
	public class GoogleCalendarEventDataModel : IEventDataModel
	{
		private readonly Color _calendarBackgroundColor;

		public GoogleCalendarEventDataModel(ICalendarDataModel sourceCalendar, string eventId)
		{
			Calendar = sourceCalendar;
			EventId = eventId;
			_calendarBackgroundColor = ThemeHelper.GetCalendarBackgroundColor(sourceCalendar);
		}

		public bool AllDayEvent { get; init; }
		public DateTime StartTime { get; init; }
		public DateTime EndTime { get; init; }
		public string Title { get; init; } = "";
		public string Location { get; init; } = "";
		public string Description { get; init; } = "";
		public Color Color => ThemeHelper.TryGetEventBackgroundColor(CustomEventColorId) ?? ThemeHelper.GetCalendarBackgroundColor(Calendar);
		public ICalendarDataModel Calendar { get; }
		public string EventId { get; }
		public int? CustomEventColorId { get; init; }
		public Event SourceEvent { get; init; } = null!;
	}
}