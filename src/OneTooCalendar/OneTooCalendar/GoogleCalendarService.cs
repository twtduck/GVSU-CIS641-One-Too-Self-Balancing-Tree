using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace OneTooCalendar
{
	public interface ICalendarService { }

	public class GoogleCalendarService : ICalendarService
	{
		public async Task<bool> InitializeServiceAsync(CancellationToken token)
		{
			var googleCalendarService = await GetCalendarService(token);
			if (googleCalendarService is null)
				return false;

			var calendars = await GetCalendarsAsync(token);
			return calendars.Any();
		}

		public async Task<IList<IEventDataModel>?> GetEventsForDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken token)
		{
			Debug.Assert(_googleCalendarService is not null);
			var calendarFeeds = await GetCalendarsAsync(token);
			var events = new List<IEventDataModel>();
			var getEventsResults = await Task.WhenAll(calendarFeeds.Select(x => GetCalendarEventsAsync(x, startDate, endDate, token)));
			if (getEventsResults.Any(x => x is null))
				return null;

			events.AddRange(getEventsResults.SelectMany(x => x!));


			return events;
		}

		private CalendarService? _googleCalendarService;

		private async Task<CalendarService?> GetCalendarService(CancellationToken token)
		{
			if (_googleCalendarService != null)
				return _googleCalendarService;

			UserCredential credential;

			try
			{
				await using var stream = new FileStream("ApiKeys/credentials.json", FileMode.Open, FileAccess.Read);
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					(await GoogleClientSecrets.FromStreamAsync(stream, token)).Secrets,
					new[] { CalendarService.Scope.CalendarReadonly },
					"user",
					token,
					new FileDataStore(credPath, true)
					);
			}
			catch (Exception)
			{
				return null;
			}

			// Create Google Calendar API service.
			var service = new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = "OneTooCalendar",
			});

			_googleCalendarService = service;

			return service;
		}

		private async Task<Calendar[]> GetCalendarsAsync(CancellationToken token)
		{
			var service = _googleCalendarService;
			if (service is null)
				return Array.Empty<Calendar>();

			try
			{
				var request = service.CalendarList.List();
				var calList = await request.ExecuteAsync(token);
				return Task.WhenAll(
					calList.Items
						.Select(x => x.Id)
						.Select(service.Calendars.Get)
						.Select(x => x.ExecuteAsync(token))
					).Result;
			}
			catch (Exception)
			{
				return Array.Empty<Calendar>();
			}
		}

		private async Task<IList<IEventDataModel>?> GetCalendarEventsAsync(Calendar googleCalendar, DateTime startTime, DateTime endTime, CancellationToken token)
		{
			var service = _googleCalendarService;
			if (service is null)
				return null;

			try
			{
				var eventsListRequest = service.Events.List(googleCalendar.Id);
				eventsListRequest.TimeMin = startTime;
				eventsListRequest.TimeMax = endTime;
				eventsListRequest.ShowDeleted = false;
				eventsListRequest.SingleEvents = true;
				var singleEvents = await eventsListRequest.ExecuteAsync(token);
				var eventsFound = new List<IEventDataModel>();
				foreach (var singleEventsItem in singleEvents.Items)
				{
					eventsFound.Add(new CalendarEvent(googleCalendar)
					{
						AllDayEvent = !singleEventsItem.Start.DateTime.HasValue,
						StartTime = singleEventsItem.Start.DateTime ??
							(DateTime.TryParse(singleEventsItem.Start.Date, out var startDateParsed)
								? startDateParsed
								: ErrorGettingDate()),
						EndTime = singleEventsItem.End.DateTime ??
							(DateTime.TryParse(singleEventsItem.Start.Date, out var endDateParsed)
								? endDateParsed
								: ErrorGettingDate()),
						Title = singleEventsItem.Summary,
						Location = singleEventsItem.Location
					});
				}

				return eventsFound;
			}
			catch (Exception)
			{
				return null;
			}

			DateTime ErrorGettingDate()
			{
				Debug.Fail(" Could not get DateTime for event");
				return startTime;
			}
		}


		// public async Task<List<IApiCalendar>> GetDuckCalsAsync()
		// {
		//     var service = GetCalendarService();
		//     var cals = await GetCalendarsAsync(service);
		//     return Task.WhenAll(cals.Select(BuildCalendarAsync)).Result.DropNullValues().ToList();
		// }
		//
		// private async Task<IApiCalendar?> BuildCalendarAsync(Calendar calendar)
		// {
		//     // TODO Show error or handle time zones
		//     if (TZConvert.GetTimeZoneInfo(calendar.TimeZone).BaseUtcOffset != TimeZoneInfo.Local.BaseUtcOffset)
		//         return default;
		//     
		//     var service = GetCalendarService();
		//     var eventsListRequest = service.Events.List(calendar.Id);
		//     eventsListRequest.ShowDeleted = false;
		//     eventsListRequest.SingleEvents = true;
		//     var singleEvents = await eventsListRequest.ExecuteAsync();
		//     eventsListRequest.SingleEvents = false;
		//     eventsListRequest.ShowDeleted = true;
		//     var recurringEvents = await eventsListRequest.ExecuteAsync();
		//     var cal = new DuckCal(calendar.Summary);
		//     // TODO Timezone may be not in the system
		//     foreach (var gCalEventSeries in singleEvents.Items)
		//     {
		//         BuildOrModifyEventSeries(gCalEventSeries, cal.EventSeries);
		//     }
		//     return cal;
		// }
		//
		// private ICalendarEventSeries? BuildOrModifyEventSeries(Event gCalEventSeries, List<ICalendarEventSeries> calendarEventSeries)
		// {
		//     return default;
		// }
		//
		// private IDuckCalEvent? BuildDuckCalEvent(Event gCalEvent)
		// {
		//     if (gCalEvent.Status == "cancelled")
		//         return null;
		//     if (gCalEvent.Start.DateTime.HasValue && gCalEvent.End.DateTime.HasValue)
		//     {
		//         return new TimeBoundDuckCalEvent(gCalEvent.Summary)
		//         {
		//             Start = new DateTimeOffset(gCalEvent.Start.DateTime!.Value),
		//             End = new DateTimeOffset(gCalEvent.End.DateTime!.Value)
		//         };
		//     }
		//     else if (gCalEvent.Start.DateTime.HasValue || gCalEvent.End.DateTime.HasValue)
		//     {
		//         //TODO Handle odd case where there is only one of start/end times
		//         return default;
		//     }
		//     else
		//     {
		//         // TODO Parse safely
		//         return new DateBoundDuckCalEvent(gCalEvent.Summary)
		//         {
		//             Start = DateTimeOffset.Parse(gCalEvent.Start.Date),
		//             End = DateTimeOffset.Parse(gCalEvent.End.Date)
		//         };
		//     }
		// }
	}
}