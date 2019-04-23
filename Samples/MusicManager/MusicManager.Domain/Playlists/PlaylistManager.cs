using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Playlists
{
    public class PlaylistManager : Model
    {
        private readonly IRandomService randomService;
        private readonly ObservableCollection<PlaylistItem> items;
        private readonly PlayedItemsStack<PlaylistItem> playedItemsStack;
        private bool isTotalDurationEstimated;
        private TimeSpan totalDuration;
        private PlaylistItem currentItem;
        private bool canPreviousItem;
        private bool canNextItem;
        private bool repeat;
        private bool shuffle;

        public PlaylistManager(int playedItemStackCapacity = 1000, IRandomService randomService = null)
        {
            this.randomService = randomService ?? new RandomService();
            items = new ObservableCollection<PlaylistItem>();
            Items = new ReadOnlyObservableList<PlaylistItem>(items);
            playedItemsStack = new PlayedItemsStack<PlaylistItem>(playedItemStackCapacity);

            items.CollectionChanged += ItemsCollectionChanged;
        }

        public IReadOnlyObservableList<PlaylistItem> Items { get; }

        public bool IsTotalDurationEstimated
        {
            get => isTotalDurationEstimated;
            private set => SetProperty(ref isTotalDurationEstimated, value);
        }

        public TimeSpan TotalDuration
        {
            get => totalDuration;
            private set => SetProperty(ref totalDuration, value);
        }

        public PlaylistItem CurrentItem
        {
            get => currentItem;
            set
            {
                if (currentItem != value)
                {
                    if (currentItem != null && Shuffle && items.Contains(currentItem))
                    {
                        playedItemsStack.RemoveAll(currentItem);
                        playedItemsStack.Add(currentItem);
                    }
                    currentItem = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool CanPreviousItem
        {
            get => canPreviousItem;
            private set => SetProperty(ref canPreviousItem, value);
        }

        public bool CanNextItem
        {
            get => canNextItem;
            private set => SetProperty(ref canNextItem, value);
        }

        public bool Repeat
        {
            get => repeat;
            set => SetProperty(ref repeat, value);
        }

        public bool Shuffle
        {
            get => shuffle;
            set
            {
                if (SetProperty(ref shuffle, value))
                {
                    playedItemsStack.Clear();
                }
            }
        }

        public void PreviousItem()
        {
            if (!CanPreviousItem) throw new InvalidOperationException("Call this method only if CanPreviousItem is true.");
            if (!Shuffle)
            {
                int currentItemIndex = items.IndexOf(CurrentItem);
                CurrentItem = Items[currentItemIndex - 1];
            }
            else
            {
                var oldItem = CurrentItem;
                CurrentItem = playedItemsStack.Pop();
                playedItemsStack.RemoveAll(oldItem);
                UpdateCanPreviousAndCanNextItem();
            }
        }

        public void NextItem()
        {
            if (!CanNextItem) throw new InvalidOperationException("Call this method only if CanNextItem is true.");
            int currentItemIndex = items.IndexOf(CurrentItem);
            int index;
            if (!Shuffle)
            {
                index = currentItemIndex + 1;
            }
            else
            {
                if (items.Count <= playedItemsStack.Count + 1)
                {
                    playedItemsStack.Clear();
                }
                index = randomService.NextRandomNumber(items.Count - playedItemsStack.Count - 2);
                for (int i = 0; i < items.Count; i++)
                {
                    if (CurrentItem == Items[i])
                    {
                        index++;
                    }
                    else if (playedItemsStack.Contains(Items[i]))
                    {
                        index++;
                    }
                    if (index <= i)
                    {
                        break;
                    }
                }
            }
            index = index < items.Count ? index : 0;
            CurrentItem = Items[index];
        }

        public void AddItems(IEnumerable<PlaylistItem> itemsToAdd)
        {
            InsertItems(items.Count, itemsToAdd);
        }

        public void AddAndReplaceItems(IEnumerable<PlaylistItem> itemsToAdd)
        {
            items.Clear();
            playedItemsStack.Clear();
            AddItems(itemsToAdd);
        }

        public void InsertItems(int index, IEnumerable<PlaylistItem> itemsToInsert)
        {
            foreach (var item in itemsToInsert)
            {
                items.Insert(index++, item);
            }
            UpdateCanPreviousAndCanNextItem();
        }

        public void RemoveItems(IEnumerable<PlaylistItem> itemsToRemove)
        {
            foreach (var item in itemsToRemove.ToArray())
            {
                items.Remove(item);
                playedItemsStack.RemoveAll(item);
            }
            UpdateCanPreviousAndCanNextItem();
        }

        public void ClearItems()
        {
            items.Clear();
            UpdateCanPreviousAndCanNextItem();
        }

        public void MoveItems(int newIndex, IEnumerable<PlaylistItem> itemsToMove)
        {
            int oldIndex = items.IndexOf(itemsToMove.First());
            if (oldIndex != newIndex)
            {
                if (newIndex < oldIndex)
                {
                    itemsToMove = itemsToMove.Reverse();
                }

                foreach (var item in itemsToMove)
                {
                    int currentIndex = items.IndexOf(item);
                    if (currentIndex != newIndex)
                    {
                        items.Move(currentIndex, newIndex);
                    }
                }

                UpdateCanPreviousAndCanNextItem();
            }
        }
        
        public void Reset()
        {
            playedItemsStack.Clear();
            UpdateCanPreviousAndCanNextItem();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (new[] { nameof(CurrentItem), nameof(Repeat), nameof(Shuffle) }.Contains(e.PropertyName))
            {
                UpdateCanPreviousAndCanNextItem();
            }
        }

        private void UpdateCanPreviousAndCanNextItem()
        {
            if (CurrentItem == null || !items.Any())
            {
                CanPreviousItem = false;
                CanNextItem = false;
                return;
            }

            if (!Shuffle)
            {
                int currentItemIndex = Math.Max(0, items.IndexOf(CurrentItem));
                CanPreviousItem = currentItemIndex > 0;
                CanNextItem = Repeat || currentItemIndex < items.Count - 1;
            }
            else
            {
                CanPreviousItem = playedItemsStack.Count > 0;
                CanNextItem = Repeat || playedItemsStack.Count < items.Count - 1;
            }
        }

        private void UpdateTotalDuration()
        {
            var loadedItems = items.Where(x => x.MusicFile.IsMetadataLoaded).OrderBy(x => x.MusicFile.Metadata.Duration).ToArray();

            double mean = StatisticsHelper.TruncatedMean(loadedItems.Take(200).Select(x => x.MusicFile.Metadata.Duration.TotalSeconds), 0.05);
            int notLoadedCount = items.Count - loadedItems.Count();

            IsTotalDurationEstimated = notLoadedCount > 0;
            TotalDuration = loadedItems.Select(x => x.MusicFile.Metadata.Duration).Aggregate(TimeSpan.Zero, (current, next) => current + next) 
                + TimeSpan.FromSeconds(notLoadedCount * mean);
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PlaylistItem newItem in e.NewItems)
                {
                    if (!newItem.MusicFile.IsMetadataLoaded)
                    {
                        newItem.MusicFile.PropertyChanged += MusicFilePropertyChanged;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (PlaylistItem oldItem in e.OldItems)
                {
                    if (!oldItem.MusicFile.IsMetadataLoaded)
                    {
                        oldItem.MusicFile.PropertyChanged -= MusicFilePropertyChanged;
                    }
                }
            }
            
            if (e.Action != NotifyCollectionChangedAction.Move)
            {
                UpdateTotalDuration();
            }
        }

        private void MusicFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicFile.IsMetadataLoaded))
            {
                ((MusicFile)sender).PropertyChanged -= MusicFilePropertyChanged;
                UpdateTotalDuration();
            }
        }
    }
}
