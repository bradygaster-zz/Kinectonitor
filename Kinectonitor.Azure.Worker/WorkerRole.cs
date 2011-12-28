using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;
using ServiceBusSimplifier;

namespace Kinectonitor.Azure.Worker
{
	public class WorkerRole : RoleEntryPoint
	{
		public const string StorageConnectionString = "StorageConnectionString";
		public const string KinectonitorImageContainer = "kinectonitor";
		private CloudBlobContainer _container;
		private ServiceBus _serviceBus;

		private static void Log(string message)
		{
			Trace.WriteLine(message, "Information");
		}

		private void CreateStorageContainer()
		{
			Log("Attaching to or creating the blob container");

			_container = CloudStorageAccount
				.Parse(RoleEnvironment.GetConfigurationSettingValue(WorkerRole.StorageConnectionString))
					.CreateCloudBlobClient()
					.GetContainerReference(WorkerRole.KinectonitorImageContainer);

			_container.CreateIfNotExist();

			_container.SetPermissions(new BlobContainerPermissions
			{
				PublicAccess = BlobContainerPublicAccessType.Blob
			});

			Log("Attached to the blob container");
		}

		private void SubscribeToImageMessages()
		{
			Log("Subscribing to ImageMessage messages");

			_serviceBus = ServiceBus
				.Setup(ServiceBusUtilities.GetServiceBusCredentials())
				.Subscribe<ImageMessage>(this.SaveImageToBlobStorage);

			Log("Subscribed to ImageMessage messages");
		}

		private void SaveImageToBlobStorage(ImageMessage msg)
		{
			Log(string.Format("Saving {0} to blob storage", msg.Filename));

			var url = string.Empty;

			using (var ms = new MemoryStream())
			{
				ms.Write(msg.ImageData, 0, msg.ImageData.Length);
				ms.Position = 0;

				CloudBlob blob = this._container.GetBlobReference(msg.Filename);
				blob.UploadFromStream(ms);
				url = blob.Uri.AbsoluteUri;

				ms.Close();
			}

			Log(string.Format("Saved {0} to blob storage", msg.Filename));

			PublishBlobStoredImageUrl(url);
		}

		private void PublishBlobStoredImageUrl(string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				Log(string.Format("Publishing that image {0} was saved to blob storage", url));

				_serviceBus.Publish<ImageStoredMessage>(
					new ImageStoredMessage
					{
						Url = url
					});

				Log(string.Format("Published that image {0} was saved to blob storage", url));
			}
		}

		public override void Run()
		{
			Log("Kinectonitor entry point called");

			while (true)
			{
				Thread.Sleep(10000);
			}
		}

		public override bool OnStart()
		{
			ServicePointManager.DefaultConnectionLimit = 12;

			CreateStorageContainer();

			SubscribeToImageMessages();

			return base.OnStart();
		}
	}
}
