using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class EventDetailsViewModel : ViewModelBase
	{
		private readonly IEventDataModel _eventDataModel;
		private readonly ICalendarDataModel[] _googleCalendarInfos;
		private readonly IEventsApi _eventsApi;
		private readonly Action<Action?> _refreshCurrentCalendarAndThenRunAction;
		private string _selectedCalendarName;
		private readonly ColorViewModel _calendarDefaultColor;

		public EventDetailsViewModel(
			IEventDataModel eventDataModel,
			ICalendarDataModel[] googleCalendarInfos,
			IEventsApi eventsApi,
			Action<Action?> refreshCurrentCalendarAndThenRunAction
			)
		{
			_eventDataModel = eventDataModel;
			_googleCalendarInfos = googleCalendarInfos;
			_eventsApi = eventsApi;
			_refreshCurrentCalendarAndThenRunAction = refreshCurrentCalendarAndThenRunAction;
			CloseCommand = new OneTooCalendarCommand(_ => Close());
			SaveCommand = new OneTooCalendarCommand(_ => Save());
			CancelCommand = new OneTooCalendarCommand(_ => Cancel());

			Title = eventDataModel.Title;
			StartDate = eventDataModel.StartTime;
			StartTime = eventDataModel.StartTime;
			EndDate = eventDataModel.EndTime;
			EndTime = eventDataModel.EndTime;
			Location = eventDataModel.Location;
			Description = eventDataModel.Description;
			foreach (var googleCalendarInfo in googleCalendarInfos)
			{
				CalendarNames.Add(googleCalendarInfo.Name);
			}

			var calendarDataModel = googleCalendarInfos.First(x => x.Id == eventDataModel.Calendar.Id);
			_selectedCalendarName = calendarDataModel.Name;
			_calendarDefaultColor = new ColorViewModel(ThemeHelper.GetCalendarBackgroundColor(calendarDataModel), "Calendar default", 0);
			Colors.Add(_calendarDefaultColor);
			if (ThemeHelper.EventBackgroundColorMap is not null)
			{
				foreach (var color in ThemeHelper.EventBackgroundColorMap)
				{
					Colors.Add(
						new ColorViewModel(
							color.Value,
							ThemeHelper.EventBackgroundColorNames.TryGetValue(color.Key, out var colorName)
								? colorName
								: HandleMissingColorName(),
							color.Key
							)
						);
				}
			}

			SelectedColor = eventDataModel.CustomEventColorId.HasValue && Colors.ElementAtOrDefault(eventDataModel.CustomEventColorId.Value) is { } colorViewModel
				? colorViewModel
				: Colors.First();
		}

		private string HandleMissingColorName()
		{
			Debug.Fail("Missing color name");
			return "Missing color name";
		}

		private DateTime StartDateTime => StartDate.Date + StartTime.TimeOfDay;
		private DateTime EndDateTime => EndDate.Date + EndTime.TimeOfDay;

		public bool HasEdits =>
			Title != _eventDataModel.Title ||
			StartDateTime != _eventDataModel.StartTime ||
			EndDateTime != _eventDataModel.EndTime ||
			Location != _eventDataModel.Location ||
			Description != _eventDataModel.Description ||
			SelectedColor.CustomEventColorId != _eventDataModel.CustomEventColorId ||
			SelectedCalendarName != _eventDataModel.Calendar.Name; // TODO: This is not the best way to compare calendars

		public string SelectedCalendarName
		{
			get => _selectedCalendarName;
			set
			{
				_selectedCalendarName = value;
				_calendarDefaultColor.Color = ThemeHelper.GetCalendarBackgroundColor(_googleCalendarInfos.First(x => x.Name == value));
			}
		}

		public string? Description { get; set; }

		public string DisplayDescription
		{
			get => Description?.Replace(Environment.NewLine, "<br>") ?? string.Empty;
			set => Description = string.IsNullOrEmpty(value.Replace("<br>", Environment.NewLine)) && Description is null
				? null
				: value.Replace("<br>", Environment.NewLine);
		}

		public string Location { get; set; }

		public ICommand SaveCommand { get; set; }

		public ICommand CancelCommand { get; set; }

		public Action? RestoreAction { get; set; }

		public ICommand CloseCommand { get; }

		public string Title { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndDate { get; set; }

		public DateTime EndTime { get; set; }

		public ObservableCollection<string> CalendarNames { get; } = new ObservableCollection<string>();

		public ObservableCollection<ColorViewModel> Colors { get; } = new ObservableCollection<ColorViewModel>();

		public ColorViewModel SelectedColor { get; set; }

		public void Close()
		{
			Debug.Assert(RestoreAction is not null);
			if (HasEdits)
			{
				var result = MessageBox.Show("Save changes?", "OneTooCalendar", MessageBoxButton.YesNoCancel);
				if (result == MessageBoxResult.Yes)
					Save();
				else if (result == MessageBoxResult.Cancel)
					return;
			}
			RestoreAction?.Invoke();
		}

		public void Cancel()
		{
			Debug.Assert(RestoreAction is not null);
			RestoreAction?.Invoke();
		}

		public void Save()
		{
			Debug.Assert(RestoreAction is not null);
			if (HasEdits)
			{
				_eventDataModel.Title = Title;
				_eventDataModel.StartTime = StartDateTime;
				_eventDataModel.EndTime = EndDateTime;
				_eventDataModel.Location = Location;
				_eventDataModel.Description = Description;
				_eventDataModel.CustomEventColorId = SelectedColor.CustomEventColorId;
				_eventsApi.UpdateEvent(_eventDataModel);
			}
			if (_eventDataModel.Calendar.Name != SelectedCalendarName)
			{
				var newCalendarDataModel = _googleCalendarInfos.First(x => x.Name == SelectedCalendarName);
				var newEvent = new GoogleCalendarEventDataModel(newCalendarDataModel)
				{
					Title = _eventDataModel.Title,
					StartTime = _eventDataModel.StartTime,
					EndTime = _eventDataModel.EndTime,
					Location = _eventDataModel.Location,
					Description = _eventDataModel.Description,
					CustomEventColorId = _eventDataModel.CustomEventColorId
				};
				_eventsApi.AddEvent(newEvent);
				_eventsApi.DeleteEvent(_eventDataModel);
			}

			_refreshCurrentCalendarAndThenRunAction(RestoreAction);
		}
	}
}