using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Communication
{
    public class NetWork_Device : Device
    {
        /// <summary>
        /// 编码格式
        /// </summary>
        Encoding encoding = Encoding.Default;
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// IP和端口号
        /// </summary>
        private IPEndPoint ipEndPoint;
        /// <summary>
        /// Read方法返回的数据
        /// </summary>
        private string data = null;
        /// <summary>
        /// 数据缓冲区接收的数据
        /// </summary>
        private string tempData = null;
        /// <summary>
        /// 已连接的Socket
        /// </summary>
        private Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>(); 
        /// <summary>
        /// 是做服务器还是客户端
        /// </summary>
        public bool IsServer { get; set; }
        /// <summary>
        /// 当前已连接Socket个数
        /// </summary>
        public int ConnectingNum { get; set; } = 0;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="param"></param>
        public NetWork_Device(NetWorkParam param)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipEndPoint = new IPEndPoint(param.IP, param.Port);
            IsServer = param.IsServer;
            
        }
        
        /// <summary>
        /// 打开服务器或者是客户端
        /// </summary>
        public override void Open()
        {
            if (IsServer)
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
            else
            {
                try
                {
                    socket.Connect(ipEndPoint);
                    dicSocket.Add(socket.RemoteEndPoint.ToString(),socket);
                    ConnectingNum += 1;
                }
                catch (Exception ex)
                {
                    throw new Exception("连接服务器错误:"+ex.Message);
                }
                Task.Run(() => ReceiveMsg(socket));
            }
        }
        
        /// <summary>
        /// 服务器侦听
        /// </summary>
        private void ListenConnection()
        {
            while (true)
            {
                Socket serverSocket = socket.Accept();          //socket.Accept()带阻塞
                dicSocket.Add(serverSocket.RemoteEndPoint.ToString(), serverSocket);
                ConnectingNum += 1;
                Task.Run(()=>ReceiveMsg(serverSocket));  //一旦新的客户端连上就开启一个线程接收数据
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
                
                if (length>0)
                {
                    tempData = encoding.GetString(buffer, 0, length);
                }
            }
        }

        public override string Read()
        {
            data = tempData;
            tempData = null;
            return data;
        }

        public override void Write(string data)
        {
            if (dicSocket.Count > 0)
            {
                foreach (KeyValuePair<string, Socket> dic in dicSocket)
                {
                    dic.Value.Send(encoding.GetBytes(data));
                }
            }
        }
        /// <summary>
        /// 关闭Socket
        /// </summary>
        public override void Close()
        {
            socket.Close();
        }
        
    }
}
