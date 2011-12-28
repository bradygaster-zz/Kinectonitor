using System.Windows;
using Microsoft.Win32;
using System.IO;
using System;
using ServiceBusSimplifier;

namespace Kinectonitor.Windows.Sender
{
	public partial class MainWindow : Window
	{
		private ServiceBus _bus;

		public MainWindow()
		{
			InitializeComponent();

			_bus = ServiceBus.Setup(ServiceBusUtilities.GetServiceBusCredentials());
		}

		private void OnSelectPicturButtonClicked(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog
			{
				Filter = "Images|*.jpg;*.gif;*.png"
			};

			dlg.FileOk += (s, a) => 
			{
				Action begin = () =>
					{
						var msg = new ImageMessage { Filename = Path.GetFileName(dlg.FileName) };

						byte[] imageData;

						using (FileStream fs = File.Open(dlg.FileName, FileMode.Open))
						{
							imageData = new byte[fs.Length];
							fs.Position = 0;
							fs.Read(imageData, 0, imageData.Length);
						}

						msg.ImageData = imageData;

						try
						{
							_bus.Publish<ImageMessage>(msg);
						}
						catch
						{
							MessageBox.Show(string.Format("The image file '{0}' wasn't transmitted to the service bus. The image probably exceeds the maximum size.", dlg.FileName));
						}
					};

				begin.BeginInvoke(
					new AsyncCallback((ia) =>
						{
							begin.EndInvoke(ia);
						}), null);
			};

			dlg.ShowDialog();
		}
	}
}
