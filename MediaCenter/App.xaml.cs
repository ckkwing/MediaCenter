using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utilities.Settings;

namespace MediaCenter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DBHelper.InitDBProfile();
            Core.Instance.Init();
            DataManager.Instance.Init();
            JobManager.Instance.Init();

            ResourceDictionary lanRes = LanguageSetting.LoadLanguageResource();
            Application.Current.Resources.MergedDictionaries.Add(lanRes);

            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            JobManager.Instance.Uninit();
            DataManager.Instance.Uninit();
            Core.Instance.Uninit();
            base.OnExit(e);
        }
    }
}
