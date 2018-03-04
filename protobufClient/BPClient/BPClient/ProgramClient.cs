using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protobufClient
{
    class Program
    {
        static void Main(string[] args)
        {

            for (int i = 0; i < 2; i++)
            {
                new Thread(new ThreadStart(threadTestHeart)).Start();
            }
            Console.ReadKey();  

        }


        public static void threadTestHeart()
        {
            IPAddress ip = IPAddress.Parse("192.168.5.83");
            IPEndPoint ipEnd = new IPEndPoint(ip, 3800);
            Socket heartSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int socketCount = 0;
            heartSocket.Connect(ipEnd);
            while (socketCount < 3)
            {
                Dictionary<string, string> responseParam = new Dictionary<string, string>();
                responseParam["Thread"] = Thread.CurrentThread.GetHashCode() + "";
                responseParam["MessageNo"] = socketCount.ToString();
                string bodyStr = JsonConvert.SerializeObject(responseParam);
                byte[] body = Encoding.UTF8.GetBytes(bodyStr);
                string packStr = string.Format("HEAD{0:000}", body.Length) + bodyStr;
                heartSocket.Send(Encoding.UTF8.GetBytes(packStr));
                Console.WriteLine("Socket NO: " + Thread.CurrentThread.GetHashCode() + "  MessageNo " + socketCount);
                socketCount++;
                Thread.Sleep(500);
            }
            heartSocket.Close();
        } 
    }
}
