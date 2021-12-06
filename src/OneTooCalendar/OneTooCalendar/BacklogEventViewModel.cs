using System;
using System.Windows.Media;

namespace OneTooCalendar
{
	public class BacklogEventViewModel : ViewModelBase
	{
		public IEventDataModel EventDataModel { get; }

		public BacklogEventViewModel(IEventDataModel eventDataModel)
		{
			EventDataModel = eventDataModel;
		}

		public string Title => EventDataModel.Title;
		public SolidColorBrush Color => new SolidColorBrush(EventDataModel.Color);
		public TimeSpan Duration => EventDataModel?.EndTime - EventDataModel?.StartTime ?? TimeSpan.FromHours(1);
	}
}