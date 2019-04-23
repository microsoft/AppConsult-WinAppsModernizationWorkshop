using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services
{
    [Export, Export(typeof(ISelectionService))]
    internal class SelectionService : ISelectionService
    {
        [ImportingConstructor]
        public SelectionService()
        {
            SelectedMusicFiles = new ObservableCollection<MusicFileDataModel>();
        }

        public ObservableListView<MusicFileDataModel> MusicFiles { get; private set; }

        public IList<MusicFileDataModel> SelectedMusicFiles { get; }

        public void Initialize(IEnumerable<MusicFile> musicFiles)
        {
            MusicFiles = new ObservableListView<MusicFileDataModel>(
                new SynchronizingCollection<MusicFileDataModel, MusicFile>(musicFiles, x => new MusicFileDataModel(x)));
        }
    }
}
