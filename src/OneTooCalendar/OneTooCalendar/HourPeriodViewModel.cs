using System;
using System.Collections.Generic;

namespace OneTooCalendar
{
	public class HourPeriodViewModel : ViewModelBase
	{
		public HourPeriodViewModel(DateTime hourTime)
		{
			var separatorVisible = true;
			for (var time = hourTime; time.Hour == hourTime.Hour; time = time.AddMinutes(15))
			{
				QuarterHourPeriods.Add(new QuarterHourPeriodViewModel(time, separatorVisible));
				separatorVisible = false;
			}
		}

		public List<QuarterHourPeriodViewModel> QuarterHourPeriods { get; } = new List<QuarterHourPeriodViewModel>();
	}
}