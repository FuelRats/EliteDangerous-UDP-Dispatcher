using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ED.WebServer
{
	public class Program
	{
		static readonly CancellationTokenSource cts = new CancellationTokenSource();

		public static void Main(string[] args)
		{
			Console.CancelKeyPress += SelfDestructAllTheThings;
			var webHost = RunWebHost(cts.Token, args);

			Task.WaitAll(webHost);
		}

        public static void ShutDown()
        {
            Console.CancelKeyPress -= SelfDestructAllTheThings;
            cts.Cancel();
        }

		static void SelfDestructAllTheThings(object sender, ConsoleCancelEventArgs e)
		{
            ShutDown();
		}

		async static Task RunWebHost(CancellationToken token, string[] args) =>
			await WebHost.CreateDefaultBuilder(args)
			.UseShutdownTimeout(TimeSpan.FromSeconds(30))
				.UseStartup<Startup>()
			.Build()
			.RunAsync(token);
	}
}
