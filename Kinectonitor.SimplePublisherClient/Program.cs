using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServiceBusSimplifier;

namespace Kinectonitor.SimplePublisherClient
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceBus
				.Setup(ServiceBusUtilities.GetServiceBusCredentials())
				.Subscribe<ImageMessage>((e) =>
					{
						Console.WriteLine(
							string.Format("Image '{0}' received", e.Filename)
							);
					});

			Console.ReadLine();
		}
	}
}
