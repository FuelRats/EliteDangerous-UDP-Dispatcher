using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace EliteDangerous_UDP_Dispatcher.DispatchedEventReceivers
{
	public class WebhookDispatcher : IDispatchedEventReceiver
	{
        public WebhookDispatcher()
        {
            Console.WriteLine("DEBUG: Loaded WebhookDispatcher (from Config)");
        }


        public WebhookDispatcher(string url)
		{
			Url = url;
            Console.WriteLine("DEBUG: Loaded WebhookDispatcher");
        }

		public string Url { get; set; }

		public async void SendItem(string json)
		{
			var cli = HttpClientFactory.Create();

			cli.PostAsJsonAsync(Url, JsonConvert.DeserializeObject(json));
		}
	}
}
