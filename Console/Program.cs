using DDnsSharp.Core;
using DDnsSharp.Core.Models;
using DDnsSharp.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

    class Program
    {
        static void Main(string[] args)
        {
            var ioc = DDnsSharpIoC.Current;
            ioc.Rebind<IServiceHelper>().To<TestServiceHelper>();

            tr = new TestRun();
            tr.Start();
            Console.WriteLine("Started");
            Console.ReadKey(true);
            tr.Stop();
            Console.WriteLine("Stopped");
            Console.ReadKey(true);
        }

        private static TestRun tr;
    }

    class TestRun
    {
        private const int REGULAR_INTERVAL = 3;
        private const int ERROR_INTERVAL = 15;

        public TestRun()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            System.IO.Directory.SetCurrentDirectory(path);
        }
        private Logger logger;

        private Timer timer;


        public void Start()
        {
            logger = LogManager.GetCurrentClassLogger();

            timer = new Timer(REGULAR_INTERVAL);
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            OnJob();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnJob();
        }

        private async void OnJob()
        {
            try
            {
                DDnsSharpRuntime.LoadAppConfig();
                await DDNS.Start(DDnsSharpRuntime.AppConfig.UpdateList);
                DDnsSharpRuntime.SaveAppConfig();
                if (timer.Interval > REGULAR_INTERVAL)
                    timer.Interval = REGULAR_INTERVAL;
            }
            catch (Exception ex)
            {
                logger.ErrorException("更新记录时出现意外错误", ex);
                timer.Interval = ERROR_INTERVAL;
            }
        }
    }

    class TestServiceHelper:IServiceHelper
    {

        public string BuildAPIUrl(string serviceName, string methodName)
        {
            return String.Format(Consts.API_FORMAT_STRING, Consts.API_BASE_URL, serviceName, methodName);
        }

        public async Task<string> AccessAPI(string url, string method = "GET", string requestData = null)
        {
            return await Task.Factory.StartNew<string>(() => "127.0.0.1");
        }

        public async Task<T> AccessAPI<T>(string serviceName, string methodName, RequestModelBase requestModel)
            where T : ReturnValueBase
        {
            var type = typeof(T);
            return await Task.Factory.StartNew<T>(() => Activator.CreateInstance<T>());
        }

        public async Task<T> AccessAPI<T>(string url, RequestModelBase requestModel)
            where T : ReturnValueBase
        {
            var type = typeof(T);
            return await Task.Factory.StartNew<T>(() => Activator.CreateInstance<T>());
        }
    }
