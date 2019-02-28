using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EliteDangerous_UDP_Dispatcher.DispatchedEventReceivers
{
	public class WebhookReceiver : IDispatchedEventReceiver
	{
		public WebhookReceiver(string url)
		{
			Url = url;
		}

		public string Url { get; set; }

		public async void SendItem(string json)
		{
			var cli = HttpClientFactory.Create();

			cli.PostAsJsonAsync(Url, JsonConvert.DeserializeObject(json));
		}
	}
}
