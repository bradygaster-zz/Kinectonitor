using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;
using ServiceBusSimplifier;

namespace Kinectonitor.Web.Hubs
{
	[HubName("imageSubscriberHub")]
	public class ImageSubscriberHub : Hub
	{
		public void DoWork()
		{
			ServiceBus
				.Setup(ServiceBusUtilities.GetServiceBusCredentials())
				.Subscribe<ImageStoredMessage>(this.OnImageReceived);
		}

		void OnImageReceived(ImageStoredMessage message)
		{
			Clients.imageReceived(message.Url);
		}
	}
}