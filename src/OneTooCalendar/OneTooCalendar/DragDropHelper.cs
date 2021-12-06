using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace OneTooCalendar
{
	public interface IDragDropTarget
	{
		bool MouseIsOver { get; }
	}
	public class DragDropHelper
	{
		private static CalendarViewModel? _calendarViewModel;
		public static List<EventGridEventViewModel> SourceList { get; } = new List<EventGridEventViewModel>();

		public static void OnEventGridEventMouseDown(EventGridEventViewModel eventGridEventViewModel, int startBlock, MouseButtonEventArgs mouseButtonEventArgs)
		{
			if (_calendarViewModel is null)
			{
				Debug.Fail("_calendarViewModel is null");
				return;
			}

			
		}

		public static void InitializeCalendar(CalendarViewModel calendarViewModel)
		{
			Debug.Assert(_calendarViewModel is null, "Reinitializing calendar in DragDropHelper");
			_calendarViewModel = calendarViewModel;
		}
	}
}