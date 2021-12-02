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
					new[] { CalendarService.Scope.Calendar },
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
			var service = new CalendarService(new BaseClientService.Initializer
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
					eventsFound.Add(new CalendarEvent(googleCalendar, singleEventsItem.Id)
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
						Location = singleEventsItem.Location,
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

		public async Task<bool> DeleteEventAsync(EventSynchronizationInfo eventToDelete, CancellationToken token)
		{
			Debug.Assert(_googleCalendarService is not null);
			var service = _googleCalendarService;
			if (service is null)
				return false;

			try
			{
				var request = service.Events.Delete(eventToDelete.CalendarId, eventToDelete.EventId);
				await request.ExecuteAsync(token);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}