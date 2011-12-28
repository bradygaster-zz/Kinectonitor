using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Samples.Kinect.WpfViewers;
using KinectNui = Microsoft.Research.Kinect.Nui;
using ServiceBusSimplifier;

namespace Kinectonitor.Windows.Monitor
{
	public partial class MainWindow : Window
	{
		KinectNui.Runtime _kinect;
		InteropBitmapHelper imageHelper = null;
		Timer _timer;
		bool _isTrackingActive = false;
		ServiceBus _bus;

		bool AreSkeletonsBeingTracked
		{
			get { return _isTrackingActive; }
			set
			{
				if (_isTrackingActive != value)
					TakePhoto(); // take a phot whenever the skeleton status changes

				_isTrackingActive = value;
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			this.Loaded += new RoutedEventHandler(OnLoad);
			this.Closing += new System.ComponentModel.CancelEventHandler(OnClosing);
		}

		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_kinect.Uninitialize();
			_timer.Stop();
		}

		void OnLoad(object sender, RoutedEventArgs e)
		{
			_kinect = KinectNui.Runtime.Kinects[0];
			_kinect.Initialize(KinectNui.RuntimeOptions.UseColor | RuntimeOptions.UseSkeletalTracking);
			_kinect.VideoStream.Open(KinectNui.ImageStreamType.Video, 2,
				KinectNui.ImageResolution.Resolution640x480,
				KinectNui.ImageType.Color);
			_kinect.VideoFrameReady += new EventHandler<KinectNui.ImageFrameReadyEventArgs>(OnKinectVideoReady);
			_kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

			_timer = new Timer();
			_timer.Interval = 3000;
			_timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);

			//_bus = ServiceBus.Setup(ServiceBusUtilities.GetServiceBusCredentials());
		}

		void OnTimerElapsed(object sender, ElapsedEventArgs e)
		{
			TakePhoto();
		}

		private void TakePhoto()
		{
			this.Dispatcher.Invoke(new Action(() =>
			{
				if (AreSkeletonsBeingTracked)
				{
					var filename = string.Format(@"c:\{0}.jpg", Environment.TickCount);
					FileStream stream = new FileStream(filename, FileMode.Create);
					JpegBitmapEncoder encoder = new JpegBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(_bitmap));
					encoder.Save(stream);
					stream.Close();

					byte[] imageData = null;

					using (FileStream fs = File.Open(filename, FileMode.Open))
					{
						imageData = new byte[fs.Length];
						fs.Position = 0;
						fs.Read(imageData, 0, imageData.Length);
					}

					/*
					_bus.Publish<ImageMessage>(new ImageMessage
					{
						Filename = System.IO.Path.GetFileName(filename),
						ImageData = imageData
					});
					*/
				}
			}));
		}

		BitmapSource _bitmap;

		void OnKinectVideoReady(object sender, KinectNui.ImageFrameReadyEventArgs e)
		{
			_timer.Start();

			_bitmap = BitmapSource.Create(e.ImageFrame.Image.Width,
				e.ImageFrame.Image.Height,
				96,
				96,
				PixelFormats.Bgr32,
				null,
				e.ImageFrame.Image.Bits,
				e.ImageFrame.Image.Width * e.ImageFrame.Image.BytesPerPixel
				);

			var planarImage = e.ImageFrame.Image;

			if (imageHelper == null)
			{
				imageHelper = new InteropBitmapHelper(planarImage.Width, planarImage.Height, planarImage.Bits);
				kinectVideo.Source = imageHelper.InteropBitmap;
			}
			else
			{
				imageHelper.UpdateBits(planarImage.Bits);
			}
		}

		#region Skeleton Processing
		private void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
		{
			SkeletonFrame skeletonFrame = e.SkeletonFrame;

			AreSkeletonsBeingTracked = skeletonFrame.Skeletons.Any(x => 
				x.TrackingState == SkeletonTrackingState.Tracked);

			skeletonActive.Text = AreSkeletonsBeingTracked
				? "Yes"
				: "No";

			//KinectSDK TODO: this shouldn't be needed, but if power is removed from the Kinect, you may still get an event here, but skeletonFrame will be null.
			if (skeletonFrame == null)
			{
				return;
			}

			int iSkeleton = 0;
			Brush[] brushes = new Brush[6];
			brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
			brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
			brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
			brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
			brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

			skeletonCanvas.Children.Clear();
			foreach (SkeletonData data in skeletonFrame.Skeletons)
			{
				if (SkeletonTrackingState.Tracked == data.TrackingState)
				{
					// Draw bones
					Brush brush = brushes[iSkeleton % brushes.Length];
					skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
					skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
					skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
					skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
					skeletonCanvas.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

					// Draw joints
					foreach (Joint joint in data.Joints)
					{
						Point jointPos = getDisplayPosition(joint);
						Line jointLine = new Line();
						jointLine.X1 = jointPos.X - 3;
						jointLine.X2 = jointLine.X1 + 6;
						jointLine.Y1 = jointLine.Y2 = jointPos.Y;
						jointLine.Stroke = jointColors[joint.ID];
						jointLine.StrokeThickness = 6;
						skeletonCanvas.Children.Add(jointLine);
					}
				}
				iSkeleton++;
			} // for each skeleton
		}

		private Polyline getBodySegment(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids)
		{
			PointCollection points = new PointCollection(ids.Length);
			for (int i = 0; i < ids.Length; ++i)
			{
				points.Add(getDisplayPosition(joints[ids[i]]));
			}

			Polyline polyline = new Polyline();
			polyline.Points = points;
			polyline.Stroke = brush;
			polyline.StrokeThickness = 5;
			return polyline;
		}

		private Point getDisplayPosition(Joint joint)
		{
			float depthX, depthY;
			_kinect.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
			depthX = depthX * 320; //convert to 320, 240 space
			depthY = depthY * 240; //convert to 320, 240 space
			int colorX, colorY;
			ImageViewArea iv = new ImageViewArea();
			// only ImageResolution.Resolution640x480 is supported at this point
			_kinect.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

			// map back to skeleton.Width & skeleton.Height
			return new Point((int)(skeletonCanvas.Width * colorX / 640.0), (int)(skeletonCanvas.Height * colorY / 480));
		}

		private static Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };

		#endregion Skeleton Processing
	}
}
