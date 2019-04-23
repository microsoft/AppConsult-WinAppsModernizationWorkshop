using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class PlaylistViewModel : ViewModel<IPlaylistView>
    {
        private PlaylistManager playlistManager;
        private PlaylistItem selectedPlaylistItem;
        private ICommand playSelectedCommand;
        private ICommand removeSelectedCommand;
        private ICommand showMusicPropertiesCommand;
        private ICommand openListCommand;
        private ICommand saveListCommand;
        private ICommand clearListCommand;
        private string searchText;
        
        [ImportingConstructor]
        public PlaylistViewModel(IPlaylistView view) : base(view)
        {
            SelectedPlaylistItems = new ObservableCollection<PlaylistItem>();
            SearchNextCommand = new DelegateCommand(SearchNext);
            SearchPreviousCommand = new DelegateCommand(SearchPrevious);
            ClearSearchCommand = new DelegateCommand(ClearSearch);
        }

        public PlaylistManager PlaylistManager
        {
            get => playlistManager;
            set => SetProperty(ref playlistManager, value);
        }

        public PlaylistItem SelectedPlaylistItem
        {
            get => selectedPlaylistItem;
            set => SetProperty(ref selectedPlaylistItem, value);
        }

        public IList<PlaylistItem> SelectedPlaylistItems { get; }

        public ICommand PlaySelectedCommand
        {
            get => playSelectedCommand;
            set => SetProperty(ref playSelectedCommand, value);
        }

        public ICommand RemoveSelectedCommand
        {
            get => removeSelectedCommand;
            set => SetProperty(ref removeSelectedCommand, value);
        }

        public ICommand ShowMusicPropertiesCommand
        {
            get => showMusicPropertiesCommand;
            set => SetProperty(ref showMusicPropertiesCommand, value);
        }

        public ICommand OpenListCommand
        {
            get => openListCommand;
            set => SetProperty(ref openListCommand, value);
        }

        public ICommand SaveListCommand
        {
            get => saveListCommand;
            set => SetProperty(ref saveListCommand, value);
        }

        public ICommand ClearListCommand
        {
            get => clearListCommand;
            set => SetProperty(ref clearListCommand, value);
        }

        public Action<int, IEnumerable<string>> InsertFilesAction { get; set; }

        public Action<int, IEnumerable<MusicFile>> InsertMusicFilesAction { get; set; }

        public ICommand SearchNextCommand { get; }

        public ICommand SearchPreviousCommand { get; }

        public ICommand ClearSearchCommand { get; }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    SearchTextCore(SearchMode.Default);
                }
            }
        }

        private void SearchTextCore(SearchMode searchMode)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                IEnumerable<PlaylistItem> itemsToSearch;
                if (SelectedPlaylistItem != null)
                {
                    var index = IndexOf(PlaylistManager.Items, SelectedPlaylistItem);
                    if (searchMode == SearchMode.Next)
                    {
                        index++;  // Skip the current item so that the next one will be found.
                    }
                    itemsToSearch = PlaylistManager.Items.Skip(index).Concat(PlaylistManager.Items.Take(index));
                }
                else
                {
                    itemsToSearch = PlaylistManager.Items;
                }

                if (searchMode == SearchMode.Previous)
                {
                    itemsToSearch = itemsToSearch.Reverse();
                }
                var foundItem = itemsToSearch.FirstOrDefault(x => IsContained(x.MusicFile, SearchText));
                if (foundItem != null)
                {
                    SelectedPlaylistItem = foundItem;
                    ViewCore.ScrollIntoView(foundItem);
                }
            }
            ViewCore.FocusSearchBox();
        }

        public void FocusSelectedItem()
        {
            ViewCore.FocusSelectedItem();
        }

        public void ScrollIntoView()
        {
            ViewCore.ScrollIntoView(PlaylistManager.CurrentItem);
        }

        private void SearchNext()
        {
            SearchTextCore(SearchMode.Next);
        }

        private void SearchPrevious()
        {
            SearchTextCore(SearchMode.Previous);
        }

        private void ClearSearch()
        {
            SearchText = "";
        }

        private static bool IsContained(MusicFile musicFile, string searchText)
        {
            return MusicTitleHelper.GetTitleText(musicFile.FileName, musicFile.IsMetadataLoaded ? musicFile.Metadata.Artists : null, musicFile.IsMetadataLoaded ? musicFile.Metadata.Title : null)
                    .IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                || musicFile.IsMetadataLoaded
                    && (musicFile.Metadata.Artists.Any(y => y.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0));
        }

        private static int IndexOf<T>(IReadOnlyList<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (Equals(list[i], item))
                {
                    return i;
                }
            }
            return -1;
        }


        private enum SearchMode
        {
            Default,
            Next,
            Previous
        }
    }
}
