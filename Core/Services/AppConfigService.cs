using DDnsSharp.Core.Models;
using DDnsSharp.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Threading;

namespace DDnsSharp.Core.Services
{
    public class AppConfigService
    {
        private const string MUTEX_NAME = "AppConfigService";
        private const string FILE_NAME = "config.txt";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AppConfigService()
        {
            try
            {
                syncMutex = Mutex.OpenExisting(MUTEX_NAME);
            }
            catch(WaitHandleCannotBeOpenedException)
            {
                syncMutex = new Mutex(false,MUTEX_NAME);
            }
        }

        private Mutex syncMutex;

        public void MakeBackup()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(dir, FILE_NAME);
            if(File.Exists(fullPath))
            {
                var str = File.ReadAllText(fullPath);
                if(!string.IsNullOrEmpty(str))
                {
                    File.Copy(fullPath, Path.Combine(dir, "config.bak"), true);
                }
            }
        }

        public AppConfig Read()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(dir,FILE_NAME);
            AppConfig result = null;
            string fileContent = null;
            syncMutex.WaitOne();
            if (File.Exists(fullPath))
            {
                try
                {
                    using (var fs = File.OpenRead(fullPath))
                    using (var sr = new StreamReader(fs))
                    {
                        fileContent = sr.ReadToEnd();
                    }

                }
                catch (IOException ex)
                {
                    logger.ErrorException("AppConfigService.IOException", ex);
                }
            }
            if(!string.IsNullOrEmpty(fileContent))
            {
                result = JsonConvert.DeserializeObject<AppConfig>(fileContent);
                result.Password = Encryption.Decrypt(result.Password);
            }
            else
            {
                result = new AppConfig();
            }
            syncMutex.ReleaseMutex();
            return result;
        }

        public void Save(AppConfig conf)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(dir, FILE_NAME);
            var nc = new AppConfig();
            nc.Email = conf.Email;
            nc.Password = Encryption.Encrypt(conf.Password);
            nc.UpdateList = conf.UpdateList;
            syncMutex.WaitOne();
            FileStream fs = null;
            try
            {
                if (File.Exists(fullPath))
                {
                    fs = File.Open(fullPath, FileMode.Truncate);
                }
                else
                {
                    fs = File.Create(fullPath);
                }
                fs.Close();
            }
            catch (IOException ex)
            {
                logger.ErrorException("AppConfigService.IOException", ex);
            }
            if(fs!=null)
            {
                using (var sw = new StreamWriter(fs))
                {
                    var confStr = JsonConvert.SerializeObject(nc);
                    sw.Write(confStr);
                }
                fs.Close();
            }
            syncMutex.ReleaseMutex();
        }
    }
}
