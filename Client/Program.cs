using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ClientModel;

namespace Client
{
    class Program
    {
        public static XmlSerializer Serializer;
        static void Main(string[] args)
        {
            int port;
            string input;
            Console.WriteLine("Введите порт, на который мне сесть");
            input = Console.ReadLine();
            port = Int32.Parse(input);
            UdpClient client = new UdpClient(port);
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
                            Encoding.ASCII.GetBytes(FormMessage(MessageType.ConnectionCheckFromClient, ""));
                        client.Send(messageToSend, messageToSend.Length, anyIpEP.Address.ToString(),anyIpEP.Port+1);
                        break;
                }
            }
        }

        static void Execute()
        {
            
        }

        public static string FormMessage(MessageType type, string data)
        {
            var message = new Message() { MessageType = type };
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            Serializer.Serialize(writer, message);
            return builder.ToString();
        }
    }
}
