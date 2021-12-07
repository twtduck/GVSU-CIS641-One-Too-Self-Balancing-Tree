using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OneTooCalendar
{
	public class BacklogViewModel : ViewModelBase
	{
		private readonly ApplyEditsAndRefresh _applyEditsAndRefresh;
		private bool _isDropTarget;

		public BacklogViewModel(ApplyEditsAndRefresh applyEditsAndRefresh)
		{
			_applyEditsAndRefresh = applyEditsAndRefresh;
			AddBacklogEventCommand = new OneTooCalendarCommand(_ => AddBacklogEvent());
		}

		private void AddBacklogEvent()
		{
			
		}

		public ObservableCollection<BacklogEventViewModel> BacklogEvents { get; } = new ObservableCollection<BacklogEventViewModel>();

		public Visibility EventsListVisibility => BacklogEvents.Any() ? Visibility.Visible : Visibility.Collapsed;

		public Visibility EmptyEventsListLabelVisibility => BacklogEvents.Any() ? Visibility.Collapsed : Visibility.Visible;

		public SolidColorBrush BackgroundColor => _isDropTarget ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE0E0E0")) : new SolidColorBrush(Colors.White);

		public ICommand AddBacklogEventCommand { get; }

		public void OnDragOver(DragEventArgs args)
		{
			if (args.Data.GetDataPresent(typeof(EventGridEventViewModel)))
			{
				args.Effects = DragDropEffects.Move;
				_isDropTarget = true;
			}
			else
			{
				args.Effects = DragDropEffects.None;
				_isDropTarget = false;
			}
			args.Handled = true;
		}

		public void OnDragLeave(DragEventArgs args)
		{
			_isDropTarget = false;
			args.Handled = true;
		}

		public void OnDrop(DragEventArgs args)
		{
			if (args.Data.GetDataPresent(typeof(EventGridEventViewModel)))
			{
				var droppedEventGridEvent = (EventGridEventViewModel)args.Data.GetData(typeof(EventGridEventViewModel));
				_applyEditsAndRefresh.Invoke(droppedEventGridEvent.EventDataModel, EventMovementType.ToBacklog, () => { });
				OnPropertyChanged(nameof(EventsListVisibility));
				OnPropertyChanged(nameof(EmptyEventsListLabelVisibility));
			}
			args.Handled = true;
		}
	}
}