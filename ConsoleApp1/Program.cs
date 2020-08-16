using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using CommunicationDevice;
using System.Net.Configuration;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            DeviceFactory com = new DeviceFactory(new SerialDevice("COM2", 9600));
            try
            {
                com.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"打开串口失败----{ex.Message}");
                return;
            }
            Console.WriteLine("打开串口成功！");
            while (true)
            {
                try
                {
                    com.Send("abc");
                    Console.WriteLine("发送信息----abc");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"发送错误----{ex.Message}");
                }
                string msg = null;
                try
                {
                    msg = com.Recieve();
                    if (msg != null)
                    {
                        Console.WriteLine("接收信息----" + msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"接收错误----{ex.Message}");
                }
                finally
                {
                    msg = null;
                }
                Thread.Sleep(1000);
            }
            

            //Device serialDevice = new Serial_Device(new SerailParam { PortName = "COM2", BaudRate = 9600, DataBits = 8, Parity = Parity.None, StopBits = StopBits.One, Timeout = 3000 });
            //serialDevice.Open();
            //while (true)
            //{
            //    string data = serialDevice.Read();
            //    if (data != null)
            //    {
            //        Console.WriteLine(data);
            //    }
            //    serialDevice.Write("abc/.,");
            //    Thread.Sleep(500);
            //}
            //NetWork_Device device = new NetWork_Device(new NetWorkParam() { IP = IPAddress.Parse("192.168.0.160"), IsServer = false, Port = 2002 });
            //NetWork_Device device2 = new NetWork_Device(new NetWorkParam() { IP = IPAddress.Parse("192.168.0.200"), IsServer = false, Port = 2000 });

            //try
            //{
            //    device.Open();
            //    device2.Open();
            //    Console.WriteLine("连接服务器成功！");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("连接服务器失败"+ex.Message);
            //    return;
            //}
            //while (true)
            //{

            //    string data = device.Read();
            //    string data2 = device2.Read();
            //    if (data!=null&&data2!=null)
            //    {
            //        Console.WriteLine(data+data2);
            //        data = null;
            //        data2 = null;
            //    }

            //    Thread.Sleep(1000);
            //}


        }

        
    }
}
