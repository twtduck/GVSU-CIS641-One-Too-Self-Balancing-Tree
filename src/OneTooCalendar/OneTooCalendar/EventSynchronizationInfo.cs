namespace OneTooCalendar
{
	public struct EventSynchronizationInfo
	{
		public EventSynchronizationInfo(string calendarId, string eventId)
		{
			CalendarId = calendarId;
			EventId = eventId;
		}

		public string CalendarId { get; }
		public string EventId { get; }
	}
}