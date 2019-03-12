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

            Task.WaitAll(udp, web);
        }

        static void SelfDestructAllTheThings(object sender, ConsoleCancelEventArgs e)
        {
            Console.CancelKeyPress -= SelfDestructAllTheThings;

            UdpListener.Program.ShutDown();
            WebServer.Program.ShutDown();

            cts.Cancel();
        }
    }
}
