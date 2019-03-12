using System;

namespace ED.DispatchEventReceivers
{
	public class ConsoleDispatcher : IDispatchedEventReceiver
	{
        public ConsoleDispatcher()
        {
            Console.WriteLine("DEBUG: Loaded ConsoleDispatcher");
        }

        public string GetConfigJson()
        {
            return "{}";
        }

        public void SendItem(string json)
		{
			Console.WriteLine(json);
		}
	}
}
