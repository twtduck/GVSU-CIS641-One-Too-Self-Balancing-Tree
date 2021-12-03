using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OneTooCalendar
{
	public static class EventsApiExtensions
	{
		public static void ClearCachesAndGetEventsForWeek(this IEventsApi eventsApi, DateTime weekStart, Action<IList<IEventDataModel>?> onCompletion)
		{
			var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
			eventsApi.TrySynchronizeAndClearCachesAsync(token)
				.RunCatchingFailure()
				.ContinueWith(
					synchronizationTask =>
						(synchronizationTask.Result
							? eventsApi.TryGetWeekEventsAsync(weekStart, token)
							: Task.FromResult(default(IList<IEventDataModel>?)))
						.RunCatchingFailure()
						.ContinueWith(
							getEventsTask => onCompletion(getEventsTask.Result),
							token
							),
					token
					);
		}
	}
}