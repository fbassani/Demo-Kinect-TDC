using System;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

namespace KinectSlideShow {
	public class GerenciadorDoKinect : IDisposable {
		public KinectSensor kinect;
		public KinectSensorChooser SensorChooser { get; private set; }

		public event Action<KinectSensor> KinectAlterado = delegate { }; 

		public GerenciadorDoKinect() {
			SensorChooser = new KinectSensorChooser();
			SensorChooser.KinectChanged += kinectChooser_KinectChanged;
			SensorChooser.Start();
		}

		private void kinectChooser_KinectChanged(object sender, KinectChangedEventArgs e) {
			if (e.NewSensor == null) {
				return;
			}

			kinect = e.NewSensor;
			kinect.DepthStream.Enable();
			kinect.SkeletonStream.Enable();
			kinect.Start();
			KinectAlterado(kinect);
		}

		public void Dispose() {
			if (kinect != null) {
				kinect.Dispose();
			}
		}
	}
}