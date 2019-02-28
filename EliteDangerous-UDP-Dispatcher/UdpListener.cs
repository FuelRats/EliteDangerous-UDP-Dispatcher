using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EliteDangerous_UDP_Dispatcher
{
	public class UdpListener
	{
		public System.Net.Sockets.UdpClient UDP { get; set; }
		JsonSerializerSettings JSS;

		public UdpListener(int listenPort = 31173)
		{
			UDP = new System.Net.Sockets.UdpClient(listenPort);
			JSS = new JsonSerializerSettings()
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}

		public event EventHandler<UDPJsonLineReceivedEventArgs> FullLineEventHandler
		{
			add { fullLineEventHandler += value; }
			remove { if (fullLineEventHandler != null) { fullLineEventHandler -= value; } }
		}

		EventHandler<UDPJsonLineReceivedEventArgs> fullLineEventHandler;

		public async void SendData(object data, IPEndPoint receiver)
		{
			var dataObj = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
			UDP.SendAsync(dataObj, dataObj.Length, receiver);
		}

		public async Task ReceiveData(CancellationToken token)
		{
			StringBuilder sb = new StringBuilder();
			while (true)
			{
				if (token.IsCancellationRequested)
				{
					UDP.Close();
					break;
				}

				var buffer = await UDP.ReceiveAsync();
				var remoteInfo = buffer.RemoteEndPoint;
				var bufferAsString = Encoding.UTF8.GetString(buffer.Buffer);
				sb.Append(bufferAsString);

				if (bufferAsString.EndsWith("\r\n", StringComparison.InvariantCultureIgnoreCase))
				{
					var fullString = sb.ToString().Trim();

					sb.Clear();

					fullLineEventHandler.Invoke(this, new UDPJsonLineReceivedEventArgs
					{
						JSONString = fullString,
						RemoteInfo = remoteInfo
					});
				}
				else if (bufferAsString.StartsWith("{", StringComparison.InvariantCultureIgnoreCase) && 
					bufferAsString.EndsWith("}", StringComparison.InvariantCultureIgnoreCase) && 
					bufferAsString.Contains("Publish"))
				{
					var fullString = sb.ToString();

					var obj = JsonConvert.DeserializeObject(fullString);
					sb.Clear();

					fullLineEventHandler.Invoke(this, new UDPJsonLineReceivedEventArgs
					{
						JSON = obj,
						JSONString = fullString,
						RemoteInfo = remoteInfo
					});
				}
			}

			return;
		}
	}

	public class UDPJsonLineReceivedEventArgs : EventArgs
	{
		public dynamic JSON { get; set; }
		public string JSONString { get; set; }
		public IPEndPoint RemoteInfo { get; set; }
	}
}
