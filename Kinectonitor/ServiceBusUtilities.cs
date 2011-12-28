using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceBusSimplifier;

namespace Kinectonitor
{
	public static class ServiceBusUtilities
	{
		public static InitializationRequest GetServiceBusCredentials()
		{
			return new InitializationRequest
			{
				IssuerKey = "",
				Issuer = "",
				Namespace = ""
			};
		}
	}
}
