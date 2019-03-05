using ED.DispatchEventReceivers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ED.UdpListener
{
    public class Program
    {
        static readonly CancellationTokenSource cts = new CancellationTokenSource();

        static UdpListener UDP = new UdpListener();

        static List<IDispatchedEventReceiver> EventReceivers = new List<IDispatchedEventReceiver>();

        public static void Main(string[] args)
        {
            var events = ConfigManager.LoadEventReceivers();

            EventReceivers.AddRange(events);

            //EventReceivers.Add(new ConsoleDispatcher());
            //EventReceivers.Add(new UdpDispatcher("127.0.0.1", 59295));
            //EventReceivers.Add(new TcpDispatcher("127.0.0.1", 59294));
            //EventReceivers.Add(new WebhookReceiver("https://webhook.site/6fbfcdf7-628d-40b7-ac6d-cec7956af1e9"));

            Console.CancelKeyPress += SelfDestructAllTheThings;
            UDP.FullLineEventHandler += Udp_FullLineEventHandler;
            var listener = UDP.ReceiveData(cts.Token);

            Task.WaitAll(listener);
        }

        static void Udp_FullLineEventHandler(object sender, UDPJsonLineReceivedEventArgs e)
        {
            if (e?.JSON?.Publish == "EliteDangerous")
            {
                UDP.SendData(new
                {
                    Subscribe = true,
                    All = true
                }, e.RemoteInfo);
                return;
            }

            Parallel.ForEach(EventReceivers, er =>
            {
                er.SendItem(e.JSONString);
            });
        }

        static void SelfDestructAllTheThings(object sender, ConsoleCancelEventArgs e)
        {
            Console.CancelKeyPress -= SelfDestructAllTheThings;
            cts.Cancel();

            ConfigManager.SaveEventReceivers(EventReceivers);
        }
    }
}
