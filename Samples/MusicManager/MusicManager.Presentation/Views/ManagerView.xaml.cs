using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Waf.Presentation.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IManagerView))]
    public partial class ManagerView : IManagerView
    {
        private readonly Lazy<ManagerViewModel> viewModel;
        private readonly List<DataGridColumn> autoColumns;
        
        public ManagerView()
        {
            InitializeComponent();
            viewModel = new Lazy<ManagerViewModel>(this.GetViewModel<ManagerViewModel>);
            autoColumns = new List<DataGridColumn>()
            {
                ratingColumn,
                genreColumn,
                yearColumn,
                albumColumn,
                trackNoColumn
            };
            autoColumns.ForEach(x => x.Visibility = Visibility.Collapsed);

            Loaded += LoadedHandler;
            musicFilesGrid.Sorting += MusicFilesGridSorting;
            DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn)).AddValueChanged(this.titleColumn, TitleColumnWidthChanged);
        }

        private ManagerViewModel ViewModel => viewModel.Value;

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            FocusMusicFilesGrid();
        }

        private void DirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.FolderBrowser.UserPath = ViewModel.FolderBrowser.CurrentPath;  // The old UserPath might still be invalid.
            ViewModel.UpdateSubDirectoriesCommand.Execute(null);
            folderBrowserPopup.Width = directoryButton.ActualWidth + searchButton.ActualWidth + 1;
            folderBrowserPopup.IsOpen = true;
            userPathBox.Select(int.MaxValue, 0);
        }

        private void FolderBrowserPopupClosed(object sender, EventArgs e)
        {
            FocusMusicFilesGrid();
        }

        private void LoadRecursiveClick(object sender, RoutedEventArgs e)
        {
            folderBrowserPopup.IsOpen = false;
        }

        private void DataGridRowContextMenuOpening(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).ContextMenu.DataContext = ViewModel;
        }

        private void DataGridRowMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {    
            ViewModel.PlayerService.PlaySelectedCommand.Execute(null);   
        }

        private void DataGridRowMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var draggedItem = (DataGridRow)sender;
                var items = musicFilesGrid.ItemsSource.Cast<MusicFileDataModel>().ToList();
                var selectedItems = musicFilesGrid.SelectedItems.Cast<MusicFileDataModel>().OrderBy(x => items.IndexOf(x)).ToArray();
                DragDrop.DoDragDrop(draggedItem, selectedItems.Select(x => x.MusicFile).ToArray(), DragDropEffects.Copy);
            }
        }

        private void UserPathBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var bindingExpression = BindingOperations.GetBindingExpressionBase(userPathBox, TextBox.TextProperty);
                bindingExpression.UpdateSource();
            }
        }

        private async void DirectoriesListBoxItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure that Binding is updated before executing the command. 
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Input);  
            ViewModel.NavigateToSelectedSubDirectoryCommand.Execute(null);
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            searchPopup.Width = searchButton.ActualWidth + clearSearchButton.ActualWidth + 75;
            searchPopup.IsOpen = true;
            userSearchFilterBox.SelectAll();
        }

        private void StatusBarButtonClick(object sender, RoutedEventArgs e)
        {
            menuPopup.Width = statusBarButton.ActualWidth;
            menuPopup.IsOpen = true;
        }

        private void MusicFilesGridSorting(object sender, DataGridSortingEventArgs e)
        {
            var sort = DataGridHelper.HandleDataGridSorting<MusicFileDataModel>(e);
            if (e.Column == titleColumn)
            {
                if (e.Column.SortDirection == null) sort = null;
                else sort = x => x.OrderBy(y => y, new ListSortComparer<MusicFileDataModel>(TitleColumnComparison, e.Column.SortDirection.Value));
            }
            else if (e.Column == genreColumn)
            {
                if (e.Column.SortDirection == null) sort = null;
                else sort = x => x.OrderBy(y => y, new ListSortComparer<MusicFileDataModel>(GenreColumnComparison, e.Column.SortDirection.Value));
            }
            ViewModel.SelectionService.MusicFiles.Sort = sort;
        }

        private void TitleColumnWidthChanged(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(AutoShowHideColumns, DispatcherPriority.Background);
        }

        private void AutoShowHideColumns()
        {
            const double tolerance = 5;
            const double titleColumnMinWidth = 250;
            if (titleColumn.Width.DisplayValue + tolerance < titleColumnMinWidth)
            {
                var lastVisibleColumn = autoColumns.LastOrDefault(x => x.Visibility == Visibility.Visible);
                if (lastVisibleColumn != null)
                {
                    lastVisibleColumn.Visibility = Visibility.Collapsed;
                    Dispatcher.InvokeAsync(AutoShowHideColumns, DispatcherPriority.Background);
                }
            }
            else
            {
                var firstCollapsedColumn = autoColumns.FirstOrDefault(x => x.Visibility == Visibility.Collapsed);
                if (firstCollapsedColumn != null && titleColumn.Width.DisplayValue - tolerance > titleColumnMinWidth + firstCollapsedColumn.Width.DisplayValue)
                {
                    firstCollapsedColumn.Visibility = Visibility.Visible;
                    Dispatcher.InvokeAsync(AutoShowHideColumns, DispatcherPriority.Background);
                }
            }
        }

        private void FocusMusicFilesGrid()
        {
            musicFilesGrid.Focus();
            musicFilesGrid.CurrentCell = new DataGridCellInfo(musicFilesGrid.SelectedItem, musicFilesGrid.Columns[0]);
        }

        private static int TitleColumnComparison(MusicFileDataModel x, MusicFileDataModel y)
        {
            var titleX = MusicTitleHelper.GetTitleText(x.MusicFile.FileName, 
                    x.MusicFile.IsMetadataLoaded ? x.MusicFile.Metadata.Artists : null, x.MusicFile.IsMetadataLoaded ? x.MusicFile.Metadata.Title : null);
            var titleY = MusicTitleHelper.GetTitleText(y.MusicFile.FileName, 
                    y.MusicFile.IsMetadataLoaded ? y.MusicFile.Metadata.Artists : null, y.MusicFile.IsMetadataLoaded ? y.MusicFile.Metadata.Title : null);
            return string.Compare(titleX, titleY, StringComparison.CurrentCulture);
        }

        private static int GenreColumnComparison(MusicFileDataModel x, MusicFileDataModel y)
        {
            var genreX = x.MusicFile.IsMetadataLoaded ? StringListConverter.ToString(x.MusicFile.Metadata.Genre) : "";
            var genreY = y.MusicFile.IsMetadataLoaded ?  StringListConverter.ToString(y.MusicFile.Metadata.Genre) : "";
            return string.Compare(genreX, genreY, StringComparison.CurrentCulture);
        }
    }
}
