using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
		private readonly EventCommandFactory _eventCommandFactory;

		private readonly List<EventGridEventViewModel> _individualEvents = new List<EventGridEventViewModel>();

		public DateViewModel(DateTime dateTime, EventCommandFactory eventCommandFactory)
		{
			_dateTime = dateTime;
			_eventCommandFactory = eventCommandFactory;

			EventGridList = new ObservableCollection<Grid>();
		}

		private static Grid BuildEventGrid(
			IList<IEventDataModel> eventDataModels,
			DateTime dateMidnight,
			List<EventGridEventViewModel> individualEvents,
			EventCommandFactory eventCommandFactory
			)
		{
			const int blockSize = 12;
			var eventGrid = new Grid();
			for (int i = 0; i < 24 * 4; i++)
			{
				eventGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(blockSize) });
			}

			var eventDataModelsForToday = eventDataModels
				.Where(x => !x.AllDayEvent && !(x.StartTime >= dateMidnight.AddDays(1) || x.EndTime <= dateMidnight))
				.OrderBy(x => x.StartTime)
				.ThenBy(x => x.EndTime);
			var (blockInfos, columnsNeeded) = BuildBlockInfosForEvents(eventDataModelsForToday, dateMidnight);
			for (int i = 0; i < columnsNeeded; i++)
			{
				eventGrid.ColumnDefinitions.Add(new ColumnDefinition());
			}

			for (int i = 0; i < 24 * 4; i++)
			{
				if (i % 4 == 0)
				{
					var dividerGrid = new Grid();
					dividerGrid.SetValue(Grid.RowProperty, i);
					dividerGrid.SetValue(Grid.ColumnSpanProperty, columnsNeeded);
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

			foreach (var blockInfo in blockInfos)
			{
				var thisEventGridEventViewModel = new EventGridEventViewModel(eventCommandFactory, blockInfo.EventDataModel);
				thisEventGridEventViewModel.SetValue(Grid.RowProperty, blockInfo.StartBlock);
				thisEventGridEventViewModel.SetValue(Grid.RowSpanProperty, blockInfo.Duration);
				thisEventGridEventViewModel.SetValue(Grid.ColumnProperty, blockInfo.Column);
				thisEventGridEventViewModel.SetValue(Grid.ColumnSpanProperty, blockInfo.ColumnSpan);
				var eventColor = blockInfo.EventDataModel.Color;
				// Events in the past get a reduced saturation color
				if (DateTime.Now > blockInfo.EventDataModel.EndTime)
				{
					var hsb = eventColor.ToHsb();
					hsb = new Hsb(hsb.Hue, hsb.Saturation / 3, hsb.Brightness);
					eventColor = hsb.ToColor();
				}
				else
				{
					var hsb = eventColor.ToHsb();
					hsb = new Hsb(hsb.Hue, hsb.Saturation, hsb.Brightness / (1.5 - hsb.Saturation / 2));
					eventColor = hsb.ToColor();
				}
				thisEventGridEventViewModel.Background = new SolidColorBrush(eventColor);
				thisEventGridEventViewModel.BorderThickness = new Thickness(0);
				thisEventGridEventViewModel.CornerRadius = new CornerRadius(4);
				thisEventGridEventViewModel.Margin = new Thickness(2);
				thisEventGridEventViewModel.Child = new TextBlock { Text = blockInfo.EventDataModel.Title, Margin = new Thickness(2), TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(Colors.White)};
				eventGrid.Children.Add(thisEventGridEventViewModel);
				individualEvents.Add(thisEventGridEventViewModel);
			}

			// Draw the current time line
			if (dateMidnight == DateTime.Today)
			{
				var block = GetBlockIndexFromTime(DateTime.Now, dateMidnight);
				var percentThroughBlock = DateTime.Now.Minute % 15 / 15.0;
				var pixelOffset = (int)(percentThroughBlock * blockSize);
				var lineGrid = new Grid();
				lineGrid.SetValue(Grid.RowProperty, block);
				lineGrid.SetValue(Grid.ColumnSpanProperty, columnsNeeded);
				if (pixelOffset > 0)
					lineGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(pixelOffset) });
				lineGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				var line = new Border { BorderThickness = new Thickness(0, 2, 0, 0), BorderBrush = new SolidColorBrush(Colors.Red) };
				line.SetValue(Grid.RowProperty, pixelOffset > 0 ? 1 : 0);
				lineGrid.Children.Add(line);
				eventGrid.Children.Add(lineGrid);
			}

			return eventGrid;
		}

		private static (List<BlockInfo>, int columns) BuildBlockInfosForEvents(IEnumerable<IEventDataModel> eventDataModelsForToday, DateTime dateMidnight)
		{
			const int minDuration = 2;
			var blockInfos = new List<BlockInfo>();
			foreach (var eventDataModel in eventDataModelsForToday)
			{
				var startToday = eventDataModel.StartTime < dateMidnight ? dateMidnight : eventDataModel.StartTime;
				var endToday = eventDataModel.EndTime > dateMidnight.AddDays(1) ? dateMidnight.AddDays(1) : eventDataModel.EndTime;
				var firstBlock = GetBlockIndexFromTime(startToday, dateMidnight);
				var duration = GetBlockIndexFromTime(endToday, dateMidnight) - firstBlock;
				Debug.Assert(duration >= 0);
				if (duration < minDuration)
					duration = minDuration;
				blockInfos.Add(new BlockInfo(eventDataModel) { StartBlock = firstBlock, Duration = duration, Column = -1 });
			}

			var blockInfoGroups = new List<BlockInfoGroup>();
			foreach (var blockInfo in blockInfos)
			{
				var group = blockInfoGroups.FirstOrDefault(x => BlockInfoGroup.BlockInfoBelongsInGroup(blockInfo, x));
				if (group is null)
				{
					group = new BlockInfoGroup();
					blockInfoGroups.Add(group);
				}
				group.BlockInfos.Add(blockInfo);
			}

			foreach (var blockInfoGroup in blockInfoGroups)
			{
				foreach (var blockInfo in blockInfoGroup.BlockInfos)
				{
					var addedToColumn = false;
					for (var columnIndex = 0; columnIndex < blockInfoGroup.Columns.Count; columnIndex++)
					{
						var column = blockInfoGroup.Columns[columnIndex];
						if (column.Any(columnBlockInfo => BlockInfo.Collides(blockInfo, columnBlockInfo)))
							continue;

						column.Add(blockInfo);
						blockInfo.Column = columnIndex;
						addedToColumn = true;
						break;
					}

					if (!addedToColumn)
					{
						blockInfo.Column = blockInfoGroup.Columns.Count;
						blockInfoGroup.Columns.Add(new List<BlockInfo>(new[] { blockInfo }));
					}
				}
			}

			var columns = LeastCommonMultiple(blockInfoGroups.Select(x => x.Columns.Count));
			foreach (var blockInfoGroup in blockInfoGroups)
			{
				var multiplier = columns / blockInfoGroup.Columns.Count;
				foreach (var blockInfo in blockInfoGroup.BlockInfos)
				{
					blockInfo.Column *= multiplier;
					blockInfo.ColumnSpan = multiplier;
				}
			}

			return (blockInfoGroups.SelectMany(x => x.BlockInfos).OrderBy(x => x.StartBlock).ThenBy(x => x.Duration).ToList(), columns);

			static int LeastCommonMultiple(IEnumerable<int> numbers)
			{
				var max = numbers.Max();
				var min = numbers.Min();
				var lcm = max;
				while (lcm % min != 0)
					lcm += max;
				return lcm;
			}
		}

		private static int GetBlockIndexFromTime(DateTime time, DateTime todayMidnight)
		{
			if (time >= todayMidnight.AddDays(1))
				return 24 * 4;

			return time.Hour * 4 + time.Minute / 15;
		}

		private class BlockInfo
		{
			public IEventDataModel EventDataModel { get; }

			public BlockInfo(IEventDataModel eventDataModel)
			{
				EventDataModel = eventDataModel;
			}
			public int StartBlock { get; set; }
			public int Duration { get; set; }
			public int Column { get; set; }
			public int ColumnSpan { get; set; }
			public int OnePastLastBlock => StartBlock + Duration;

			public static bool Collides(BlockInfo blockInfo, BlockInfo columnBlockInfo)
			{
				return blockInfo.OnePastLastBlock > columnBlockInfo.StartBlock && blockInfo.StartBlock < columnBlockInfo.OnePastLastBlock;
			}
		}

		private class BlockInfoGroup
		{
			public List<BlockInfo> BlockInfos { get; } = new List<BlockInfo>();

			public List<List<BlockInfo>> Columns { get; } = new List<List<BlockInfo>>();

			public static bool BlockInfoBelongsInGroup(BlockInfo blockInfo, BlockInfoGroup blockInfoGroup) =>
				blockInfo.OnePastLastBlock > blockInfoGroup.BlockInfos.Min(x => x.StartBlock) && blockInfo.StartBlock < blockInfoGroup.BlockInfos.Max(x => x.OnePastLastBlock);
		}

		public string DayOfTheWeek => CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames[(int)_dateTime.DayOfWeek];

		public int DayNumber => _dateTime.Day;

		public ObservableCollection<Grid> EventGridList { get; }
		public double BorderOpacity { get; set; }

		public void UpdateFromEventsList(IList<IEventDataModel> events)
		{
			EventGridList.Clear();
			foreach (var individualEvent in _individualEvents)
			{
				individualEvent.Dispose();
			}
			_individualEvents.Clear();
			EventGridList.Add(BuildEventGrid(events, _dateTime, _individualEvents, _eventCommandFactory));
		}

		public void Dispose()
		{
			foreach (var individualEvent in _individualEvents)
				individualEvent.Dispose();
			_individualEvents.Clear();
		}
	}
}