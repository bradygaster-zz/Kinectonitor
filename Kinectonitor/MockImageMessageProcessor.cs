using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kinectonitor
{
	public class MockImageMessageProcessor : IImageMessageProcessor
	{
		public void Process(ImageMessage imageMessage)
		{
			imageMessage.Filename = string.Format("{0}{1}", Environment.TickCount,
				Path.GetExtension(imageMessage.Filename));

			using (FileStream fs = File.Create(
				// TODO: make this template configurable
				string.Format(@"{0}\KinectonitorImages\{1}",
					AppDomain.CurrentDomain.BaseDirectory, imageMessage.Filename)))
			{
				fs.Write(imageMessage.ImageData, 0, imageMessage.ImageData.Length);
				fs.Close();
			}

			if (this.ImageReceived != null)
			{
				this.ImageReceived(this, new ImageMessageProcessorEventArgs(imageMessage));
			}

			Console.WriteLine(
				string.Format("I received an image named '{0}'", imageMessage.Filename)
				);
		}

		public event EventHandler<ImageMessageProcessorEventArgs> ImageReceived;

		public void ClearHandlers()
		{
			ImageReceived = null;
		}
	}
}
