using System;
using System.Threading;
using System.Threading.Tasks;

namespace ED.FullUdpPackage
{
    class Program
    {
        static readonly CancellationTokenSource cts = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += SelfDestructAllTheThings;

            var udp = Task.Run(() => UdpListener.Program.Main(args));
            var web = Task.Run(() => WebServer.Program.Main(args));

            Task.WaitAny(udp, web);
            ShutDown();
        }

        static void SelfDestructAllTheThings(object sender, ConsoleCancelEventArgs e)
        {
            ShutDown();
        }

        private static void ShutDown()
        {
            Console.CancelKeyPress -= SelfDestructAllTheThings;

            try { UdpListener.Program.ShutDown(); } catch { }
            try { WebServer.Program.ShutDown(); } catch { }

            cts.Cancel();
        }
    }
}
