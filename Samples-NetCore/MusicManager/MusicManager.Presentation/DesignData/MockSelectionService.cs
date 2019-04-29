using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Waf.Applications;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class MockSelectionService : ISelectionService
    {
        private readonly ObservableCollection<MusicFileDataModel> innerMusicFiles;

        public MockSelectionService()
        {
            innerMusicFiles = new ObservableCollection<MusicFileDataModel>();
            SelectedMusicFiles = new ObservableCollection<MusicFileDataModel>();
            MusicFiles = new ObservableListView<MusicFileDataModel>(innerMusicFiles);
        }

        public ObservableListView<MusicFileDataModel> MusicFiles { get; }

        public IList<MusicFileDataModel> SelectedMusicFiles { get; }
        
        public void SetMusicFiles(IEnumerable<MusicFileDataModel> musicFiles)
        {
            innerMusicFiles.Clear();
            musicFiles.ToList().ForEach(x => innerMusicFiles.Add(x));
        }
    }
}
