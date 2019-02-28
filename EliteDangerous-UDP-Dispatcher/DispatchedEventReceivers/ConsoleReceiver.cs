using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteDangerous_UDP_Dispatcher.DispatchedEventReceivers
{
	public class ConsoleReceiver : IDispatchedEventReceiver
	{
		public void SendItem(string json)
		{
			Console.WriteLine(json);
		}
	}
}
