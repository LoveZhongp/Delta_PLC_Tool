using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Delta_PLC_Tool
{
    public class SocketClient
    {
        #region 构造函数

        /// <summary>
        ///     构造函数,连接服务器IP地址默认为本机127.0.0.1
        /// </summary>
        /// <param name="port">监听的端口</param>
        public SocketClient(int port)
        {
            _ip = "127.0.0.1";
            _port = port;
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口</param>
        public SocketClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        #endregion

        #region 内部成员

        private Socket _socket;
        private readonly string _ip = "";
        private readonly int _port;
        private bool _isRec = true;
        // 一次读取的最大字节数
        //private int maxLoadSize = 100;
        // 存放读取数据的容器
        private byte[] container = new byte[1024 * 1024];
        // 缓存字节，解决分包情况
        private byte[] bufferBytes = null;

        /// <summary>
        ///     判断服务器是否连通
        /// </summary>
        /// <returns></returns>
        public bool IsSocketConnected()
        {
            var part1 = _socket.Poll(1000, SelectMode.SelectRead);
            var part2 = _socket.Available == 0;
            if (part1 && part2)
                return false;
            return true;
        }

        /// <summary>
        ///     开始接受客户端消息
        /// </summary>
        public void StartRecMsg()
        {
            try
            {
                _socket.BeginReceive(container, 0, container.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        var length = _socket.EndReceive(asyncResult);
                        if (length > 0)
                        {
                            var recBytes = new byte[length];
                            Array.Copy(container, 0, recBytes, 0, length);

                            // 合并bufferBytes和recBytes的字符
                            byte[] newByte = null;
                            if (bufferBytes == null)
                            {
                                newByte = recBytes;
                            }
                            else
                            {
                                newByte = new byte[bufferBytes.Length + recBytes.Length];
                                Array.Copy(bufferBytes, 0, newByte, 0, bufferBytes.Length);
                                Array.Copy(recBytes, 0, newByte, bufferBytes.Length, recBytes.Length);

                            }
                            var msg = Encoding.UTF8.GetString(recBytes);
                            SysLog.Info("Socket Receive:" + msg);
                            // 如果读取的数据中最后的字符是换行符                         
                            //if (length > 2 && recBytes[0] == '#' && recBytes[length - 1] == '%')
                            
                            //处理消息
                            HandleRecMsg?.BeginInvoke(newByte, this, null, null);
                            bufferBytes = null;
                            

                        }
                        //马上进行下一轮接受，增加吞吐量
                        if (length > 0 && _isRec && IsSocketConnected())
                            StartRecMsg();

                    }
                    catch (Exception ex)
                    {
                        HandleException?.BeginInvoke(ex, null, null);
                        Close();
                        SysLog.Info("Socket 消息解析失败，");
                    }
                }, null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex, null, null);
                Close();
            }
        }

        #endregion

        #region 外部接口

        /// <summary>
        ///     开始服务，连接服务端
        /// </summary>
        public void StartClient()
        {
            try
            {
                _isRec = true;
                //实例化 套接字 （ip4寻址协议，流式传输，TCP协议）
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //创建 ip对象
                var address = IPAddress.Parse(_ip);
                //创建网络节点对象 包含 ip和port
                var endpoint = new IPEndPoint(address, _port);
                //将 监听套接字  绑定到 对应的IP和端口
                _socket.BeginConnect(endpoint, asyncResult =>
                {
                    try
                    {
                        _socket.EndConnect(asyncResult);
                        //开始接受服务器消息
                        StartRecMsg();

                        HandleClientStarted?.BeginInvoke(this, null, null);
                    }
                    catch (Exception ex)
                    {
                        HandleException?.BeginInvoke(ex, null, null);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex, null, null);
            }
        }

        /// <summary>
        ///     发送数据
        /// </summary>
        /// <param name="bytes">数据字节</param>
        public void Send(byte[] bytes)
        {
            try
            {
                _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        var length = _socket.EndSend(asyncResult);
                        HandleSendMsg?.BeginInvoke(bytes, this, null, null);
                    }
                    catch (Exception ex)
                    {
                        HandleException?.BeginInvoke(ex, null, null);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex, null, null);
                StartClient();
            }
        }

        /// <summary>
        ///     发送字符串（默认使用UTF-8编码）
        /// </summary>
        /// <param name="msgStr">字符串</param>
        public void Send(string msgStr)
        {
            Send(Encoding.UTF8.GetBytes(msgStr));
            SysLog.Info("Socket发送:" + msgStr);
            Console.WriteLine("CPC发送:" + msgStr);
        }

        /// <summary>
        ///     发送字符串（使用自定义编码）
        /// </summary>
        /// <param name="msgStr">字符串消息</param>
        /// <param name="encoding">使用的编码</param>
        public void Send(string msgStr, Encoding encoding)
        {
            Send(encoding.GetBytes(msgStr));
        }

        /// <summary>
        ///     传入自定义属性
        /// </summary>
        public object Property { get; set; }

        /// <summary>
        ///     关闭与服务器的连接
        /// </summary>
        public void Close()
        {
            try
            {
                _isRec = false;
                _socket.Disconnect(false);
                HandleClientClose?.BeginInvoke(this, null, null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex, null, null);
            }
            finally
            {
                _socket.Dispose();
                GC.Collect();
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        ///     客户端连接建立后回调
        /// </summary>
        public Action<SocketClient> HandleClientStarted { get; set; }

        /// <summary>
        ///     处理接受消息的委托
        /// </summary>
        public Action<byte[], SocketClient> HandleRecMsg { get; set; }

        /// <summary>
        ///     客户端连接发送消息后回调
        /// </summary>
        public Action<byte[], SocketClient> HandleSendMsg { get; set; }

        /// <summary>
        ///     客户端连接关闭后回调
        /// </summary>
        public Action<SocketClient> HandleClientClose { get; set; }

        /// <summary>
        ///     异常处理程序
        /// </summary>
        public Action<Exception> HandleException { get; set; }

        #endregion
    }
}
