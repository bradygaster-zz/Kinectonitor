using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace Kinectonitor.Web.Hubs
{
	[HubName("imageSubscriberHub")]
	public class ImageSubscriberHub : Hub
	{
		IImageMessageProcessor _processor;

		public ImageSubscriberHub()
		{
			// TODO: replace the static usage with IoC
			_processor = MvcApplication.MessageProcessor;
			_processor.ClearHandlers();
			_processor.ImageReceived += 
				new EventHandler<ImageMessageProcessorEventArgs>(OnImageReceived);
		}

		public void DoWork()
		{
			// TODO: this is just here for debugging
			// TODO: replace with authentication call/initialization routine/etc...
		}

		void OnImageReceived(object sender, ImageMessageProcessorEventArgs e)
		{
			Clients.imageReceived(e.ImageMessage.Filename);
		}
	}
}