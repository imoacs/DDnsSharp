using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDnsSharp.Core.Services.IP
{
    public class IPServiceSohu : IIPService
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public virtual string ServiceUrl { get { return "http://pv.sohu.com/cityjson?ie=utf-8"; } }

        public virtual async Task<string> GetIP()
        {
            var content = await ServiceHelper.AccessAPI(ServiceUrl);
            var match = Regex.Match(content, CommonService.IP_REGEX_PATTERN);
            if (match.Success)
                return match.Value;
            else
            {
                logger.Info(string.Format("无法有效的从 {0} 处获得公网IP", ServiceUrl));
                return CommonService.EMPTY_IP;
            }
        }
    }
}
