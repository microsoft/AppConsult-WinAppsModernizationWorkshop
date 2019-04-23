using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Windows.Storage;
using Windows.Storage.Search;

namespace Waf.MusicManager.Applications.Controllers
{
    [Export]
    internal class ManagerController
    {
        private readonly IShellService shellService;
        private readonly IEnvironmentService environmentService;
        private readonly IMusicFileContext musicFileContext;
        private readonly SelectionService selectionService;
        private readonly ManagerStatusService managerStatusService;
        private readonly IFileSystemWatcherService fileSystemWatcherService;
        private readonly Lazy<ManagerViewModel> managerViewModel;
        private readonly ObservableCollection<MusicFile> musicFiles;
        private readonly DelegateCommand updateSubDirectoriesCommand;
        private readonly DelegateCommand navigateDirectoryUpCommand;
        private readonly DelegateCommand navigateHomeCommand;
        private readonly DelegateCommand navigatePublicHomeCommand;
        private readonly DelegateCommand loadRecursiveCommand;
        private readonly DelegateCommand navigateToSelectedSubDirectoryCommand;
        private readonly DelegateCommand showMusicPropertiesCommand;
        private readonly DelegateCommand deleteSelectedFilesCommand;
        private CancellationTokenSource updateMusicFilesCancellation;
        
        [ImportingConstructor]
        public ManagerController(IShellService shellService, IEnvironmentService environmentService, IMusicFileContext musicFileContext, 
            SelectionService selectionService, ManagerStatusService managerStatusService, IFileSystemWatcherService fileSystemWatcherService, 
            Lazy<ManagerViewModel> managerViewModel)
        {
            this.shellService = shellService;
            this.environmentService = environmentService;
            this.musicFileContext = musicFileContext;
            this.selectionService = selectionService;
            this.managerStatusService = managerStatusService;
            this.fileSystemWatcherService = fileSystemWatcherService;
            this.managerViewModel = managerViewModel;
            musicFiles = new ObservableCollection<MusicFile>();
            updateSubDirectoriesCommand = new DelegateCommand(UpdateSubDirectories);
            navigateDirectoryUpCommand = new DelegateCommand(NavigateDirectoryUp, CanNavigateDirectoryUp);
            navigateHomeCommand = new DelegateCommand(NavigateHome);
            navigatePublicHomeCommand = new DelegateCommand(NavigatePublicHome);
            loadRecursiveCommand = new DelegateCommand(LoadRecursive);
            navigateToSelectedSubDirectoryCommand = new DelegateCommand(NavigateToSelectedSubDirectory);
            showMusicPropertiesCommand = new DelegateCommand(ShowMusicProperties);
            deleteSelectedFilesCommand = new DelegateCommand(DeleteSelectedFiles);
        }

        private ManagerViewModel ManagerViewModel => managerViewModel.Value;

        public void Initialize()
        {
            selectionService.Initialize(musicFiles);

            fileSystemWatcherService.NotifyFilter = NotifyFilters.FileName;
            fileSystemWatcherService.Created += FileSystemWatcherServiceCreated;
            fileSystemWatcherService.Renamed += FileSystemWatcherServiceRenamed;
            fileSystemWatcherService.Deleted += FileSystemWatcherServiceDeleted;

            ManagerViewModel.UpdateSubDirectoriesCommand = updateSubDirectoriesCommand;
            ManagerViewModel.NavigateDirectoryUpCommand = navigateDirectoryUpCommand;
            ManagerViewModel.NavigateHomeCommand = navigateHomeCommand;
            ManagerViewModel.NavigatePublicHomeCommand = navigatePublicHomeCommand;
            ManagerViewModel.LoadRecursiveCommand = loadRecursiveCommand;
            ManagerViewModel.NavigateToSelectedSubDirectoryCommand = navigateToSelectedSubDirectoryCommand;
            ManagerViewModel.ShowMusicPropertiesCommand = showMusicPropertiesCommand;
            ManagerViewModel.DeleteSelectedFilesCommand = deleteSelectedFilesCommand;
            ManagerViewModel.FolderBrowser.PropertyChanged += FolderBrowserPropertyChanged;
            ManagerViewModel.SearchFilter.PropertyChanged += SearchFilterPropertyChanged;

            try
            {
                ManagerViewModel.FolderBrowser.CurrentPath = shellService.Settings.CurrentPath ?? environmentService.MusicPath;
            }
            catch (Exception)
            {
                ManagerViewModel.FolderBrowser.CurrentPath = environmentService.MusicPath;
            }
            
            shellService.ContentView = ManagerViewModel.View;
        }

        public void Shutdown()
        {
            shellService.Settings.CurrentPath = ManagerViewModel.FolderBrowser.CurrentPath;
        }

        private bool CanNavigateDirectoryUp() { return !string.IsNullOrEmpty(ManagerViewModel.FolderBrowser.CurrentPath); }
        
        private void NavigateDirectoryUp()
        {
            try
            {
                using (shellService.SetApplicationBusy())
                {
                    var folder = StorageFolder.GetFolderFromPathAsync(ManagerViewModel.FolderBrowser.CurrentPath).GetResult();
                    var parent = folder.GetParentAsync().GetResult();
                    ManagerViewModel.FolderBrowser.CurrentPath = parent?.Path ?? "";
                }
            }
            catch (Exception)
            {
                // This can happen when we have lost the connection to a network drive.
                ManagerViewModel.FolderBrowser.CurrentPath = "";
            }
        }

        private void NavigateHome()
        {
            ManagerViewModel.FolderBrowser.CurrentPath = environmentService.MusicPath;
        }

        private void NavigatePublicHome()
        {
            ManagerViewModel.FolderBrowser.CurrentPath = environmentService.PublicMusicPath;
        }

        private void LoadRecursive()
        {
            UpdateMusicFiles(FolderDepth.Deep);
        }

        private void NavigateToSelectedSubDirectory()
        {
            if (ManagerViewModel.FolderBrowser.SelectedSubDirectory == null) 
            { 
                throw new InvalidOperationException("SelectedSubDirectory must not be null."); 
            }
            ManagerViewModel.FolderBrowser.CurrentPath = ManagerViewModel.FolderBrowser.SelectedSubDirectory.Path;
        }

        private void ShowMusicProperties()
        {
            shellService.ShowMusicPropertiesView();
        }

        private void DeleteSelectedFiles()
        {
            foreach (var musicFile in selectionService.SelectedMusicFiles)
            {
                DeleteFileAsync(musicFile.MusicFile.FileName);
            }
        }

        private async void DeleteFileAsync(string fileName)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(fileName);
                await file.DeleteAsync();
            }
            catch (Exception ex)
            {
                shellService.ShowError(ex, Resources.CouldNotDeleteFile, fileName);
            }
        }

        private void UpdateSubDirectories()
        {
            try
            {
                ManagerViewModel.FolderBrowser.SubDirectories = GetSubFoldersFromPath(ManagerViewModel.FolderBrowser.CurrentPath);
            }
            catch (Exception ex)
            {
                Log.Default.Warn(ex, "UpdateSubDirectories");
            }
        }

        private async void UpdateMusicFiles(FolderDepth folderDepth)
        {
            updateMusicFilesCancellation?.Cancel();
            var cancellation = new CancellationTokenSource();
            updateMusicFilesCancellation = cancellation;
            Log.Default.Trace("ManagerController.UpdateMusicFiles:Start");
            managerStatusService.StartUpdatingFilesList();
            
            musicFiles.Clear();
            var path = ManagerViewModel.FolderBrowser.CurrentPath;
            try
            {
                var filesCount = 0;
                if (Directory.Exists(path))
                {
                    var userSearchFilter = ManagerViewModel.SearchFilter.UserSearchFilter;
                    var applicationSearchFilter = ManagerViewModel.SearchFilter.ApplicationSearchFilter;
                    var files = await GetFilesAsync(path, folderDepth, userSearchFilter, applicationSearchFilter, cancellation.Token);

                    filesCount = files.Count;

                    var newFiles = files.Select(x => musicFileContext.Create(x)).ToArray();
                    foreach (var newFile in newFiles)
                    {
                        musicFiles.Add(newFile);
                    }

                    if (selectionService.MusicFiles.Any())
                    {
                        selectionService.SelectedMusicFiles.Clear();
                        selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles.First());
                    }
                }
                managerStatusService.FinishUpdatingFilesList(filesCount);
            }
            catch (OperationCanceledException)
            {
                Log.Default.Trace("ManagerController.UpdateMusicFiles:Canceled");
            }
            
            if (cancellation == updateMusicFilesCancellation)
            {
                updateMusicFilesCancellation = null;
            }

            Log.Default.Trace("ManagerController.UpdateMusicFiles:End");
        }

        private void FolderBrowserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FolderBrowserDataModel.UserPath))
            {
                // This might throw an exception => shown in the Path TextBox as validation error.
                ManagerViewModel.FolderBrowser.CurrentPath = GetFolderFromPath(ManagerViewModel.FolderBrowser.UserPath).Path;
            }
            if (e.PropertyName == nameof(FolderBrowserDataModel.CurrentPath))
            {
                navigateDirectoryUpCommand.RaiseCanExecuteChanged();
                ManagerViewModel.FolderBrowser.UserPath = FolderHelper.GetDisplayPath(ManagerViewModel.FolderBrowser.CurrentPath).GetResult();
                UpdateSubDirectories();
                ManagerViewModel.SearchFilter.Clear();
                UpdateMusicFiles(FolderDepth.Shallow);
            }
        }

        private void SearchFilterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchFilterDataModel.UserSearchFilter) || e.PropertyName == nameof(SearchFilterDataModel.ApplicationSearchFilter))
            {
                var userSearchFilter = ManagerViewModel.SearchFilter.UserSearchFilter;
                var applicationSearchFilter = ManagerViewModel.SearchFilter.ApplicationSearchFilter;
                if (string.IsNullOrEmpty(userSearchFilter) && string.IsNullOrEmpty(applicationSearchFilter))
                {
                    // Reset the search; behave like the user navigated into another folder.
                    UpdateMusicFiles(FolderDepth.Shallow);
                }
                else
                {
                    UpdateMusicFiles(FolderDepth.Deep);
                }   
            }
        }

        private Task<IReadOnlyList<string>> GetFilesAsync(string directory, FolderDepth folderDepth, string userSearchFilter, string applicationSearchFilter, CancellationToken cancellationToken)
        {
            if (folderDepth == FolderDepth.Shallow)
            {
                fileSystemWatcherService.Path = directory;
                fileSystemWatcherService.EnableRaisingEvents = true;
            }
            else
            {
                fileSystemWatcherService.EnableRaisingEvents = false;
            }
            
            // It is necessary to run this in an own task => otherwise, reentrance would block the UI thread although this should not happen.
            return Task.Run(() => GetFilesCore(directory, folderDepth, userSearchFilter, applicationSearchFilter, cancellationToken));
        }

        private static IReadOnlyList<string> GetFilesCore(string directory, FolderDepth folderDepth, string userSearchFilter, string applicationSearchFilter, 
            CancellationToken cancellationToken)
        {
            // This method is run in an task (not in the UI thread) => static ensures some thread-safety.
            var folder = StorageFolder.GetFolderFromPathAsync(directory).GetResult(cancellationToken);
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, SupportedFileTypes.MusicFileExtensions) 
            { 
                UserSearchFilter = userSearchFilter ?? "", 
                ApplicationSearchFilter = applicationSearchFilter ?? "", 
                FolderDepth = folderDepth 
            };
            var result = folder.CreateFileQueryWithOptions(queryOptions);
            
            // It seems that GetFilesAsync does not check the cancellationToken; so get only parts of the file results in a loop.
            Log.Default.Trace("ManagerController.UpdateMusicFiles:GetFilesAsync Start");
            var files = new List<string>();
            uint index = 0;
            int resultCount;
            const uint maxFiles = 100;
            do
            {
                var filesResult = result.GetFilesAsync(index, maxFiles).GetResult(cancellationToken);
                resultCount = filesResult.Count;
                files.AddRange(filesResult.Select(x => x.Path));
                index += maxFiles;
            }
            while (resultCount == maxFiles);
            Log.Default.Trace("ManagerController.UpdateMusicFiles:GetFilesAsync End");
            return files;
        }

        private static IReadOnlyList<FolderItem> GetSubFoldersFromPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var folder = StorageFolder.GetFolderFromPathAsync(path).GetResult();
                var result = folder.CreateFolderQuery();
                var folders = result.GetFoldersAsync().GetResult();
                return folders.Select(x => new FolderItem(x.Path, x.DisplayName)).ToArray();
            }
            else
            {
                var driveInfos = DriveInfo.GetDrives();
                return driveInfos.Select(x =>
                {
                    var displayName = StorageFolder.GetFolderFromPathAsync(x.RootDirectory.FullName).GetResult().DisplayName;
                    return new FolderItem(x.RootDirectory.FullName, displayName);
                }).ToArray();
            }
        }

        private static FolderItem GetFolderFromPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var folder = FolderHelper.GetFolderFromLocalizedPathAsync(path).GetResult();
                return new FolderItem(folder.Path, folder.DisplayName);
            }
            else
            {
                return new FolderItem(null, "Root");
            }
        }

        private void InsertMusicFile(string fileName)
        {
            if (!SupportedFileTypes.MusicFileExtensions.Contains(Path.GetExtension(fileName)))
            {
                return;
            }

            var insertFileName = Path.GetFileName(fileName);
            int i;
            for (i = 0; i < musicFiles.Count; i++)
            {
                if (string.Compare(insertFileName, Path.GetFileName(musicFiles[i].FileName), true, CultureInfo.CurrentCulture) <= 0)
                {
                    break;
                }
            }
            musicFiles.Insert(i, musicFileContext.Create(fileName));
        }

        private void RemoveMusicFile(string fileName)
        {
            var musicFileToRemove = musicFiles.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            if (musicFileToRemove != null)
            {
                musicFiles.Remove(musicFileToRemove);
            }
        }

        private void FileSystemWatcherServiceCreated(object sender, FileSystemEventArgs e)
        {
            InsertMusicFile(e.FullPath);
        }

        private void FileSystemWatcherServiceRenamed(object sender, RenamedEventArgs e)
        {
            RemoveMusicFile(e.OldFullPath);
            InsertMusicFile(e.FullPath);   
        }

        private void FileSystemWatcherServiceDeleted(object sender, FileSystemEventArgs e)
        {
            RemoveMusicFile(e.FullPath);
        }
    }
}
