using System;
using System.Windows;

namespace OneTooCalendar
{
	public class QuarterHourPeriodViewModel : ViewModelBase
	{
		private readonly DateTime _time;
		private readonly bool _separatorIsVisible;
		public const int QuarterHourPeriodHeight = 12;

		public QuarterHourPeriodViewModel(DateTime time, bool separatorIsVisible)
		{
			_time = time;
			_separatorIsVisible = separatorIsVisible;
		}

		public Visibility SeparatorVisibility => _separatorIsVisible ? Visibility.Visible : Visibility.Hidden;

		public override string ToString()
		{
			return _time.ToShortTimeString();
		}
	}
}