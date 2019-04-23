using System.Collections.ObjectModel;
using System.Waf.Foundation;

namespace Waf.MusicManager.Domain.Transcoding
{
    public class TranscodingManager
    {
        private readonly ObservableCollection<TranscodeItem> transcodeItems;

        public TranscodingManager()
        {
            transcodeItems = new ObservableCollection<TranscodeItem>();
            TranscodeItems = new ReadOnlyObservableList<TranscodeItem>(transcodeItems);
        }

        public IReadOnlyObservableList<TranscodeItem> TranscodeItems { get; }

        public void AddTranscodeItem(TranscodeItem item)
        {
            transcodeItems.Add(item);
        }

        public void RemoveTranscodeItem(TranscodeItem item)
        {
            transcodeItems.Remove(item);
        }
    }
}
