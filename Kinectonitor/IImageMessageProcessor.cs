using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kinectonitor
{
	public interface IImageMessageProcessor
	{
		void Process(ImageMessage imageMessage);
		event EventHandler<ImageMessageProcessorEventArgs> ImageReceived;
		void ClearHandlers();
	}

	public class ImageMessageProcessorEventArgs : EventArgs
	{
		public ImageMessage ImageMessage { get; private set; }

		public ImageMessageProcessorEventArgs(ImageMessage imageMessage)
		{
			ImageMessage = imageMessage;
		}
	}
}
