using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;

namespace Waf.MusicManager.Applications.Controllers
{
    [Export]
    internal class TranscodingController
    {
        private readonly IMessageService messageService;
        private readonly IShellService shellService;
        private readonly IMusicFileContext musicFileContext;
        private readonly ISelectionService selectionService;
        private readonly TranscodingService transcodingService;
        private readonly Lazy<ITranscoder> transcoder;
        private readonly Lazy<TranscodingListViewModel> transcodingListViewModel;
        private readonly Dictionary<TranscodeItem, CancellationTokenSource> cancellationTokenSources;
        private readonly DelegateCommand convertToMp3AllCommand;
        private readonly DelegateCommand convertToMp3SelectedCommand;
        private readonly DelegateCommand cancelAllCommand;
        private readonly DelegateCommand cancelSelectedCommand;
        private readonly SemaphoreSlim throttler;
        private readonly TranscodingManager transcodingManager;
        private readonly ThrottledAction throttledMusicFilesCollectionChangedAction;
        private TaskCompletionSource<object> allTranscodingsCanceledCompletion;

        [ImportingConstructor]
        public TranscodingController(IMessageService messageService, IShellService shellService, IMusicFileContext musicFileContext, ISelectionService selectionService, 
            TranscodingService transcodingService, Lazy<ITranscoder> transcoder, Lazy<TranscodingListViewModel> transcodingListViewModel)
        {
            this.messageService = messageService;
            this.shellService = shellService;
            this.musicFileContext = musicFileContext;
            this.selectionService = selectionService;
            this.transcodingService = transcodingService;
            this.transcoder = transcoder;
            this.transcodingListViewModel = transcodingListViewModel;
            cancellationTokenSources = new Dictionary<TranscodeItem, CancellationTokenSource>();
            convertToMp3AllCommand = new DelegateCommand(ConvertToMp3All, CanConvertToMp3All);
            convertToMp3SelectedCommand = new DelegateCommand(ConvertToMp3Selected, CanConvertToMp3Selected);
            cancelAllCommand = new DelegateCommand(CancelAll, CanCancelAll);
            cancelSelectedCommand = new DelegateCommand(CancelSelected, CanCancelSelected);
            throttler = new SemaphoreSlim(Environment.ProcessorCount);  // Do not dispose the throttler; it is used after Shutdown to cancel the open tasks
            transcodingManager = new TranscodingManager();
            throttledMusicFilesCollectionChangedAction = new ThrottledAction(ThrottledMusicFilesCollectionChanged, ThrottledActionMode.InvokeOnlyIfIdleForDelayTime, TimeSpan.FromMilliseconds(10));
        }

        private TranscodingListViewModel TranscodingListViewModel => transcodingListViewModel.Value;

        public void Initialize()
        {
            transcodingService.ConvertToMp3AllCommand = convertToMp3AllCommand;
            transcodingService.ConvertToMp3SelectedCommand = convertToMp3SelectedCommand;
            transcodingService.CancelAllCommand = cancelAllCommand;
            transcodingService.CancelSelectedCommand = cancelSelectedCommand;

            shellService.TranscodingListView = new Lazy<object>(InitializeTranscodingListView);

            shellService.Closing += ShellServiceClosing;
            selectionService.MusicFiles.CollectionChanged += (sender, e) => throttledMusicFilesCollectionChangedAction.InvokeAccumulated();
            ((INotifyCollectionChanged)selectionService.SelectedMusicFiles).CollectionChanged += SelectedMusicFilesCollectionChanged;
        }

        public void Shutdown()
        {
            if (cancellationTokenSources.Any())
            {
                allTranscodingsCanceledCompletion = new TaskCompletionSource<object>();
                CancelAll();
                shellService.AddTaskToCompleteBeforeShutdown(allTranscodingsCanceledCompletion.Task);
            }
        }

        private object InitializeTranscodingListView()
        {
            TranscodingListViewModel.TranscodingManager = transcodingManager;
            TranscodingListViewModel.InsertFilesAction = InsertFiles;
            TranscodingListViewModel.InsertMusicFilesAction = InsertMusicFiles;
            CollectionChangedEventManager.AddHandler((INotifyCollectionChanged)TranscodingListViewModel.SelectedTranscodeItems, SelectedTranscodeItemsCollectionChanged);
            return TranscodingListViewModel.View;
        }

        private void ShellServiceClosing(object sender, CancelEventArgs e)
        {
            if (cancellationTokenSources.Any())
            {
                e.Cancel = !messageService.ShowYesNoQuestion(shellService.ShellView, Resources.AbortRunningConverts);
            }
        }

        private bool CanConvertToMp3All()
        {
            return GetMusicFilesSupportedToConvert(selectionService.MusicFiles.Select(x => x.MusicFile)).Any();
        }
        
        private void ConvertToMp3All()
        {
            Transcode(GetMusicFilesSupportedToConvert(selectionService.MusicFiles.Select(x => x.MusicFile)).ToArray());
        }

        private bool CanConvertToMp3Selected()
        {
            return GetMusicFilesSupportedToConvert(selectionService.SelectedMusicFiles.Select(x => x.MusicFile)).Any();
        }

        private void ConvertToMp3Selected()
        {
            Transcode(GetMusicFilesSupportedToConvert(selectionService.SelectedMusicFiles.Select(x => x.MusicFile)).ToArray());
        }

        private void InsertFiles(int index, IEnumerable<string> fileNames)
        {
            MusicFile[] musicFiles;
            try
            {
                musicFiles = fileNames.Select(musicFileContext.Create).ToArray();
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "TranscodingController.InsertFile");
                shellService.ShowError(ex, Resources.CouldNotOpenFiles);
                return;
            }
            Transcode(GetMusicFilesSupportedToConvert(musicFiles).ToArray());
        }

        private void InsertMusicFiles(int index, IEnumerable<MusicFile> musicFiles)
        {
            Transcode(GetMusicFilesSupportedToConvert(musicFiles).ToArray());
        }

        private bool CanCancelAll()
        {
            return cancellationTokenSources.Any(x => !x.Value.IsCancellationRequested);
        }

        private void CancelAll()
        {
            Cancel(cancellationTokenSources.Where(x => !x.Value.IsCancellationRequested));
        }

        private bool CanCancelSelected()
        {
            return cancellationTokenSources.Any(x => !x.Value.IsCancellationRequested && TranscodingListViewModel.SelectedTranscodeItems.Any(y => y == x.Key));
        }

        private void CancelSelected()
        {
            Cancel(cancellationTokenSources.Where(x => !x.Value.IsCancellationRequested && TranscodingListViewModel.SelectedTranscodeItems.Any(y => y == x.Key)));
        }

        private void Cancel(IEnumerable<KeyValuePair<TranscodeItem, CancellationTokenSource>> sources)
        {
            UpdateCancelCommands();
            foreach (var source in sources)
            {
                source.Value.Cancel();
            }
        }

        private IEnumerable<MusicFile> GetMusicFilesSupportedToConvert(IEnumerable<MusicFile> musicFiles)
        {
            return musicFiles.Where(x => !string.IsNullOrEmpty(x.FileName) && !x.FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) 
                && selectionService.MusicFiles.All(y => y.MusicFile.FileName != GetDestinationFileName(x.FileName)));
        }

        private static string GetDestinationFileName(string sourceFileName)
        {
            int indexOfLastPoint = sourceFileName.LastIndexOf('.');
            var newFileName = indexOfLastPoint < 0 ? sourceFileName : sourceFileName.Substring(0, indexOfLastPoint);
            return newFileName + ".mp3";
        }
        
        private void Transcode(IReadOnlyCollection<MusicFile> musicFiles)
        {
            shellService.ShowTranscodingListView();
            foreach (var musicFile in musicFiles)
            {
                TranscodeAsync(musicFile);
            }
        }

        private async void TranscodeAsync(MusicFile musicFile)
        {
            var destinationFileName = GetDestinationFileName(musicFile.FileName);
            var transcodeItem = new TranscodeItem(musicFile, destinationFileName);
            transcodingManager.AddTranscodeItem(transcodeItem);
            Log.Default.Trace("Start Transcode: {0} > {1}", musicFile.FileName, destinationFileName);

            var cts = new CancellationTokenSource();
            cancellationTokenSources.Add(transcodeItem, cts);
            UpdateCancelCommands();
            var metadata = await musicFile.GetMetadataAsync();
            uint bitrate = GetConvertBitrate(metadata.Bitrate);

            try
            {
                await TranscodeAsyncCore(transcodeItem, bitrate, cts.Token);
            }
            catch (OperationCanceledException)
            {
                transcodingManager.RemoveTranscodeItem(transcodeItem);
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "TranscodeAsync exception");
                transcodeItem.Error = ex;
            }
            finally
            {
                cancellationTokenSources.Remove(transcodeItem);
                if (allTranscodingsCanceledCompletion != null && !cancellationTokenSources.Any())
                {
                    allTranscodingsCanceledCompletion.SetResult(null);
                }
                UpdateCancelCommands();
                Log.Default.Trace("End Transcode: {0} > {1}", musicFile.FileName, destinationFileName);
            }
        }

        private async Task TranscodeAsyncCore(TranscodeItem transcodeItem, uint bitrate, CancellationToken token)
        {
            await throttler.WaitAsync(token);  // Throttle the transcoding
            try
            {
                var task = transcoder.Value.TranscodeAsync(transcodeItem.Source.FileName, transcodeItem.DestinationFileName, bitrate,
                        token, new Progress<double>(x => transcodeItem.Progress = x / 100d));
                transcodingService.RaiseTranscodingTaskCreated(transcodeItem.DestinationFileName, task);
                await task;

                var destinationMusicFile = musicFileContext.Create(transcodeItem.DestinationFileName);
                var sourceMetadata = await transcodeItem.Source.GetMetadataAsync();
                var destinationMetadata = await destinationMusicFile.GetMetadataAsync();
                destinationMetadata.ApplyValuesFrom(sourceMetadata);
                await musicFileContext.SaveChangesAsync(destinationMusicFile);
            }
            finally
            {
                throttler.Release();
            }
        }

        internal static uint GetConvertBitrate(long sourceBitrate)
        {
            if (sourceBitrate <= 128000)
            {
                return 128000;
            }
            else if (sourceBitrate <= 192000)
            {
                return 192000;
            }
            else if (sourceBitrate <= 256000)
            {
                return 256000;
            }
            else
            {
                return 320000;
            }
        }

        private void ThrottledMusicFilesCollectionChanged()
        {
            convertToMp3AllCommand.RaiseCanExecuteChanged();
            convertToMp3SelectedCommand.RaiseCanExecuteChanged();
        }

        private void SelectedMusicFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            convertToMp3SelectedCommand.RaiseCanExecuteChanged();
        }

        private void SelectedTranscodeItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateCancelCommands();
        }

        private void UpdateCancelCommands()
        {
            cancelAllCommand.RaiseCanExecuteChanged();
            cancelSelectedCommand.RaiseCanExecuteChanged();
        }
    }
}
