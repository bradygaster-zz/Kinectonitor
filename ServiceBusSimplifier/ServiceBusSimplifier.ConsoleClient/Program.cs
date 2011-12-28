using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceBusSimplifier.ConsoleClient
{
	class Program
	{
		static void Main(string[] args)
		{
			var bus = ServiceBus
				.Setup(new InitializationRequest
				{
					IssuerKey = "QS3GFImGyiuQMfExDJKcjfEZx0T0kvBTfgWNLtHVagA=",
					Issuer = "owner",
					Namespace = "nimblesquidqa01"
				})
				.Subscribe<SimpleEvent>((e) =>
				{
					Console.WriteLine(
						string.Format("This bus says '{0}'", e.Message)
						);
				})
				.Subscribe<OtherSimpleEvent>(HandleOtherSimpleEvent);

			var message = "Hello World!";

			while (!string.IsNullOrEmpty(message))
			{
				if (!string.IsNullOrEmpty(message))
				{
					bus.Publish<SimpleEvent>(
						new SimpleEvent
						{
							Message = message
						});

					bus.Publish<OtherSimpleEvent>(
						new OtherSimpleEvent
						{
							Title = message,
							Id = Guid.NewGuid()
						});
				}

				message = Console.ReadLine();
			}

			bus.Close().ClearTopics();

			Console.WriteLine("Hit enter to exit.");
			Console.ReadLine();
		}

		static void HandleOtherSimpleEvent(OtherSimpleEvent e)
		{
			Console.WriteLine(
				string.Format("This bus says '{0}' with an ID of '{1}'", e.Title, e.Id)
				);
		}
	}

	public class SimpleEvent
	{
		public string Message { get; set; }
	}

	public class OtherSimpleEvent
	{
		public string Title { get; set; }
		public Guid Id { get; set; }
	}
}
