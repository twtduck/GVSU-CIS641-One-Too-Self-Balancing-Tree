using System;
using System.Diagnostics;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class EventDetailsViewModel : ViewModelBase
	{
		public EventDetailsViewModel(IEventDataModel eventDataModel)
		{
			CloseCommand = new OneTooCalendarCommand(_ => Close());
			SaveCommand = new OneTooCalendarCommand(_ => Save());
			CancelCommand = new OneTooCalendarCommand(_ => Cancel());
			Title = eventDataModel.Title;
			StartDate = eventDataModel.StartTime;
			StartTime = eventDataModel.StartTime;
			EndDate = eventDataModel.EndTime;
			EndTime = eventDataModel.EndTime;
		}

		public ICommand SaveCommand { get; set; }

		public ICommand CancelCommand { get; set; }

		public Action? RestoreAction { get; set; }

		public ICommand CloseCommand { get; }

		public string Title { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndDate { get; set; }

		public DateTime EndTime { get; set; }

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