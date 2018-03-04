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
using ProtoBuf;
using System;
using System.IO;
namespace SocketClient
{

    //添加特性，表示可以被ProtoBuf工具序列化
    [ProtoContract]
    public class NetModel
    {
        //添加特性，表示该字段可以被序列化，1可以理解为下标
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public string Commit;
        [ProtoMember(3)]
        public string Message;
    }

    [ProtoContract]
    public class TestDataTable
    {
        [ProtoMember(1)]
        public NetModel[] Data = new NetModel[0];
    }  
  
    public partial class ClientMain 
    {
        //创建 1个客户端套接字 和1个负责监听服务端请求的线程  
        static Thread threadclient = null;
        static Thread threadclientSend = null;
        static Socket socketclient = null;

       
                   

        public static void Main(string[] args)
        {

            // Serialize<TestDataTable>();

            NetModel item = new NetModel() { ID = 1, Commit = "LanOu", Message = "Unity" };
            NetModel item2 = new NetModel() { ID = 2, Commit = "2f", Message = "2msg" };
            TestDataTable tb = new TestDataTable();
            NetModel[] items = { item, item2 };
            tb.Data = items;

             byte[] dtb= Serialize<TestDataTable>(tb);

             TestDataTable tb2 = new TestDataTable();
             tb2 = DeSerialize<TestDataTable>(dtb, dtb.Length);
            //定义一个套接字监听  
            socketclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取文本框中的IP地址  
            IPAddress address = IPAddress.Parse("127.0.0.1");
            //将获取的IP地址和端口号绑定在网络节点上  
            IPEndPoint point = new IPEndPoint(address, 8098);
            //客户端套接字连接到网络节点上，用的是Connect  
            socketclient.Connect(point);
            //线程
            threadclient = new Thread(recvMsg);
            threadclient.IsBackground = true;
            threadclient.Start();
       
           //不停的给服务器发送数据
         
            while (true)
            {
                sendMsg();
            }
            
        }


        // 接收服务端发来信息的方法    
        static void recvMsg()
        {        
            //持续监听服务端发来的消息 
            while (true)
            {
                try
                {
                    //定义一个1M的内存缓冲区，用于临时性存储接收到的消息  
                    byte[] arrRecvmsg = new byte[1024];
                    //将客户端套接字接收到的数据存入内存缓冲区，并获取长度  
                    int length = socketclient.Receive(arrRecvmsg);
                    NetModel netModel = DeSerialize<NetModel>(arrRecvmsg,length);
                    //接受的消息 
                    Console.WriteLine("服务器:" + GetCurrentTime() + "\r\n" + netModel.Message + "\r\n\n");                  
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("远程服务器已经中断连接" + "\r\n\n");                
                    break;
                }
            }
        }


         static  void sendMsg()
        {
            //发送的消息               
                  try
                  {
                      Console.ReadKey();
                      NetModel item = new NetModel() { ID = 1, Commit = "LanOu", Message = "Unity" };
                      //序列化对象
                      byte[] temp = Serialize(item);
                      //ProtoBuf的优势一：小
                      Console.WriteLine(temp.Length);
                      //反序列化为对象
                      NetModel result = DeSerialize<NetModel>(temp,temp.Length);
                      Console.WriteLine(result.Message);


                      //调用客户端套接字发送字节数组     
                      socketclient.Send(temp);
                      //将发送的信息追加到聊天内容文本框中     
                      Debug.WriteLine("hello...." + ": " + GetCurrentTime() + "\r\n"  + "\r\n\n");
                      //调用ClientSendMsg方法 将文本框中输入的信息发送给服务端     
                  }
                  catch (Exception ex)
                  {
                      Debug.WriteLine("远程服务器已经中断连接" + "\r\n\n");


                  }
            
        }


         //***********************************************************      
         /// 获取当前系统时间的方法    
         /// 当前时间     
         static DateTime GetCurrentTime()
         {
             DateTime currentTime = new DateTime();
             currentTime = DateTime.Now;
             return currentTime;
         }
         // 将消息序列化为二进制的方法
         // < param name="model">要序列化的对象< /param>
         private static byte[] Serialize<T>(T model)
         {
             try
             {
                 //涉及格式转换，需要用到流，将二进制序列化到流中
                 using (MemoryStream ms = new MemoryStream())
                 {
                     //使用ProtoBuf工具的序列化方法
                     ProtoBuf.Serializer.Serialize<T>(ms, model);
                     //定义二级制数组，保存序列化后的结果
                     byte[] result = new byte[ms.Length];
                     //将流的位置设为0，起始点
                     ms.Position = 0;
                     //将流中的内容读取到二进制数组中
                     ms.Read(result, 0, result.Length);
                     return result;
                 }
             }
             catch (Exception ex)
             {
                 Console.WriteLine("序列化失败: " + ex.ToString());
                 return null;
             }
         }

         // 将收到的消息反序列化成对象
         // < returns>The serialize.< /returns>
         // < param name="msg">收到的消息.</param>
         private static T DeSerialize<T>(byte[] msg, int length)
         {
             try
             {
                 using (MemoryStream ms = new MemoryStream())
                 {
                     //将消息写入流中
                     ms.Write(msg, 0, length);
                     //将流的位置归0
                     ms.Position = 0;
                     //使用工具反序列化对象
                     T result = ProtoBuf.Serializer.Deserialize<T>(ms);
                     return result;
                 }
             }
             catch (Exception ex)
             {
                 Console.WriteLine("反序列化失败: " + ex.ToString());
                 return default(T);
             }
         }

        //***********************************************************      

    }
}