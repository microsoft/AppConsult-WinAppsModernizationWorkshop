using System.Runtime.Serialization;
using System.Waf.Applications.Services;

namespace Waf.MusicManager.Applications.Properties
{
    [DataContract]
    public sealed class AppSettings : UserSettingsBase
    {
        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public bool IsMaximized { get; set; }

        [DataMember]
        public string CurrentPath { get; set; }

        [DataMember]
        public double Volume { get; set; }

        [DataMember]
        public bool Shuffle { get; set; }

        [DataMember]
        public bool Repeat { get; set; }

        protected override void SetDefaultValues()
        {
            Volume = 0.5;
        }
    }
}
