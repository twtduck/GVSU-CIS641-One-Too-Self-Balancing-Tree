using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;

namespace OneTooCalendar
{
	public class DateViewModel : ViewModelBase, IDisposable
	{
		private readonly DateTime _dateTime;

		private readonly List<EventViewModel> IndividualEvents = new();

		public DateViewModel(DateTime dateTime)
		{
			_dateTime = dateTime;

			EventGridList = new ObservableCollection<Grid>();
		}

		private static Grid BuildEventGrid(IList<IEventDataModel> eventDataModels, DateTime dateMidnight, List<EventViewModel> individualEvents)
		{
			const int blockSize = 12;
			var eventGrid = new Grid();
			for (int i = 0; i < 24 * 4; i++)
			{
				eventGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(blockSize) });
			}

			for (int i = 0; i < 24 * 4; i++)
			{
				if (i % 4 == 0)
				{
					var dividerGrid = new Grid();
					dividerGrid.SetValue(Grid.RowProperty, i);
					dividerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
					dividerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(blockSize - 1) });
					var border = new Border
					{
						BorderBrush = ThemeHelper.CalendarDivider,
						BorderThickness = new Thickness(0, 1, 0, 0)
					};
					border.SetValue(Grid.RowProperty, 0);
					dividerGrid.Children.Add(border);
					eventGrid.Children.Add(dividerGrid);
				}
			}

			foreach (var eventDataModel in eventDataModels.OrderBy(x => x.StartTime))
			{
				if (eventDataModel.AllDayEvent)
					continue;

				if (eventDataModel.StartTime >= dateMidnight.AddDays(1) || eventDataModel.EndTime <= dateMidnight)
					continue;

				var firstBlock = GetBlockIndexFromTime(eventDataModel.StartTime);
				var duration = GetBlockIndexFromTime(eventDataModel.EndTime) - firstBlock;
				if (eventDataModel.EndTime.Date > dateMidnight)
					duration += 24 * 4;
				if (duration < 1)
					duration = 1;
				if (duration > 24 * 4 - firstBlock)
					duration = 24 * 4 - firstBlock;

				var thisEventGrid = new EventViewModel();
				thisEventGrid.SetValue(Grid.RowProperty, firstBlock);
				thisEventGrid.SetValue(Grid.RowSpanProperty, duration);
				var eventColor = eventDataModel.Color;
				// Events in the past get a reduced saturation color
				if (DateTime.Now > eventDataModel.EndTime)
				{
					var hsb = eventColor.ToHsb();
					hsb = new Hsb(hsb.Hue, hsb.Saturation / 3, hsb.Brightness);
					eventColor = hsb.ToColor();
				}
				thisEventGrid.Background = new SolidColorBrush(eventColor);
				thisEventGrid.BorderThickness = new Thickness(0);
				thisEventGrid.CornerRadius = new CornerRadius(4);
				thisEventGrid.Margin = new Thickness(2);
				thisEventGrid.Child = new TextBlock() { Text = eventDataModel.Title, Margin = new Thickness(4), TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(System.Windows.Media.Colors.White)};
				eventGrid.Children.Add(thisEventGrid);
				individualEvents.Add(thisEventGrid);
			}

			// Draw the current time line
			if (dateMidnight == DateTime.Today)
			{
				var block = GetBlockIndexFromTime(DateTime.Now);
				var percentThroughBlock = (DateTime.Now.Minute % 15) / 15.0;
				var pixelOffset = (int)(percentThroughBlock * blockSize);
				var lineGrid = new Grid();
				lineGrid.SetValue(Grid.RowProperty, block);
				if (pixelOffset > 0)
					lineGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(pixelOffset) });
				lineGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
				var line = new Border() { BorderThickness = new Thickness(0, 2, 0, 0), BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.Red) };
				line.SetValue(Grid.RowProperty, pixelOffset > 0 ? 1 : 0);
				lineGrid.Children.Add(line);
				eventGrid.Children.Add(lineGrid);
			}

			return eventGrid;

			static int GetBlockIndexFromTime(DateTime time)
			{
				return time.Hour * 4 + time.Minute / 15;
			}
		}

		public string DayOfTheWeek => CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames[(int)_dateTime.DayOfWeek];

		public int DayNumber => _dateTime.Day;

		public ObservableCollection<Grid> EventGridList { get; }
		public double BorderOpacity { get; set; }

		public void UpdateFromEventsList(IList<IEventDataModel> events)
		{
			EventGridList.Clear();
			foreach (var individualEvent in IndividualEvents)
			{
				individualEvent.Dispose();
			}
			IndividualEvents.Clear();
			EventGridList.Add(BuildEventGrid(events, _dateTime, IndividualEvents));
		}

		public void Dispose()
		{
			foreach (var individualEvent in IndividualEvents)
				individualEvent.Dispose();
			IndividualEvents.Clear();
		}
	}
}