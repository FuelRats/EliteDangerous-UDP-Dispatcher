using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EliteDangerous_UDP_Dispatcher
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
