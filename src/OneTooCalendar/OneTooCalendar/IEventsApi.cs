using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OneTooCalendar
{
	public interface IEventsApi
	{
		void DeleteEvent(IEventDataModel eventDataModel);
		Task<IList<IEventDataModel>?> TryGetWeekEventsAsync(DateTime weekStart, CancellationToken token);
		Task<bool> TrySynchronizeAndClearCachesAsync(CancellationToken token);
	}
}