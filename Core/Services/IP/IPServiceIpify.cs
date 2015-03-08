using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDnsSharp.Core.Services.IP
{
    public class IPServiceIpify : IPServiceTelize
    {
        public override string ServiceUrl { get { return "http://api.ipify.org"; } }
    }
}
