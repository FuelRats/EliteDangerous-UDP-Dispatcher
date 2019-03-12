namespace ED.DispatchEventReceivers
{
	public interface IDispatchedEventReceiver
	{
		void SendItem(string json);
        string GetConfigJson();
	}
}
