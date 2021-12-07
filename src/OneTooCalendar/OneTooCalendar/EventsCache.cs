using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OneTooCalendar
{
	public class EventsCache : IEventsApi
	{
		public EventsCache(GoogleCalendarApi googleCalendarApi)
		{
			_googleCalendarApi = googleCalendarApi;
		}
		private readonly GoogleCalendarApi _googleCalendarApi;
		private List<IEventDataModel> EventsToDeleteOnNextSync { get; } = new List<IEventDataModel>();
		private List<IEventDataModel> EventsToAddOnNextSync { get; } = new List<IEventDataModel>();
		private List<IEventDataModel> EventsToUpdateOnNextSync { get; } = new List<IEventDataModel>();
		private Dictionary<DateTime, IList<IEventDataModel>> WeekEventCacheEntries { get; } = new Dictionary<DateTime, IList<IEventDataModel>>();

		public void DeleteEvent(IEventDataModel eventDataModel)
		{
			if (EventsToAddOnNextSync.Contains(eventDataModel))
				EventsToAddOnNextSync.Remove(eventDataModel);
			else
				EventsToDeleteOnNextSync.Add(eventDataModel);
			EventsToUpdateOnNextSync.Remove(eventDataModel);

			Debug.Assert(!EventsToAddOnNextSync.Contains(eventDataModel));
			Debug.Assert(!EventsToUpdateOnNextSync.Contains(eventDataModel));
			foreach (var weekEventCacheEntry in WeekEventCacheEntries)
				weekEventCacheEntry.Value.Remove(eventDataModel);
		}

		public void AddEvent(IEventDataModel eventDataModel)
		{
			EventsToAddOnNextSync.Add(eventDataModel);
			Debug.Assert(!EventsToDeleteOnNextSync.Contains(eventDataModel));
			Debug.Assert(!EventsToUpdateOnNextSync.Contains(eventDataModel));
			foreach (var weekEventCacheEntry in WeekEventCacheEntries)
				weekEventCacheEntry.Value.Add(eventDataModel);
		}

		public void UpdateEvent(IEventDataModel eventDataModel)
		{
			if (EventsToAddOnNextSync.Contains(eventDataModel))
				return;

			if (!EventsToUpdateOnNextSync.Contains(eventDataModel))
				EventsToUpdateOnNextSync.Add(eventDataModel);

			Debug.Assert(!EventsToDeleteOnNextSync.Contains(eventDataModel));
			Debug.Assert(!EventsToAddOnNextSync.Contains(eventDataModel));
		}

		public Task<IList<IEventDataModel>?> TryGetWeekEventsAsync(DateTime weekStart, CancellationToken token)
		{
			return GetEventsForDateRangeAsync(weekStart, weekStart.AddDays(7), token);
		}

		private async Task<IList<IEventDataModel>?> GetEventsForDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken token)
		{
			var isValidCacheRequest = startDate.Hour == 0 &&
				startDate.Minute == 0 &&
				startDate.Second == 0 && startDate.DayOfWeek == DayOfWeek.Sunday && endDate == startDate.AddDays(7);

			Debug.Assert(isValidCacheRequest);

			if (isValidCacheRequest)
			{
				var cacheFound = WeekEventCacheEntries.TryGetValue(startDate, out var eventsForWeek);
				if (cacheFound)
				{
					return eventsForWeek;
				}
			}

			var events = await _googleCalendarApi.GetEventsForDateRangeAsync(startDate, endDate, token);

			if (isValidCacheRequest && events is not null)
				WeekEventCacheEntries.Add(startDate, events);

			return events;
		}

		public async Task<bool> TrySynchronizeAndClearCachesAsync(CancellationToken token)
		{
			if (!await CheckForInternetConnectionAsync(8000, url: "http://www.google.com"))
				return false;

			if ((await Task.WhenAll(EventsToDeleteOnNextSync.Select(x => _googleCalendarApi.TryDeleteEventAsync(x, token)))).Any(x => !x))
				return false;

			if ((await Task.WhenAll(EventsToAddOnNextSync.Select(x => _googleCalendarApi.TryAddEventAsync(x, token)))).Any(x => !x))
				return false;

			if ((await Task.WhenAll(EventsToUpdateOnNextSync.Select(x => _googleCalendarApi.TryUpdateEventAsync(x, token)))).Any(x => !x))
				return false;

			EventsToDeleteOnNextSync.Clear();
			EventsToAddOnNextSync.Clear();
			EventsToUpdateOnNextSync.Clear();
			WeekEventCacheEntries.Clear();
			return true;
		}

		// Inspired by https://stackoverflow.com/a/2031831
		private static async Task<bool> CheckForInternetConnectionAsync(int timeoutMs = 10000, string? url = null)
		{
			try
			{
				url ??= CultureInfo.InstalledUICulture switch
				{
					{ Name: var n } when n.StartsWith("fa") => // Iran
						"http://www.aparat.com",
					{ Name: var n } when n.StartsWith("zh") => // China
						"http://www.baidu.com",
					_ =>
						"http://www.gstatic.com/generate_204",
				};

				var request = (HttpWebRequest)WebRequest.Create(url);
				request.KeepAlive = false;
				request.Timeout = timeoutMs;
				using var response = (HttpWebResponse)await request.GetResponseAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}