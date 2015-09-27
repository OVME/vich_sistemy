using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClientModel;


namespace Tasks
{
    public class InternalPackageProvider
    {
        public XmlSerializer Serializer;
        public InternalPackage Package { get; set; }
        public UdpClient ProviderUdpClient { get; set; }
        public Dictionary<int, IPEndPoint> IpEndPoints { get; set; }

        public InternalPackageProvider()
        {
            Serializer = new XmlSerializer(typeof(Message), "nstu.ru");
            Package = null;
        }

        public void GetData()
        {
            while (true)
            {
                var ep = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes = { };
                byte[] rec = { };
                var strrec = "";
                while (strrec != "000")
                {
                    rec = ProviderUdpClient.Receive(ref ep);
                    strrec = Encoding.ASCII.GetString(rec);
                    if (strrec != "000")
                    {
                        bytes = bytes.Concat(rec).ToArray();
                    }

                }

                var strData = Encoding.ASCII.GetString(bytes);
                Message message;
                using (TextReader reader = new StringReader(strData))
                {
                    message = (Message)(Serializer.Deserialize(reader));
                }

                Package = new InternalPackage { Data = message.Data, From = IpEndPoints.First(p => p.Value.Address.Equals(ep.Address) && p.Value.Port == ep.Port-1).Key, To = message.ToClient };   
            }
        }

        public InternalPackage GetPackage()
        {
            while(Package == null)
                Thread.Sleep(1);
            return Package;
        }
    }
}
