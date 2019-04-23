using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class TranscodingListViewModel : ViewModel<ITranscodingListView>
    {
        private TranscodingManager transcodingManager;

        [ImportingConstructor]
        public TranscodingListViewModel(ITranscodingListView view, ITranscodingService transcodingService) : base(view)
        {
            TranscodingService = transcodingService;
            SelectedTranscodeItems = new ObservableCollection<TranscodeItem>();
        }

        public ITranscodingService TranscodingService { get; }

        public IList<TranscodeItem> SelectedTranscodeItems { get; }

        public TranscodingManager TranscodingManager
        {
            get => transcodingManager;
            set => SetProperty(ref transcodingManager, value);
        }

        public Action<int, IEnumerable<string>> InsertFilesAction { get; set; }

        public Action<int, IEnumerable<MusicFile>> InsertMusicFilesAction { get; set; }
    }
}
