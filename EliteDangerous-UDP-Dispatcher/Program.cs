using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EliteDangerous_UDP_Dispatcher
{
	public class Program
	{
		static readonly CancellationTokenSource cts = new CancellationTokenSource();

		static UdpListener UDP = new UdpListener();

		static List<IDispatchedEventReceiver> EventReceivers = new List<IDispatchedEventReceiver>();

		public async static Task Main(string[] args)
		{
			var events = ConfigManager.LoadEventReceivers();
            
			EventReceivers.AddRange(events);

            //EventReceivers.Add(new DispatchedEventReceivers.ConsoleDispatcher());
            //EventReceivers.Add(new DispatchedEventReceivers.TcpDispatcher("127.0.0.1", 58439));
			//EventReceivers.Add(new DispatchedEventReceivers.WebhookReceiver("https://webhook.site/6fbfcdf7-628d-40b7-ac6d-cec7956af1e9"));

			Console.CancelKeyPress += SelfDestructAllTheThings;
			var webHost = RunWebHost(cts.Token, args);

			UDP.FullLineEventHandler += Udp_FullLineEventHandler;
			var listener = UDP.ReceiveData(cts.Token);

			Task.WaitAll(webHost, listener);
			await Task.CompletedTask;
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

		async static Task RunWebHost(CancellationToken token, string[] args) =>
			await WebHost.CreateDefaultBuilder(args)
			.UseShutdownTimeout(TimeSpan.FromSeconds(30))
				.UseStartup<Startup>()
			.Build()
			.RunAsync(token);
	}
}
