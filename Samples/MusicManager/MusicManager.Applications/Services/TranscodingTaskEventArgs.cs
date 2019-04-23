using System;
using System.Threading.Tasks;

namespace Waf.MusicManager.Applications.Services
{
    public class TranscodingTaskEventArgs : EventArgs
    {
        public TranscodingTaskEventArgs(string fileName, Task transcodingTask)
        {
            FileName = fileName;
            TranscodingTask = transcodingTask;
        }

        public string FileName { get; }

        public Task TranscodingTask { get; }
    }
}
