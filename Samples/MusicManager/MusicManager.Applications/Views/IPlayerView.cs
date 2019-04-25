using System;
using System.Waf.Applications;

namespace Waf.MusicManager.Applications.Views
{
    public interface IPlayerView : IView
    {
        TimeSpan GetPosition();

        void SetPosition(TimeSpan position);
    }
}
