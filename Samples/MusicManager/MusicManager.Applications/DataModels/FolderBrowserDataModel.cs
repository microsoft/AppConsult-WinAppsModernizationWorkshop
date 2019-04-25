using System.Collections.Generic;
using System.Waf.Foundation;

namespace Waf.MusicManager.Applications.DataModels
{
    public class FolderBrowserDataModel : Model
    {
        private string userPath;
        private string currentPath;
        private IReadOnlyList<FolderItem> subDirectories;
        private FolderItem selectedSubDirectory;

        public string UserPath
        {
            get => userPath;
            set => SetProperty(ref userPath, value ?? "");
        }

        public string CurrentPath
        {
            get => currentPath;
            set => SetProperty(ref currentPath, value ?? "");
        }

        public IReadOnlyList<FolderItem> SubDirectories
        {
            get => subDirectories;
            set => SetProperty(ref subDirectories, value);
        }

        public FolderItem SelectedSubDirectory
        {
            get => selectedSubDirectory;
            set => SetProperty(ref selectedSubDirectory, value);
        }
    }
}
