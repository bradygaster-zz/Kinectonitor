using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusSimplifier
{
	public class ServiceBus
	{
        
		string _namespace;
		string _issuer;
		string _issuerKey;
		NamespaceManager _namespaceManager;
		MessagingFactory _messagingFactory;
		TokenProvider _tokenProvider;
		Uri _serviceUri;
		List<Tuple<string, SubscriptionClient>> _subscribers;

		private ServiceBus()
		{
			_subscribers = new List<Tuple<string, SubscriptionClient>>();
		}

		private void SetupServiceBusEnvironment()
		{
			if (_namespaceManager == null)
			{
				_tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(_issuer, _issuerKey);
				_serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", _namespace, string.Empty);
				_messagingFactory = MessagingFactory.Create(_serviceUri, _tokenProvider);
				_namespaceManager = new NamespaceManager(_serviceUri, _tokenProvider);
			}
		}

		public static ServiceBus Setup(InitializationRequest request)
		{
			var ret = new ServiceBus
			{
				_namespace = request.Namespace,
				_issuer = request.Issuer,
				_issuerKey = request.IssuerKey
			};

			return ret;
		}

		public ServiceBus Subscribe<T>(Action<T> receiveHandler)
		{
			SetupServiceBusEnvironment();
			var topicName = string.Format("Topic_{0}", typeof(T).Name);
			var subscriptionName = string.Format("Subscription_{0}", typeof(T).Name);

			if (!_namespaceManager.TopicExists(topicName))
				_namespaceManager.CreateTopic(topicName);

			var topic = _namespaceManager.GetTopic(topicName);

			SubscriptionDescription subscription;

			if (!_namespaceManager.SubscriptionExists(topic.Path, subscriptionName))
				subscription = _namespaceManager.CreateSubscription(topic.Path, subscriptionName);
			else
				subscription = _namespaceManager.GetSubscription(topic.Path, subscriptionName);

			var subscriptionClient = _messagingFactory.CreateSubscriptionClient(topicName, subscriptionName, ReceiveMode.ReceiveAndDelete);

			_subscribers.Add(new Tuple<string, SubscriptionClient>(topicName, subscriptionClient));

			Begin<T>(receiveHandler, subscriptionClient);

			return this;
		}

		private void Begin<T>(Action<T> receiveHandler, SubscriptionClient subscriptionClient)
		{
			subscriptionClient.BeginReceive(
				TimeSpan.FromMinutes(5),
				(cb) =>
				{
					var brokeredMessage = subscriptionClient.EndReceive(cb);
					if (brokeredMessage != null)
					{
						var messageData = brokeredMessage.GetBody<T>();
						receiveHandler(messageData);
						Begin<T>(receiveHandler, subscriptionClient);
					}
				},
				null);
		}

		public ServiceBus Publish<T>(T message)
		{
			SetupServiceBusEnvironment();
			var topicName = string.Format("Topic_{0}", typeof(T).Name);
			var topicClient = _messagingFactory.CreateTopicClient(topicName);

			try
			{
				topicClient.Send(new BrokeredMessage(message));
			}
			catch (Exception x)
			{
				throw x;
			}
			finally
			{
				topicClient.Close();
			}

			return this;
		}

		public ServiceBus Close()
		{
			_subscribers.ForEach((s) => s.Item2.Close());
			return this;
		}

		public ServiceBus ClearTopics()
		{
			_subscribers.ForEach((s) => _namespaceManager.DeleteTopic(s.Item1));
			return this;
		}
	}
}



