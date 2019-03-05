using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace EliteDangerous_UDP_Dispatcher
{
	public class Program
	{
		static readonly CancellationTokenSource cts = new CancellationTokenSource();

		public async static Task Main(string[] args)
		{
			Console.CancelKeyPress += SelfDestructAllTheThings;
			var webHost = RunWebHost(cts.Token, args);

			Task.WaitAll(webHost);
			await Task.CompletedTask;
		}

		static void SelfDestructAllTheThings(object sender, ConsoleCancelEventArgs e)
		{
			Console.CancelKeyPress -= SelfDestructAllTheThings;
			cts.Cancel();
		}

		async static Task RunWebHost(CancellationToken token, string[] args) =>
			await WebHost.CreateDefaultBuilder(args)
			.UseShutdownTimeout(TimeSpan.FromSeconds(30))
				.UseStartup<Startup>()
			.Build()
			.RunAsync(token);
	}
}
