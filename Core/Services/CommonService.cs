using DDnsSharp.Core.Models;
using DDnsSharp.Core.Services.IP;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDnsSharp.Core.Services
{
    public static class CommonService
    {
        public const string EMPTY_IP = "0.0.0.0";
        public const string IP_REGEX_PATTERN = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}";
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static async Task<ReturnValueBase> GetAPIVersion()
        {
            var requestModel = DDnsSharpRuntime.NewRequestModel<RequestModelBase>();
            var url = ServiceHelper.Current.BuildAPIUrl("Info", "Version");
            return await ServiceHelper.Current.AccessAPI<ReturnValueBase>(url, requestModel);
        }

        public static async Task<UserInfoReturnValue> GetUserInfo()
        {
            var requestModel = DDnsSharpRuntime.NewRequestModel<RequestModelBase>();
            var url = ServiceHelper.Current.BuildAPIUrl("User", "Detail");
            return await ServiceHelper.Current.AccessAPI<UserInfoReturnValue>(url, requestModel);
        }

        private static IIPService[] ipServices;
        private static int serviceIndex = 0;

        public static async Task<string> GetCurrentIP()
        {
            if (ipServices == null)
            {
                GetIPServices();
            }
            if (ipServices.Length == 0)
                return EMPTY_IP;

            var ip = EMPTY_IP;
            var service = ipServices[serviceIndex];
            serviceIndex++;
            if (serviceIndex > ipServices.Length - 1)
                serviceIndex = 0;

            ip = await service.GetIP();

            return ip;
        }

        private static void GetIPServices()
        {
            var ifIPSerivce = typeof(IIPService);
            var types = Assembly.Load("DDnsSharp.Core").GetTypes();
            types = types.Where(t => t.IsClass && ifIPSerivce.IsAssignableFrom(t)).ToArray();
            List<IIPService> result = new List<IIPService>();
            foreach (var t in types)
            {
                try
                {
                    var service = Assembly.GetExecutingAssembly().CreateInstance(t.FullName);
                    result.Add((IIPService)service);
                }
                catch
                {
                    logger.Error("无法实例化IPService类:" + t.FullName);
                }
            }
            ipServices = result.ToArray();
            if (ipServices.Length <= 0)
                logger.Error("无法找到有效的IPService.");
        }
    }
}
