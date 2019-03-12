using System;
using System.Net.Http;
using System.Text;

namespace ED.DispatchEventReceivers
{
	public class WebhookDispatcher : IDispatchedEventReceiver
	{
        public WebhookDispatcher()
        {
            Console.WriteLine("DEBUG: Loaded WebhookDispatcher (from Config)");
        }

        private HttpClient _hc;
        public WebhookDispatcher(string url)
		{
			Url = url;
            Console.WriteLine("DEBUG: Loaded WebhookDispatcher");
        }

        private void Init()
        {
            _hc = new HttpClient();
        }

		public string Url { get; set; }

		public async void SendItem(string json)
		{
            if(_hc == null)
            {
                Init();
            }
            await _hc.PostAsync(Url, new StringContent(json, Encoding.UTF8, "application/json"));
		}

        public string GetConfigJson()
        {
            return "{ Url: '" + Url.Replace("'", "\'") + "' }";
        }
    }
}
