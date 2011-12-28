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
				IssuerKey = "QS3GFImGyiuQMfExDJKcjfEZx0T0kvBTfgWNLtHVagA=",
				Issuer = "owner",
				Namespace = "nimblesquidqa01"
			};
		}
	}
}
