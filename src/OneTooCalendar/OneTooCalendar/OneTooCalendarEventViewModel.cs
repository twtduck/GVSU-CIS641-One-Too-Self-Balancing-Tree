using System;
using System.Windows.Media;

namespace OneTooCalendar
{
	public interface IEventDataModel
	{
		DateTime StartTime { get; }
		DateTime EndTime { get; }
		string Title { get; }
		bool AllDayEvent { get; }
		Color Color { get; }
		EventSynchronizationInfo SyncInfo { get; }
		string Location { get; }
		string Description { get; }
	}
}