using System;
using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Transcoding
{
    public class TranscodeItem : Model
    {
        private TranscodeStatus transcodeStatus;
        private double progress;
        private Exception error;

        public TranscodeItem(MusicFile source, string destinationFileName)
        {
            Source = source;
            DestinationFileName = destinationFileName;
            UpdateStatus();
        }

        public MusicFile Source { get; }

        public string DestinationFileName { get; }

        public TranscodeStatus TranscodeStatus
        {
            get => transcodeStatus;
            private set => SetProperty(ref transcodeStatus, value);
        }

        public double Progress
        {
            get => progress;
            set
            {
                if (SetProperty(ref progress, value))
                {
                    UpdateStatus();
                }
            }
        }

        public Exception Error
        {
            get => error;
            set
            {
                if (SetProperty(ref error, value))
                {
                    UpdateStatus();
                }
            }
        }

        private void UpdateStatus()
        {
            if (Error != null) 
            { 
                TranscodeStatus = TranscodeStatus.Error; 
            }
            else if (Progress == 0) 
            { 
                TranscodeStatus = TranscodeStatus.Pending; 
            }
            else if (Progress < 1) 
            { 
                TranscodeStatus = TranscodeStatus.InProgress; 
            }
            else 
            { 
                TranscodeStatus = TranscodeStatus.Completed; 
            }
        }
    }
}
