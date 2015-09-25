using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ClientModel;
using Tasks;
using Tasks.Factorial;

namespace Client
{
    class Program
    {
        public static XmlSerializer Serializer;
        private static UdpClient client;
        private static UdpClient clientToSend;
        public static Dictionary<int, IPEndPoint> IpEndPoints;
        public static IPEndPoint ServerEndPoint;
        static void Main(string[] args)
        {
            int port;
            string input;
            Console.WriteLine("Введите порт, на который мне сесть");
            input = Console.ReadLine();
            port = Int32.Parse(input);
            client = new UdpClient(port);
            client.Client.ReceiveBufferSize = 200000;//ну а че тут поделаешь...
            client.Client.SendBufferSize = 200000;
            clientToSend = new UdpClient(port+1);
            clientToSend.Client.ReceiveBufferSize = 200000;
            clientToSend.Client.SendBufferSize = 200000;
            var anyIpEP = new IPEndPoint(IPAddress.Any, 0);
            Serializer = new XmlSerializer(typeof(Message),"nstu.ru");
            while (true)
            {
                Message message;
                var bytemessage = client.Receive(ref anyIpEP);
                var strmessage = Encoding.ASCII.GetString(bytemessage);
                using (TextReader reader = new StringReader(strmessage))
                {
                    message = (Message)(Serializer.Deserialize(reader));
                }
                switch (message.MessageType)
                {
                        case MessageType.ConnectionCheckFromServer:
                        Console.WriteLine("Connection check message from the server with ip " + anyIpEP.Address);
                        var messageToSend =
                            Encoding.ASCII.GetBytes(FormMessage(MessageType.ConnectionCheckFromClient, "",TaskType.Default,0,0));
                        clientToSend.Send(messageToSend, messageToSend.Length, anyIpEP.Address.ToString(),anyIpEP.Port+1);
                        break;

                    case MessageType.FromServerWithTask:
                        Console.WriteLine("Got a task from server. Task is "+message.Task.GetTaskName());
                        ServerEndPoint = anyIpEP;
                        ServerEndPoint.Port++;
                        Execute(message);
                        break;
                }
            }
        }




        static void Execute(Message message)
        {
            IParallelableTask parallelableTask;
            switch (message.Task)
            {
                    case TaskType.Factorial:
                    parallelableTask = new Factorial();
                    break;
                default:
                    parallelableTask = new Factorial();
                    break;
            }    
            parallelableTask.LoadTask(message.ToClient,message.NumberOfClients-1,message.Data);
            var ipd = new InternalPackageProvider();
            ipd.ProviderUdpClient = client;
            IpEndPoints = new Dictionary<int, IPEndPoint>();
            for (int i = 0; i < message.NumberOfClients; i++)
            {
                IpEndPoints.Add(i, new IPEndPoint(IPAddress.Parse(message.IpDictionary.First(p => p.Number == i).Ip), message.IpDictionary.First(p => p.Number == i).Port));
            }
            ipd.IpEndPoints = IpEndPoints;
            System.Threading.Thread thread = new Thread(ipd.GetData);
            parallelableTask.SetDataProvider(ipd);
            parallelableTask.SendEvent += SendToClient;
            parallelableTask.ReadyEvent += SendResultToServer;
            thread.Start();
            parallelableTask.Execute();
            thread.Abort();
        }

        private static void SendResultToServer(object sender, EventArgs e)
        {
            var pack = ((IParallelableTask) sender).PackageToSend;
            var message = FormMessage(MessageType.FromClientWithResult, pack.Data, TaskType.Default, pack.From, 0);
            var bytemes = Encoding.ASCII.GetBytes(message);

            if(bytemes.Length>65536)

            clientToSend.Send(bytemes, bytemes.Length, ServerEndPoint.Address.ToString(), ServerEndPoint.Port);
        }

        private static void SendToClient(object sender, EventArgs e)
        {
            var pack = ((IParallelableTask) sender).PackageToSend;
            var message = FormMessage(MessageType.FromClientToClient, pack.Data, TaskType.Default, pack.To, 0);
            var byteMessage = Encoding.ASCII.GetBytes(message);
            var ipep = IpEndPoints[pack.To];
            clientToSend.Connect(ipep);
            clientToSend.Send(byteMessage, byteMessage.Length);
        }

        public static string FormMessage(MessageType type, string data, TaskType task, int num, int clnum)
        {
            var message = new Message() { MessageType = type, Task = task,Data =data,ToClient = num};
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            Serializer.Serialize(writer, message);
            return builder.ToString();
        }
    }
}
