using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDnsSharp.Core.Services.IP
{
    public interface IIPService
    {
        Task<string> GetIP();

        string ServiceUrl { get; }
    }
}
