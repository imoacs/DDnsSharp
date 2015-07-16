using DDnsSharp.Core.Models;
using DDnsSharp.Core.Services;
using System;

namespace DDnsSharp.Core
{
    public class DDnsSharpRuntime
    {
        static DDnsSharpRuntime()
        {
            AppConfig = new AppConfig();
        }

        private static AppConfigService cfgSvc = new AppConfigService();
        public static AppConfig AppConfig { get; private set; }

        public static void MakeBackup()
        {
            cfgSvc.MakeBackup();
        }

        public static void LoadAppConfig()
        {
            AppConfig = cfgSvc.Read();
        }

        public static void SaveAppConfig()
        {
            if (AppConfig != null)
            {
                cfgSvc.Save(AppConfig);
                if (ConfigSaved != null)
                    ConfigSaved(AppConfig, EventArgs.Empty);
            }
        }

        public static event EventHandler ConfigSaved;

        public static T NewRequestModel<T>()
            where T : RequestModelBase
        {
            var c = DDnsSharpRuntime.AppConfig;
            var instance = Activator.CreateInstance<T>();
            instance.LoginEmail = c.Email;
            instance.LoginPassword = c.Password;
            return instance;
        }
    }
}
