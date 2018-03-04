using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace SocketClient
{
    public partial class ClientMain 
    {
        //创建 1个客户端套接字 和1个负责监听服务端请求的线程  
        static Thread threadclient = null;
        static Socket socketclient = null;

        public static void Main(string[] args)
        {
            //定义一个套接字监听  
            socketclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取文本框中的IP地址  
            IPAddress address = IPAddress.Parse("127.0.0.1");
            //将获取的IP地址和端口号绑定在网络节点上  
            IPEndPoint point = new IPEndPoint(address, 8098);
            //客户端套接字连接到网络节点上，用的是Connect  
            socketclient.Connect(point);
            //线程
            threadclient = new Thread(recv);
            threadclient.IsBackground = true;
            threadclient.Start();
          
      
        }

     
        // 接收服务端发来信息的方法    
        static void recv()
        {
            int x = 0;
            //持续监听服务端发来的消息 
            while (true)
            {
                try
                {
                    //定义一个1M的内存缓冲区，用于临时性存储接收到的消息  
                    byte[] arrRecvmsg = new byte[1024 * 1024];

                    //将客户端套接字接收到的数据存入内存缓冲区，并获取长度  
                    int length = socketclient.Receive(arrRecvmsg);

                    //将套接字获取到的字符数组转换为人可以看懂的字符串  
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);
                    if (x == 1)
                    {
                        
                        Debug.WriteLine("服务器:" + GetCurrentTime() + "\r\n" + strRevMsg + "\r\n\n");
                    }
                    else
                    {
                       
                        Debug.WriteLine(strRevMsg + "\r\n\n");
                        x = 1;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("远程服务器已经中断连接" + "\r\n\n");
                  
                    break;
                }



            
            }
        }

        //获取当前系统时间  
        static DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }



        void send(byte[] msg) {
            try
            {
                //调用客户端套接字发送字节数组     
                socketclient.Send(msg);
                //将发送的信息追加到聊天内容文本框中     
                Debug.WriteLine("hello...." + ": " + GetCurrentTime() + "\r\n" + msg.ToString() + "\r\n\n");
                //调用ClientSendMsg方法 将文本框中输入的信息发送给服务端     
            }
            catch (Exception ex)
            {
                Debug.WriteLine("远程服务器已经中断连接" + "\r\n\n");

               
            }
        }


    }
}