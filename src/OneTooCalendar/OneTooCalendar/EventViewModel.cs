using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class EventGridEventViewModel : Border, IDisposable
	{
		public IEventDataModel EventDataModel { get; }

		public EventGridEventViewModel(EventCommandFactory eventCommandFactory, IEventDataModel eventDataModel)
		{
			EventDataModel = eventDataModel;
			MouseLeftButtonDown += OnMouseLeftButtonDown;
			ContextMenu = new ContextMenu();
			ContextMenu.Items.Add(new MenuItem { Header = EventCommandFactory.CreateDeleteEventControlContent(), Command = eventCommandFactory.CreateDeleteEventCommand(EventDataModel) });
			ContextMenu.Items.Add(new MenuItem { Header = EventCommandFactory.CreateEditEventControlContent(), Command = eventCommandFactory.CreateEditEventCommand(EventDataModel) });
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
		}

		public void Dispose()
		{
			MouseLeftButtonDown -= OnMouseLeftButtonDown;
		}
	}
}