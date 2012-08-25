using System.Windows;
using System.Windows.Threading;

namespace KinectSlideShow {
	public partial class App {
		public App() {
			Dispatcher.UnhandledException += Dispatcher_UnhandledException;
		}

		private static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
			MessageBox.Show(e.Exception.Message);
			Current.Shutdown();
		}
	}
}
