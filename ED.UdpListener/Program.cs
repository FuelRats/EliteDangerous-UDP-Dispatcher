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

        public static int Main(string[] args)
        {
            var events = ConfigManager.LoadEventReceivers();

            EventReceivers.AddRange(events);

            if (HandleListenerArguments(args))
                return -1;

            //EventReceivers.Add(new ConsoleDispatcher());
            //EventReceivers.Add(new UdpDispatcher("127.0.0.1", 59295));
            //EventReceivers.Add(new TcpDispatcher("127.0.0.1", 59294));
            //EventReceivers.Add(new WebhookReceiver("https://webhook.site/6fbfcdf7-628d-40b7-ac6d-cec7956af1e9"));

            Console.CancelKeyPress += SelfDestructAllTheThings;
            UDP.FullLineEventHandler += Udp_FullLineEventHandler;
            var listener = UDP.ReceiveData(cts.Token);

            Task.WaitAll(listener);
            return 0;
        }

        /// <summary>
        /// Check for arguments sent to the listener
        /// </summary>
        /// <param name="args">Incoming parameters from console command</param>
        /// <returns>True = Intercept command and exit afterwards</returns>
        private static bool HandleListenerArguments(string[] args)
        {
            if (args.Length == 0) return false;

            var cmd = args[0].ToLower();
            switch(cmd)
            {
                case "config": return ConfigArguments(args);
                default: return Help();
            }
        }

        private static bool Help()
        {
            Console.WriteLine(
@"Elite: Dangerous UDP Dispatcher
-------------------------------
Usage:
- help                          This command
- config                        Manipulates the config in various manners
  - add tcp [address] [port]    Adds a TCPDispatcher that forwards all incoming UDP events
  - add udp [address] [port]    Adds a UDPDispatcher that forwards all incoming UDP events
  - add webhook [address]       Adds a WebhookDispatcher, that uses HTTP POST to send UDP events
  - add console                 Adds a ConsoleDispatcher, shows log in console (For debugging)

  - list                        List all current dispatchers in config with index counter for easy removal

  - remove [index]              Removes a single configuration, zero-based index"
);
            return true;
        }

        private static bool ConfigArguments(string[] args)
        {
            var configCmd = args[1];

            switch(configCmd)
            {
                case "list":
                    ListDispatchers();
                    break;
                case "remove":
                    RemoveDispatcher(args);
                    break;
                case "add":
                    AddDispatcher(args);
                    break;
            }

            return true;
        }

        private static void RemoveDispatcher(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("Index argument missing");
                return;
            }

            if(int.TryParse(args[2], out int selectedIndex))
            {
                if(EventReceivers.Count == 0)
                {
                    Console.WriteLine("Dispatcher list is empty, cannot remove anything from nothing.");
                    return;
                }

                if(EventReceivers.Count < (selectedIndex - 1))
                {
                    Console.WriteLine("Index argument too large");
                    return;
                }

                var cfgItem = EventReceivers[selectedIndex];

                EventReceivers.RemoveAt(selectedIndex);
                Console.WriteLine($"Removed {selectedIndex}:    {cfgItem.GetType().Name}       [{cfgItem.GetConfigJson()}]");
            }
        }

        private static void AddDispatcher(string[] args)
        {
            var type = args[2].ToLower();

            switch(type)
            {
                case "console":
                    EventReceivers.Add(new ConsoleDispatcher());
                    break;
                case "tcp":
                    if(args.Length < 5)
                    {
                        Console.WriteLine("Missing arguments: .. config add tcp [host] [port]");
                        return;
                    }

                    var tcpHostName = args[3];
                    var _tcpPort = args[4];

                    if(!int.TryParse(_tcpPort, out int tcpPort))
                    {
                        Console.WriteLine("Port must be a number");
                        return;
                    }

                    EventReceivers.Add(new TcpDispatcher(tcpHostName, tcpPort));
                    break;
                case "udp":
                    if (args.Length < 5)
                    {
                        Console.WriteLine("Missing arguments: .. config add udp [host] [port]");
                        return;
                    }

                    var udpHostName = args[3];
                    var _udpPort = args[4];

                    if (!int.TryParse(_udpPort, out int udpPort))
                    {
                        Console.WriteLine("Port must be a number");
                        return;
                    }

                    EventReceivers.Add(new UdpDispatcher(udpHostName, udpPort));
                    break;
                case "webhook":
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Missing arguments: .. config add webhook [url]");
                        return;
                    }

                    var url = args[3];
                    if(!Uri.TryCreate(url, UriKind.Absolute, out _))
                    {
                        Console.WriteLine("Argument not an URL");
                        return;
                    }

                    EventReceivers.Add(new WebhookDispatcher(url));
                    break;
            }

            ConfigManager.SaveEventReceivers(EventReceivers);
        }

        private static void ListDispatchers()
        {
            if (EventReceivers.Count == 0) Console.WriteLine("No dispatchers configured");

            for(var i = 0; i < EventReceivers.Count; i++)
            {
                var cfgItem = EventReceivers[i];
                Console.WriteLine($"{i}:    {cfgItem.GetType().Name}       [{cfgItem.GetConfigJson()}]");
            }
        }

        public static void ShutDown()
        {
            Console.CancelKeyPress -= SelfDestructAllTheThings;
            cts.Cancel();

            ConfigManager.SaveEventReceivers(EventReceivers);
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
            ShutDown();
        }
    }
}
