using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SampleInfoViewModel : InfoViewModel
    {
        public SampleInfoViewModel() : base(new MockInfoView())
        {
        }

        private class MockInfoView : MockView, IInfoView
        {
            public void ShowDialog(object owner)
            {
            }
        }
    }
}
