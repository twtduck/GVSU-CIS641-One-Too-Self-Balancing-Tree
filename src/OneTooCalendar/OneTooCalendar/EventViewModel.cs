using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class EventGridEventViewModel : Border, IDisposable
	{
		public EventSynchronizationInfo EventSynchronizationInfo { get; }
		private bool _dragging;

		public EventGridEventViewModel(EventCommandFactory eventCommandFactory, EventSynchronizationInfo eventSynchronizationInfo)
		{
			EventSynchronizationInfo = eventSynchronizationInfo;
			MouseMove += OnMouseMove;
			MouseLeftButtonDown += OnLeftButtonDown;
			MouseLeftButtonUp += OnLeftButtonUp;
			ContextMenu = new ContextMenu();
			ContextMenu.Items.Add(new MenuItem { Header = EventCommandFactory.CreateDeleteEventControlContent(), Command = eventCommandFactory.DeleteEventCommand(EventSynchronizationInfo) });
			ContextMenu.Items.Add(new MenuItem { Header = EventCommandFactory.CreateEditEventControlContent(), Command = eventCommandFactory.CreateEditEventCommand(EventSynchronizationInfo) });
		}

		private void OnLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_dragging = true;
		}

		private void OnLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_dragging = false;
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
            
		}

		public void Dispose()
		{
			MouseMove -= OnMouseMove;
			MouseLeftButtonDown -= OnLeftButtonDown;
			MouseLeftButtonUp -= OnLeftButtonUp;
		}
	}
}