using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteDangerous_UDP_Dispatcher
{
	public interface IDispatchedEventReceiver
	{
		void SendItem(string json);
	}
}
