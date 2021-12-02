using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Colors = Google.Apis.Calendar.v3.Data.Colors;

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

			await UpdateColorDictionaries(token);

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
				Debug.Fail("");
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

		public async Task<Colors?> GetCalendarColors(CancellationToken token)
		{
			var service = _googleCalendarService;
			if (service is null)
				return default;

			try
			{
				return await service.Colors.Get().ExecuteAsync(token);
			}
			catch (Exception)
			{
				Debug.Fail("");
				return default;
			}
		}

		private async Task UpdateColorDictionaries(CancellationToken token)
		{
			try
			{
				if (ThemeHelper.CalendarBackgroundColorMap is null || ThemeHelper.CalendarForegroundColorMap is null ||
					ThemeHelper.EventBackgroundColorMap is null || ThemeHelper.EventForegroundColorMap is null)
				{
					var colorsResponse = await GetCalendarColors(token);
					if (colorsResponse is not null)
					{
						{
							var calendarBackgroundColorMap = new Dictionary<int, Color>();
							if (ThemeHelper.CalendarBackgroundColorMap is null)
							{
								foreach (var keypair in colorsResponse.Calendar)
								{
									calendarBackgroundColorMap.Add(int.Parse(keypair.Key), (Color)ColorConverter.ConvertFromString(keypair.Value.Background));
								}

								ThemeHelper.CalendarBackgroundColorMap = calendarBackgroundColorMap;
							}
						}
						{
							var calendarForegroundColorMap = new Dictionary<int, Color>();
							if (ThemeHelper.CalendarForegroundColorMap is null)
							{
								foreach (var keypair in colorsResponse.Calendar)
								{
									calendarForegroundColorMap.Add(int.Parse(keypair.Key), (Color)ColorConverter.ConvertFromString(keypair.Value.Foreground));
								}

								ThemeHelper.CalendarForegroundColorMap = calendarForegroundColorMap;
							}
						}
						{
							var eventBackgroundColorMap = new Dictionary<int, Color>();
							if (ThemeHelper.EventBackgroundColorMap is null)
							{
								foreach (var keypair in colorsResponse.Event__)
								{
									eventBackgroundColorMap.Add(int.Parse(keypair.Key), (Color)ColorConverter.ConvertFromString(keypair.Value.Background));
								}

								ThemeHelper.EventBackgroundColorMap = eventBackgroundColorMap;
							}
						}
						{
							var eventForegroundColorMap = new Dictionary<int, Color>();
							if (ThemeHelper.EventForegroundColorMap is null)
							{
								foreach (var keypair in colorsResponse.Event__)
								{
									eventForegroundColorMap.Add(int.Parse(keypair.Key), (Color)ColorConverter.ConvertFromString(keypair.Value.Foreground));
								}

								ThemeHelper.EventForegroundColorMap = eventForegroundColorMap;
							}
						}
					}
				}
			}
			catch
			{
				Debug.Fail("");
				// ignored
			}
		}

		public async Task<CalendarDataModel[]> GetCalendarsAsync(CancellationToken token)
		{
			var service = _googleCalendarService;
			if (service is null)
				return Array.Empty<CalendarDataModel>();

			try
			{
				var request = service.CalendarList.List();
				var calList = await request.ExecuteAsync(token);
				return Task.WhenAll(
					calList.Items
						.Select(x => x.Id)
						.Select(service.Calendars.Get)
						.Select(x => x.ExecuteAsync(token))
					).Result.Select(x => new CalendarDataModel(x, calList.Items.Single(y => y.Id == x.Id))).ToArray();
			}
			catch (Exception)
			{
				Debug.Fail("");
				return Array.Empty<CalendarDataModel>();
			}
		}

		private async Task<IList<IEventDataModel>?> GetCalendarEventsAsync(CalendarDataModel calendar, DateTime startTime, DateTime endTime, CancellationToken token)
		{
			var service = _googleCalendarService;
			if (service is null)
				return null;

			try
			{
				var eventsListRequest = service.Events.List(calendar.Id);
				eventsListRequest.TimeMin = startTime;
				eventsListRequest.TimeMax = endTime;
				eventsListRequest.ShowDeleted = false;
				eventsListRequest.SingleEvents = true;
				var singleEvents = await eventsListRequest.ExecuteAsync(token);
				var eventsFound = new List<IEventDataModel>();
				foreach (var singleEventsItem in singleEvents.Items)
				{
					eventsFound.Add(new CalendarEvent(calendar, singleEventsItem.Id)
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
						Description = singleEventsItem.Description,
						CustomEventColorId = int.TryParse(singleEventsItem.ColorId, out var colorId) ? colorId : default,
					});
				}

				return eventsFound;
			}
			catch (Exception)
			{
				Debug.Fail("");
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
				Debug.Fail("");
				return false;
			}
		}
	}
}