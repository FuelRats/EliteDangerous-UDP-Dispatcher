using System;

namespace ED.DispatchEventReceivers
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
