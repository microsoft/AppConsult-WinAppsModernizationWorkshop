using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;
using Waf.MusicManager.Presentation.Controls;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IPlaylistView))]
    public partial class PlaylistView : IPlaylistView
    {
        private readonly Lazy<PlaylistViewModel> viewModel;
        private readonly ListBoxDragDropHelper<PlaylistItem> listBoxDragDropHelper;

        public PlaylistView()
        {
            InitializeComponent();
            viewModel = new Lazy<PlaylistViewModel>(this.GetViewModel<PlaylistViewModel>);
            listBoxDragDropHelper = new ListBoxDragDropHelper<PlaylistItem>(playlistListBox, MoveItems, TryGetInsertItems, InsertItems);
            Loaded += FirstTimeLoadedHandler;
        }

        private PlaylistViewModel ViewModel => viewModel.Value;

        public void FocusSearchBox()
        {
            searchBox.Focus();
        }
        
        public void FocusSelectedItem()
        {
            var listBoxItem = (ListBoxItem)playlistListBox.ItemContainerGenerator.ContainerFromItem(playlistListBox.SelectedItem);
            listBoxItem?.Focus();
        }

        public void ScrollIntoView(PlaylistItem item)
        {
            playlistListBox.ScrollIntoView(item);
        }
        
        private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
        {
            Loaded -= FirstTimeLoadedHandler;
            ViewModel.PlaylistManager.PropertyChanged += PlaylistManagerPropertyChanged;
        }

        private void PlaylistManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlaylistManager.CurrentItem))
            {
                if (ViewModel.PlaylistManager.CurrentItem != null)
                {
                    playlistListBox.ScrollIntoView(ViewModel.PlaylistManager.CurrentItem);
                }
            }
        }
        
        private void ListBoxItemContextMenuOpening(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).ContextMenu.DataContext = ViewModel;
        }

        private void ListBoxItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {                
            ViewModel.PlaySelectedCommand.Execute(null);   
        }

        private void StatusBarButtonClick(object sender, RoutedEventArgs e)
        {
            menuPopup.Width = statusBarButton.ActualWidth;
            menuPopup.IsOpen = true;
        }

        private void MoveItems(int newIndex, IEnumerable<PlaylistItem> itemsToMove)
        {
            ViewModel.PlaylistManager.MoveItems(newIndex, itemsToMove);
        }

        private IEnumerable TryGetInsertItems(DragEventArgs e)
        {
            return e.Data.GetData(DataFormats.FileDrop) as IEnumerable ?? e.Data.GetData(typeof(MusicFile[])) as IEnumerable;
        }

        private void InsertItems(int index, IEnumerable itemsToInsert)
        {
            if (itemsToInsert is IEnumerable<string> fileNames)
            {
                ViewModel.InsertFilesAction(index, fileNames);
            }
            else if (itemsToInsert is IEnumerable<MusicFile> musicFiles)
            {
                ViewModel.InsertMusicFilesAction(index, musicFiles);
            }
        }
    }
}
