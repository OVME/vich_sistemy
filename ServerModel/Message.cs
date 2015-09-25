using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerModel
{
    public class Message
    {
        public MessageType MessageType { get; set; }
        //public TaskType Task { get; set; }
        public string Data { get; set; }
        public int ToClient { get; set; }
        public int NumberOfClients { get; set; }
        //public Dictionary<int, string> IpDictionary { get; set; } 
    }
}
