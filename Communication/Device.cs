using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public abstract class Device
    {
        public object Param { get; set; }
        public bool IsConnect { get; set; }
        public string DeviceName { get; set; }
        public abstract void Open();
        public abstract void Close();
        public abstract string Read();
        public abstract void Write(string data);
        
    }
}
