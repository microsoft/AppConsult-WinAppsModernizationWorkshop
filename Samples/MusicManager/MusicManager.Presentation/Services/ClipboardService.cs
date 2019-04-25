using System.ComponentModel.Composition;
using System.Windows;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.Services
{
    [Export(typeof(IClipboardService))]
    internal class ClipboardService : IClipboardService
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
