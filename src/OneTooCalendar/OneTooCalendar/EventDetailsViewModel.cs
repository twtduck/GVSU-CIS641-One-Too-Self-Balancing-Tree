using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace OneTooCalendar
{
	public class EventDetailsViewModel : ViewModelBase
	{
		public EventDetailsViewModel(IEventDataModel eventDataModel, ICalendarDataModel[] googleCalendarInfos)
		{
			CloseCommand = new OneTooCalendarCommand(_ => Close());
			SaveCommand = new OneTooCalendarCommand(_ => Save());
			CancelCommand = new OneTooCalendarCommand(_ => Cancel());
			Title = eventDataModel.Title;
			StartDate = eventDataModel.StartTime;
			StartTime = eventDataModel.StartTime;
			EndDate = eventDataModel.EndTime;
			EndTime = eventDataModel.EndTime;
			Location = eventDataModel.Location;
			Description = PrepareForDisplay(eventDataModel.Description);
			foreach (var googleCalendarInfo in googleCalendarInfos)
			{
				CalendarNames.Add(googleCalendarInfo.Name);
			}

			var calendarDataModel = googleCalendarInfos.First(x => x.Id == eventDataModel.SyncInfo.CalendarId);
			SelectedCalendarName = calendarDataModel.Name;
			Colors.Add(new ColorViewModel(ThemeHelper.GetCalendarBackgroundColor(calendarDataModel), "Calendar default"));
			if (ThemeHelper.EventBackgroundColorMap is not null)
			{
				foreach (var color in ThemeHelper.EventBackgroundColorMap)
				{
					Colors.Add(
						new ColorViewModel(
							color.Value,
							ThemeHelper.EventBackgroundColorNames.TryGetValue(color.Key, out var colorName)
								? colorName
								: HandleMissingColorName()
							)
						);
				}
			}

			SelectedColor = eventDataModel.CustomEventColorId.HasValue && Colors.ElementAtOrDefault(eventDataModel.CustomEventColorId.Value) is { } colorViewModel
				? colorViewModel
				: Colors.First();
		}

		private string PrepareForDisplay(string? description)
		{
			return description?.Replace("<br>", Environment.NewLine) ?? string.Empty;
		}

		private string HandleMissingColorName()
		{
			Debug.Fail("Missing color name");
			return "Missing color name";
		}

		public string SelectedCalendarName { get; set; }

		public string Description { get; set; }

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
			RestoreAction?.Invoke();
		}
	}
}