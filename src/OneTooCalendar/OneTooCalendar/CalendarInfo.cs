using Google.Apis.Calendar.v3.Data;

namespace OneTooCalendar
{
	public class CalendarDataModel : ICalendarDataModel
	{
		private readonly CalendarListEntry _calendarListEntry;
		private readonly Calendar _googleCalendar;

		public CalendarDataModel(Calendar calendar, CalendarListEntry calendarListEntry)
		{
			_calendarListEntry = calendarListEntry;
			_googleCalendar = calendar;
			ColorId = int.Parse(_calendarListEntry.ColorId);
		}

		public string Name => _googleCalendar.Summary;
		public string Id => _googleCalendar.Id;
		public int ColorId { get; }
		public bool IsPrimary => _calendarListEntry.Primary == true;
	}

	public interface ICalendarDataModel
	{
		string Name { get; }
		string Id { get; }
		public int ColorId { get; }
	}
}