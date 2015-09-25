using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace ClientModel
{
    public class Message
    {
        public MessageType MessageType { get; set; }
        public TaskType Task { get; set; }
        public string Data { get; set; }
        public int ToClient { get; set; }
        public int NumberOfClients { get; set; }
        public List<NumberIpPort> IpDictionary { get; set; } 
    }

    public class NumberIpPort
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int Number { get; set; }
    }
}
