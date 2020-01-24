using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public class PingResponse
    {   
        public string Url_Pinged { get; set; }
        public string Status { get; set; }
        public long LatencyMS { get; set; }
    }
}
