using ED.DispatchEventReceivers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ED.UdpListener
{
	public class ConfigManager
	{
		public static List<IDispatchedEventReceiver> LoadEventReceivers()
		{
			var f = File.ReadAllText("configuration.json");
			return JsonConvert.DeserializeObject<List<IDispatchedEventReceiver>>(f, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
		}

		public static void SaveEventReceivers(List<IDispatchedEventReceiver> receivers)
		{
			File.WriteAllText("configuration.json", JsonConvert.SerializeObject(receivers, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented }));
		}
	}
}
