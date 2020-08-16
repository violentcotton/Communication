using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationDevice
{
    public interface IDevice
    {
        /// <summary>
        /// 打开设备
        /// </summary>
        void Open();
        /// <summary>
        /// 关闭设备
        /// </summary>
        void Close();
        /// <summary>
        /// 发送信息
        /// </summary>
        void Send(byte[] msg);
        /// <summary>
        /// 接收信息
        /// </summary>
        string Recieve();
    }
    public class DeviceFactory
    {
        private IDevice _device;
        public DeviceFactory(IDevice device)
        {
            this._device = device;
        }
        public void Open()
        {
            try
            {
                _device.Open();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public void Close()
        {
            try
            {
                _device.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public void Send(string msg)
        {
            try
            {
                _device.Send(Encoding.UTF8.GetBytes(msg));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public string Recieve()
        {
            try
            {
                return _device.Recieve();
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

    }
    public abstract class TCPDevice : IDevice
    {
        /// <summary>
        /// Socket字段
        /// </summary>
        protected Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// <summary>
        /// 终节点
        /// </summary>
        protected IPEndPoint ipEndPoint;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">服务端IP地址</param>
        /// <param name="port">端口号</param>
        public TCPDevice(string ip, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        protected string retMessage = null;
        /// <summary>
        /// 关闭串口
        /// </summary>
        public abstract void Close();
        public abstract void Open();

        public string Recieve()
        {
            string msg = retMessage;
            retMessage = null;
            return msg;
        }

        public abstract void Send(byte[] msg);
        
    }
    public class TcpClientDevice : TCPDevice
    {
        public TcpClientDevice(string ip,int port)
            :base(ip,port)
        {

        }
        
        /// <summary>
        /// 断开连接
        /// </summary>
        public override void Close()
        {
            if (!socket.Connected)
            {
                return;
            }
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                socket.Close();
            }
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        public override void Open()
        {
            if (socket.Connected)
            {
                Close();
            }
            try
            {
                socket.Connect(ipEndPoint);
            }
            catch (Exception ex)
            {
                throw new Exception("连接服务器错误:" + ex.Message);
            }

        }
        private void ReceiveMsg(Socket socket)
        {
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024];
                int i = -1;
                try
                {
                    i = socket.Receive(buffer);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (i > 0)
                {
                    retMessage+= Encoding.UTF8.GetString(buffer, 0, i);
                }
            }
        }
        public override void Send(byte[] msg)
        {
            if (!socket.Connected)
            {
                try
                {
                    Open();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            try
            {
                socket.Send(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public class TcpServerDevice : TCPDevice
    {
        /// <summary>
        /// 已连接的Socket
        /// </summary>
        private Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();
        /// <summary>
        /// 当前已连接Socket个数
        /// </summary>
        public int ConnectingNum { get; set; } = 0;
        public TcpServerDevice(string ip, int port)
            :base(ip,port)
        {
        }
        public override void Close()
        {
            if (!socket.Connected)
            {
                return;
            }
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                socket.Close();
            }
        }

        public override void Open()
        {
            try
            {
                socket.Bind(ipEndPoint);
            }
            catch (Exception ex)
            {
                throw new Exception("服务器开启失败:" + ex.Message);
            }
            socket.Listen(10);
            Task.Run(new Action(() =>
            {
                ListenConnection();
            }));
        }
        private void ListenConnection()
        {
            while (true)
            {
                //socket.Accept()带阻塞
                Socket serverSocket = socket.Accept();         
                dicSocket.Add(serverSocket.RemoteEndPoint.ToString(), serverSocket);
                ConnectingNum += 1;
                Task.Run(() => ReceiveMsg(serverSocket));  //一旦新的客户端连上就开启一个线程接收数据
            }
        }
        private void ReceiveMsg(Socket socket)
        {
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024];
                int length = -1;
                try
                {
                    length = socket.Receive(buffer);
                }
                catch (Exception)
                {
                    dicSocket.Remove(socket.RemoteEndPoint.ToString());
                    ConnectingNum -= 1;
                    break;
                }

                if (length > 0)
                {
                    retMessage += socket.RemoteEndPoint.ToString() + ":" + Encoding.UTF8.GetString(buffer, 0, length) + ";";
                }
            }
        }
        /// <summary>
        /// 向所有已连接的客户机发送消息
        /// </summary>
        /// <param name="msg"></param>
        public override void Send(byte[] msg)
        {
            if (dicSocket.Count > 0)
            {
                foreach (KeyValuePair<string, Socket> dic in dicSocket)
                {
                    dic.Value.Send(msg);
                }
            }
        }
    }
    public class SerialDevice : IDevice
    {
        private SerialPort _serialPort;
        public SerialDevice(string port,int baudRate, Parity parity=Parity.None,
            int dataBits=8,StopBits stopBits=StopBits.One)
        {
            _serialPort = new SerialPort(port, baudRate, parity, dataBits, stopBits);
        }
        public void Close()
        {
            try
            {
                _serialPort.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Open()
        {
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("串口打开失败:" + ex.Message);
            }
        }

        public string Recieve()
        {
            try
            {
                int i = _serialPort.BytesToRead;
                if (i<=0)
                {
                    return null;
                }
                Byte[] readBuffer = new Byte[i];
                _serialPort.Read(readBuffer, 0, i);
                return Encoding.Default.GetString(readBuffer);
            }
            catch (Exception ex)
            {
                throw new Exception("串口读取错误" + ex.Message);
            }
        }

        public void Send(byte[] msg)
        {
            try
            {
                _serialPort.Write(msg, 0, msg.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
