using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Waf.Applications.Services;

namespace Waf.MusicManager.Applications.Properties
{
    [DataContract]
    public sealed class PlaylistSettings : UserSettingsBase
    {
        [DataMember(Name = "FileNames")] 
        private readonly List<string> fileNames = new List<string>();

        [DataMember]
        public string LastPlayedFileName { get; set; }

        [DataMember]
        public TimeSpan LastPlayedFilePosition { get; set; }
        
        public IReadOnlyList<string> FileNames { get { return fileNames; } }

        public void ReplaceAll(IEnumerable<string> newFileNames)
        {
            fileNames.Clear();
            fileNames.AddRange(newFileNames);
        }

        protected override void SetDefaultValues()
        {            
        }
    }
}
