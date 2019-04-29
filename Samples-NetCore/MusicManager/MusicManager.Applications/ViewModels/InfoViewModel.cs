using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class InfoViewModel : ViewModel<IInfoView>
    {
        [ImportingConstructor]
        public InfoViewModel(IInfoView view)
            : base(view)
        {
            ShowWebsiteCommand = new DelegateCommand(ShowWebsite);
        }

        public ICommand ShowWebsiteCommand { get; }

        public string ProductName => ApplicationInfo.ProductName;

        public string Version => ApplicationInfo.Version;

        public string OSVersion => Environment.OSVersion.ToString();

        public string NetVersion => Environment.Version.ToString();

        public bool Is64BitProcess => Environment.Is64BitProcess;

        public void ShowDialog(object owner)
        {
            ViewCore.ShowDialog(owner);
        }

        private void ShowWebsite(object parameter)
        {
            string url = (string)parameter;
            try
            {
                Process.Start(url);
            }
            catch (Exception e)
            {
                Log.Default.Error(e, "An exception occured when trying to show the url '{0}'.", url);
            }
        }
    }
}
