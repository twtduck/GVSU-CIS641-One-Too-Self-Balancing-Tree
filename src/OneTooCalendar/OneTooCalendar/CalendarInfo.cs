using Google.Apis.Calendar.v3.Data;

namespace OneTooCalendar
{
	public class CalendarDataModel : ICalendarDataModel
	{
		public CalendarDataModel(Calendar calendar)
		{
			GoogleCalendar = calendar;
		}

		public string Name => GoogleCalendar.Summary;
		public string Id => GoogleCalendar.Id;
		public Calendar GoogleCalendar { get; }
	}

	public interface ICalendarDataModel
	{
		string Name { get; }
		string Id { get; }
	}
}