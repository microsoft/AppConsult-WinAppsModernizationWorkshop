using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class ManagerViewModel : ViewModel<IManagerView>
    {
        private readonly Lazy<ISelectionService> selectionService;
        private MusicFileDataModel selectedMusicFile;
        private ICommand updateSubDirectoriesCommand;
        private ICommand navigateDirectoryUpCommand;
        private ICommand navigateHomeCommand;
        private ICommand navigatePublicHomeCommand;
        private ICommand loadRecursiveCommand;
        private ICommand navigateToSelectedSubDirectoryCommand;
        private ICommand showMusicPropertiesCommand;
        private ICommand deleteSelectedFilesCommand;

        [ImportingConstructor]
        public ManagerViewModel(IManagerView view, Lazy<ISelectionService> selectionService, IManagerStatusService managerStatusService, 
            IPlayerService playerService, ITranscodingService transcodingService)
            : base(view)
        {
            this.selectionService = selectionService;
            ManagerStatusService = managerStatusService;
            PlayerService = playerService;
            TranscodingService = transcodingService;
            FolderBrowser = new FolderBrowserDataModel();
            SearchFilter = new SearchFilterDataModel();
            ClearSearchCommand = new DelegateCommand(ClearSearch);
        }

        public ISelectionService SelectionService => selectionService.Value;
        
        public IManagerStatusService ManagerStatusService { get; }

        public IPlayerService PlayerService { get; }

        public ITranscodingService TranscodingService { get; }

        public FolderBrowserDataModel FolderBrowser { get; }

        public SearchFilterDataModel SearchFilter { get; }

        public MusicFileDataModel SelectedMusicFile
        {
            get => selectedMusicFile;
            set => SetProperty(ref selectedMusicFile, value);
        }

        public ICommand UpdateSubDirectoriesCommand
        {
            get => updateSubDirectoriesCommand;
            set => SetProperty(ref updateSubDirectoriesCommand, value);
        }

        public ICommand NavigateDirectoryUpCommand
        {
            get => navigateDirectoryUpCommand;
            set => SetProperty(ref navigateDirectoryUpCommand, value);
        }

        public ICommand NavigateHomeCommand
        {
            get => navigateHomeCommand;
            set => SetProperty(ref navigateHomeCommand, value);
        }

        public ICommand NavigatePublicHomeCommand
        {
            get => navigatePublicHomeCommand;
            set => SetProperty(ref navigatePublicHomeCommand, value);
        }

        public ICommand LoadRecursiveCommand
        {
            get => loadRecursiveCommand;
            set => SetProperty(ref loadRecursiveCommand, value);
        }

        public ICommand NavigateToSelectedSubDirectoryCommand
        {
            get => navigateToSelectedSubDirectoryCommand;
            set => SetProperty(ref navigateToSelectedSubDirectoryCommand, value);
        }

        public ICommand ClearSearchCommand { get; }

        public ICommand ShowMusicPropertiesCommand
        {
            get => showMusicPropertiesCommand;
            set => SetProperty(ref showMusicPropertiesCommand, value);
        }

        public ICommand DeleteSelectedFilesCommand
        {
            get => deleteSelectedFilesCommand;
            set => SetProperty(ref deleteSelectedFilesCommand, value);
        }

        private void ClearSearch()
        {
            SearchFilter.Clear();
        }
    }
}
