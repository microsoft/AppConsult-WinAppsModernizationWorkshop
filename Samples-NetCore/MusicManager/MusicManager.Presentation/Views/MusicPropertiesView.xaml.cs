using System.ComponentModel.Composition;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IMusicPropertiesView))]
    public partial class MusicPropertiesView : IMusicPropertiesView
    {
        public MusicPropertiesView()
        {
            InitializeComponent();
        }
    }
}
