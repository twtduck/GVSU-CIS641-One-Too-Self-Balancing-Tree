using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class EventViewModel : Border, IDisposable
	{
		private bool _dragging;

		public EventViewModel()
		{
			MouseMove += OnMouseMove;
			MouseLeftButtonDown += OnLeftButtonDown;
			MouseLeftButtonUp += OnLeftButtonUp;
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