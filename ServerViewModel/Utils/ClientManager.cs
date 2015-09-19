using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ServerViewModel.Utils
{
    public class ClientManager
    {
        private string _clientIp;
        private int _clientPort;
        private IPEndPoint _clientEndPoint;

        public string ClientIp
        {
            get { return _clientIp; }
        }

        public int ClientPort
        {
            get { return _clientPort; }
        }
        public UdpClient SenderUdpClient { get; set; }
        public UdpClient RecieverUdpClient { get; set; }
        public string RecievedData { get; set; }
        public event EventHandler MessageRecievedEvent;
        public ClientManager(int sendPortNumber, int recievePortNumber, string clientIp, int clientPort)
        {
            SenderUdpClient = new UdpClient(sendPortNumber);
            RecieverUdpClient = new UdpClient(recievePortNumber);

            _clientIp = clientIp;
            _clientPort = clientPort;
            try
            {
                _clientEndPoint = new IPEndPoint(IPAddress.Parse(clientIp), clientPort);
            }
            catch (Exception e)
            {
                return;
            }
            
        }

        public void ConnectToClient()
        {
            SenderUdpClient.Connect(_clientIp, _clientPort);
            RecieverUdpClient.Connect(_clientIp, _clientPort);
        }

        public void SendData(string data)
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(data);
            SenderUdpClient.Send(bytes, bytes.Length);
        }

        public void Recieve()
        {
            while (true)
            {
                var bytes = RecieverUdpClient.Receive(ref _clientEndPoint);
                RecievedData = Encoding.ASCII.GetString(bytes);
                MessageRecieved();
            }
            
        }

        public void MessageRecieved()
        {
            var handler = MessageRecievedEvent;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        
    }
}
