using System;
using System.Windows.Media;

namespace OneTooCalendar
{
	public interface IEventDataModel
	{
		DateTime StartTime { get; set; }
		DateTime EndTime { get; set; }
		string Title { get; set; }
		bool AllDayEvent { get; }
		Color Color { get; }
		string Location { get; set; }
		string? Description { get; set; }
		int? CustomEventColorId { get; set; }
		ICalendarDataModel Calendar { get; }
		string EventId { get; set; }
		bool EventIdIsTemporary { get; set; }
	}
}