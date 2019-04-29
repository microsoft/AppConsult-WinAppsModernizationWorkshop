using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;

namespace Waf.MusicManager.Applications.Data
{
    [Export, Export(typeof(ITranscoder)), PartMetadata(UnitTestMetadata.Name, UnitTestMetadata.Data)]
    internal class Transcoder : ITranscoder
    {
        public async Task TranscodeAsync(string sourceFileName, string destinationFileName, uint bitrate, CancellationToken cancellationToken, IProgress<double> progress)
        {
            var transcoder = new MediaTranscoder();
            var sourceFile = await StorageFile.GetFileFromPathAsync(sourceFileName);
            var destinationFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(destinationFileName));
            var destinationFile = await destinationFolder.CreateFileAsync(Path.GetFileName(destinationFileName));

            var profile = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High);
            profile.Audio.Bitrate = bitrate;

            var preparedTranscodeResult = await transcoder.PrepareFileTranscodeAsync(sourceFile, destinationFile, profile);

            Exception error = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (preparedTranscodeResult.CanTranscode)
                {
                    await preparedTranscodeResult.TranscodeAsync().AsTask(cancellationToken, progress);
                }
                else
                {
                    throw new InvalidOperationException("Reason: " + preparedTranscodeResult.FailureReason);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                await destinationFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                throw error;
            }
        }
    }
}
