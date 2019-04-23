using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Waf;
using System.Waf.Applications;
using System.Waf.Presentation;
using System.Windows;
using System.Windows.Threading;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Presentation.Properties;
using Waf.MusicManager.Presentation.Services;

namespace Waf.MusicManager.Presentation
{
    public partial class App
    {
        private static readonly Tuple<string, LogLevel>[] logSettings =
        {
            Tuple.Create("App", LogLevel.Info),
            Tuple.Create("MusicManager.*", LogLevel.Warn),
        };

        private AggregateCatalog catalog;
        private CompositionContainer container;
        private IEnumerable<IModuleController> moduleControllers;
        
        public App()
        {
            Directory.CreateDirectory(EnvironmentService.ProfilePath);
            ProfileOptimization.SetProfileRoot(EnvironmentService.ProfilePath);
            ProfileOptimization.StartProfile("Startup.profile");

            var fileTarget = new FileTarget("fileTarget")
            {
                FileName = Path.Combine(EnvironmentService.LogPath, "App.log"),
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.ff} ${level} ${processid} ${logger} ${message}  ${exception}",
                ArchiveAboveSize = 1024 * 1024 * 5,  // 5 MB
                MaxArchiveFiles = 2,
            };
            var logConfig = new LoggingConfiguration();
            logConfig.DefaultCultureInfo = CultureInfo.InvariantCulture;
            logConfig.AddTarget(fileTarget);
            var maxLevel = LogLevel.AllLoggingLevels.Last();
            foreach (var logSetting in logSettings)
            {
                logConfig.AddRule(logSetting.Item2, maxLevel, fileTarget, logSetting.Item1);
            }
            LogManager.Configuration = logConfig;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.App.Info("{0} {1} is starting; OS: {2}", ApplicationInfo.ProductName, ApplicationInfo.Version, Environment.OSVersion);

#if !(DEBUG)
            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#endif

            InitializeCultures();

            if (Environment.OSVersion.Version < new Version(6, 3))
            {
                MessageBox.Show(Presentation.Properties.Resources.NewerWindowsRequired, ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            catalog = new AggregateCatalog();
            // Add the WpfApplicationFramework assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(WafConfiguration).Assembly));
            // Add the Waf.MusicManager.Applications assembly
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));
            // Add this assembly
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));

            container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);

            // Initialize all presentation services
            var presentationServices = container.GetExportedValues<IPresentationService>();
            foreach (var presentationService in presentationServices) { presentationService.Initialize(); }

            // Initialize and run all module controllers
            moduleControllers = container.GetExportedValues<IModuleController>();
            foreach (var moduleController in moduleControllers) { moduleController.Initialize(); }
            foreach (var moduleController in moduleControllers) { moduleController.Run(); }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Shutdown the module controllers in reverse order
            foreach (var moduleController in moduleControllers.Reverse()) { moduleController.Shutdown(); }

            // Wait until all registered tasks are finished
            var shellService = container.GetExportedValue<IShellService>();
            var tasksToWait = shellService.TasksToCompleteBeforeShutdown.ToArray();
            while (tasksToWait.Any(t => !t.IsCompleted))
            {
                DispatcherHelper.DoEvents();
            }
            
            // Dispose
            container.Dispose();
            catalog.Dispose();
            base.OnExit(e);
        }

        private static void InitializeCultures()
        {
            if (!string.IsNullOrEmpty(Settings.Default.Culture))
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Settings.Default.Culture);
            }
            if (!string.IsNullOrEmpty(Settings.Default.UICulture))
            {
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(Settings.Default.UICulture);
            }
        }

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, false);
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void HandleException(Exception e, bool isTerminating)
        {
            if (e == null) { return; }

            Log.App.Error(e, "Unknown application error.");

            if (!isTerminating)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Presentation.Properties.Resources.UnknownError, e.ToString()),
                    ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
