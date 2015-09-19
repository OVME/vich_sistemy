using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientModel
{
    public class Message
    {
        public MessageType MessageType { get; set; }
        public Task Task { get; set; }
        public string Data { get; set; }
    }
}
