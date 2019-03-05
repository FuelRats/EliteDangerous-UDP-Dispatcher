using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EliteDangerous_UDP_Dispatcher.DispatchedEventReceivers
{
    public class UdpDispatcher : IDispatchedEventReceiver
    {
        public string RemoteHost { get; set; }
        public int RemotePort { get; set; }

        private DnsEndPoint RemoteAddress { get; set; }

        private UdpClient Client { get; set; }

        public UdpDispatcher(string remoteAddress, int remotePort)
        {
            RemoteHost = remoteAddress;
            RemotePort = remotePort;
            Console.WriteLine("DEBUG: Loaded UdpDispatcher");
        }

        public UdpDispatcher()
        {
            Console.WriteLine("DEBUG: Loaded UdpDispatcher (from Config)");
        }

        public void Connect()
        {
            RemoteAddress = new DnsEndPoint(RemoteHost, RemotePort);
            Client = new UdpClient();
        }

        public async void SendItem(string json)
        {
            try
            {
                if (Client == null) Connect();
                var bytesToSend = Encoding.UTF8.GetBytes(json);
                await Client.SendAsync(bytesToSend, bytesToSend.Length, RemoteAddress.Host, RemoteAddress.Port);
            }
            catch (Exception ex)
            {
                // Should probably fix some kind of retry here..
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
