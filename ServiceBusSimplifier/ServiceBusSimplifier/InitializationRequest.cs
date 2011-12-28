using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceBusSimplifier
{
	public class InitializationRequest
	{
		public string Issuer { get; set; }
		public string IssuerKey { get; set; }
		public string Namespace { get; set; }
	}
}
