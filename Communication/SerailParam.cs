using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Communication
{
    public class SerailParam
    {
        public string PortName { get; set; }
        public int BaudRate  { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public int Timeout { get; set; }
    }
}
