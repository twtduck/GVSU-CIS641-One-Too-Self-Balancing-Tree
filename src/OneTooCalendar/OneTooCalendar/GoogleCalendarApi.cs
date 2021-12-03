using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Colors = Google.Apis.Calendar.v3.Data.Colors;

namespace OneTooCalendar
{
	public class GoogleCalendarApi : ICalendarSource
	{
		public async Task<bool> InitializeServiceAsync(CancellationToken token)
		{
			var googleCalendarService = await GetCalendarService(token);
			if (googleCalendarService is null)
				return false;

			await UpdateColorDictionaries(token);

			var calendars = await GetCalendarsAsync(token);
			return calendars?.Any() == true;
		}

		public async Task<IList<IEventDataModel>?> GetEventsForDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken token)
		{
			Debug.Assert(_googleCalendarService is not null);
			var calendarFeeds = await GetCalendarsAsync(token);
			if (calendarFeeds is null)
				return null;

			var events = new List<IEventDataModel>();
			var getEventsResults = await Task.WhenAll(
				calendarFeeds.Select(x => GetCalendarEventsAsync(x, startDate, endDate, token))
				);
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
				const string credPath = "token.json";
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
			var service = new CalendarService(
				new BaseClientService.Initializer
				{
					HttpClientInitializer = credential,
					ApplicationName = "OneTooCalendar",
				}
				);

			_googleCalendarService = service;

			return service;
		}

		private async Task<Colors?> GetCalendarColors(CancellationToken token)
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

		[SuppressMessage("ReSharper", "CognitiveComplexity")]
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

		public async Task<ICalendarDataModel[]?> GetCalendarsAsync(CancellationToken token)
		{
			var service = _googleCalendarService;
			if (service is null)
				return default;

			try
			{
				var request = service.CalendarList.List();
				var calList = await request.ExecuteAsync(token);
				return Task.WhenAll(
					calList.Items
						.Select(x => x.Id)
						.Select(service.Calendars.Get)
						.Select(x => x.ExecuteAsync(token))
					).Result.Select(x => new GoogleCalendarCalendarDataModel(x, calList.Items.Single(y => y.Id == x.Id))).ToArray();
			}
			catch (Exception)
			{
				Debug.Fail("");
				return default;
			}
		}

		private async Task<IList<IEventDataModel>?> GetCalendarEventsAsync(ICalendarDataModel calendar, DateTime startTime, DateTime endTime, CancellationToken token)
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
				foreach (var singleEventsItem in singleEvents.Items.Where(x => x is not null))
				{
					eventsFound.Add(
						new GoogleCalendarEventDataModel(calendar)
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
							SourceEvent = singleEventsItem,
							EventIdIsTemporary = false,
							EventId = singleEventsItem.Id
						}
						);
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

		public async Task<bool> TryUpdateEventAsync(IEventDataModel eventDataModel, CancellationToken token)
		{
			Debug.Assert(!eventDataModel.EventIdIsTemporary);
			var service = _googleCalendarService;
			if (service is null)
			{
				Debug.Fail("");
				return false;
			}

			try
			{
				var sourceEvent = ((GoogleCalendarEventDataModel)eventDataModel).SourceEvent;
				sourceEvent.Summary = eventDataModel.Title;
				sourceEvent.Location = eventDataModel.Location;
				sourceEvent.Description = eventDataModel.Description;
				sourceEvent.Start.DateTime = eventDataModel.StartTime;
				sourceEvent.End.DateTime = eventDataModel.EndTime;
				sourceEvent.ColorId = eventDataModel.CustomEventColorId?.ToString();
				var request = service.Events.Update(sourceEvent, eventDataModel.Calendar.Id, eventDataModel.EventId);
				await request.ExecuteAsync(token);
				return true;
			}
			catch (Exception)
			{
				Debug.Fail("");
				return false;
			}
		}

		public async Task<bool> TryDeleteEventAsync(IEventDataModel eventDataModel, CancellationToken token)
		{
			Debug.Assert(!eventDataModel.EventIdIsTemporary);
			Debug.Assert(_googleCalendarService is not null);
			var service = _googleCalendarService;
			if (service is null)
				return false;

			try
			{
				var request = service.Events.Delete(eventDataModel.Calendar.Id, eventDataModel.EventId);
				await request.ExecuteAsync(token);
				return true;
			}
			catch (Exception)
			{
				Debug.Fail("");
				return false;
			}
		}

		public async Task<bool> TryAddEventAsync(IEventDataModel eventDataModel, CancellationToken token)
		{
			Debug.Assert(_googleCalendarService is not null);
			var service = _googleCalendarService;
			if (service is null)
				return false;

			try
			{
				Debug.Assert(eventDataModel.EventIdIsTemporary);
				var request = service.Events.Insert(
					new Event
					{
						Summary = eventDataModel.Title,
						Location = eventDataModel.Location,
						Description = eventDataModel.Description,
						Start = new EventDateTime
						{
							DateTime = eventDataModel.StartTime,
						},
						End = new EventDateTime
						{
							DateTime = eventDataModel.EndTime,
						},
						ColorId = eventDataModel.CustomEventColorId?.ToString(),
					},
					eventDataModel.Calendar.Id
					);
				eventDataModel.EventId = (await request.ExecuteAsync(token)).Id;
				eventDataModel.EventIdIsTemporary = false;
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