using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Communication
{
    public class Serial_Device : Device
    {
        private SerialPort serialPort = new SerialPort();
        private string readData = null;
        private string tempData = null;

        public Serial_Device(SerailParam serialParam)
        {
            serialPort.PortName = serialParam.PortName;
            serialPort.BaudRate = serialParam.BaudRate;
            serialPort.Parity = serialParam.Parity;
            serialPort.DataBits = serialParam.DataBits;
            serialPort.StopBits = serialParam.StopBits;
            serialPort.ReadTimeout = serialParam.Timeout;
            serialPort.WriteTimeout = serialParam.Timeout;
        }
        public override void Close()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("串口关闭失败:"+ex.Message);
            }
        }

        public override void Open()
        {
            try
            {
                serialPort.Open();
                serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialReceived);
            }
            catch (Exception ex)
            {
                throw new Exception("串口打开失败:"+ex.Message);
            }
        }
        private void SerialReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int len = serialPort.BytesToRead;
                Byte[] readBuffer = new Byte[len];
                serialPort.Read(readBuffer, 0, len);
                tempData = Encoding.Default.GetString(readBuffer);
            }
            catch (Exception ex)
            {
                throw new Exception("串口读取错误"+ex.Message);
            }
        }

        public override string Read()
        {
            if (!serialPort.IsOpen)
                throw new Exception("串口没有打开！");
            readData = tempData;
            tempData = null;
            return readData;
        }

        public override void Write(string writeData)
        {
            if (!serialPort.IsOpen)
                throw new Exception("串口没有打开！");
            serialPort.Write(writeData);
        }
    }
}
