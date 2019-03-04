using System;

namespace EliteDangerous_UDP_Dispatcher.DispatchedEventReceivers
{
	public class ConsoleDispatcher : IDispatchedEventReceiver
	{
        public ConsoleDispatcher()
        {
            Console.WriteLine("DEBUG: Loaded ConsoleDispatcher");
        }
		public void SendItem(string json)
		{
			Console.WriteLine(json);
		}
	}
}
