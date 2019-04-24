using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class MusicPropertiesViewModel : ViewModel<IMusicPropertiesView>
    {
        private readonly IClipboardService clipboardService;
        private readonly DelegateCommand autoFillFromFileNameCommand;
        private MusicFile musicFile;

        [ImportingConstructor]
        public MusicPropertiesViewModel(IMusicPropertiesView view, IClipboardService clipboardService) : base(view)
        {
            this.clipboardService = clipboardService;
            CopyFileNameCommand = new DelegateCommand(CopyFileNameToClipboard);
            autoFillFromFileNameCommand = new DelegateCommand(AutoFillFromFileName, CanAutoFillFromFileName);
        }

        public ICommand CopyFileNameCommand { get; }

        public ICommand AutoFillFromFileNameCommand => autoFillFromFileNameCommand;

        public MusicFile MusicFile
        {
            get => musicFile;
            set
            {
                if (musicFile == value) { return; }

                if (musicFile != null)
                {
                    PropertyChangedEventManager.RemoveHandler(musicFile, MusicFilePropertyChanged, "");
                    if (musicFile.IsMetadataLoaded)
                    {
                        PropertyChangedEventManager.RemoveHandler(musicFile.Metadata, MetadataPropertyChanged, "");
                    }
                }
                musicFile = value;
                if (musicFile != null)
                {
                    PropertyChangedEventManager.AddHandler(musicFile, MusicFilePropertyChanged, "");
                    if (musicFile.IsMetadataLoaded)
                    {
                        MetadataLoaded();
                    }
                }
                RaisePropertyChanged();
                autoFillFromFileNameCommand.RaiseCanExecuteChanged();
            }
        }

        private void CopyFileNameToClipboard()
        {
            if (MusicFile != null)
            {
                clipboardService.SetText(Path.GetFileNameWithoutExtension(MusicFile.FileName));
            }
        }

        private bool CanAutoFillFromFileName()
        {
            return MusicFile != null && MusicFile.IsMetadataLoaded && musicFile.Metadata.IsSupported
                && string.IsNullOrEmpty(MusicFile.Metadata.Title) && !MusicFile.Metadata.Artists.Any();
        }

        private void AutoFillFromFileName()
        {
            var fileName = Path.GetFileNameWithoutExtension(MusicFile.FileName);
            var metadata = fileName.Split(new[] { '-' }, 2).Select(x => x.Trim()).ToArray();
            if (metadata.Length == 2)
            {
                MusicFile.Metadata.Artists = new[] { metadata[0] };
                MusicFile.Metadata.Title = metadata[1];
            }
            else
            {
                MusicFile.Metadata.Title = fileName;
            }
        }

        private void MetadataLoaded()
        {
            PropertyChangedEventManager.AddHandler(MusicFile.Metadata, MetadataPropertyChanged, "");
            autoFillFromFileNameCommand.RaiseCanExecuteChanged();
        }

        private void MusicFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicFile.IsMetadataLoaded))
            {
                MetadataLoaded();
            }
        }

        private void MetadataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicMetadata.Title) || e.PropertyName == nameof(MusicMetadata.Artists))
            {
                autoFillFromFileNameCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
