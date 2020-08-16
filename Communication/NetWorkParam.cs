using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class NetWorkParam
    {
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public bool IsServer { get; set; }

    }
}
