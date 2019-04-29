using System.ComponentModel.Composition;
using System.Windows;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IInfoView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class InfoWindow : IInfoView
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        public void ShowDialog(object owner)
        {
            Owner = owner as Window;
            ShowDialog();
        }
    }
}
