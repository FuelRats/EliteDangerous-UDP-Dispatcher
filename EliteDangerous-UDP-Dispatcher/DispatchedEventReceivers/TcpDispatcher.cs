using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EliteDangerous_UDP_Dispatcher.DispatchedEventReceivers
{
    public class TcpDispatcher : IDispatchedEventReceiver
    {
        public string RemoteHost { get; set; }
        public int RemotePort { get; set; }

        private DnsEndPoint RemoteAddress { get; set; }

        private TcpClient Client { get; set; }
        private NetworkStream Stream { get; set; }

        public TcpDispatcher(string remoteAddress, int remotePort)
        {
            RemoteHost = remoteAddress;
            RemotePort = remotePort;
            Console.WriteLine("DEBUG: Loaded TcpDispatcher");
        }

        public TcpDispatcher()
        {
            Console.WriteLine("DEBUG: Loaded TcpDispatcher (from Config)");
        }

        public void Connect()
        {
            RemoteAddress = new DnsEndPoint(RemoteHost, RemotePort);
            Client = new TcpClient();
            Client.Connect(RemoteAddress.Host, RemoteAddress.Port);
            Stream = Client.GetStream();
        }

        public async void SendItem(string json)
        {
            try
            {
                if (Client == null) Connect();
                if (!Client.Connected)
                {
                    Stream.Dispose();
                    Client.Dispose();
                    Connect();
                }

                await Stream.WriteAsync(Encoding.UTF8.GetBytes(json));
                await Stream.FlushAsync();
            }
            catch(Exception ex)
            {
                // Should probably fix some kind of retry here..
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
