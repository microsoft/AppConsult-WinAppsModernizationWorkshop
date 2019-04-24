using System.Waf.Applications;

namespace Waf.MusicManager.Applications.Views
{
    public interface IInfoView : IView
    {
        void ShowDialog(object owner);
    }
}
