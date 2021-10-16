using System;
using System.Collections.Generic;
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

    public class OneTooCalendarService : ICalendarService
    {
        public bool TryConnect()
        {
            var googleCalendarService = GetCalendarService();
            return false;
        }
        
        public IList<IEventViewModel> GetEventsForDate()
        {
            return new List<IEventViewModel>();
        }
        
        private CalendarService? _googleCalendarService;

        private CalendarService GetCalendarService()
        {
            if (_googleCalendarService != null)
                return _googleCalendarService;

            UserCredential credential;

            using (var stream =
                new FileStream("ApiKeys/credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { CalendarService.Scope.CalendarReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DuckCal",
            });

            _googleCalendarService = service;

            return service;
        }

        private async Task<Calendar[]> GetCalendarsAsync(CalendarService service)
        {
            var request = service.CalendarList.List();
            var calList = await request.ExecuteAsync();
            return Task.WhenAll(
                calList.Items
                   .Select(x => x.Id)
                   .Select(service.Calendars.Get)
                   .Select(x => x.ExecuteAsync())
                ).Result;
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