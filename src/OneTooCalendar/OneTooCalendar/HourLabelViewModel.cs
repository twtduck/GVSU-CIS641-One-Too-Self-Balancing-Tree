using System;

namespace OneTooCalendar
{
	public class HourLabelViewModel : ViewModelBase
	{
		public HourLabelViewModel(int hourNumber)
		{
			LabelContent = DateTime.Today.AddHours(hourNumber).ToShortTimeString();
		}

		public string LabelContent { get; }
	}
}