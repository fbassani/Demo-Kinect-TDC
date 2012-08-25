using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Kinect.Toolbox;
using Microsoft.Kinect;

namespace KinectSlideShow {

	public partial class MainWindow {
		private KinectSensor kinectAtivo;
		private Skeleton[] usuarios;
		private GerenciadorDoKinect gerenciadorDoKinect;
		private SlideShow slideShow;

		private SwipeGestureDetector detectorDeGestosMaoDireita;
		private SwipeGestureDetector detectorDeGestosMaoEsquerda;
		private ContextTracker contextTracker;

		private AlgorithmicPostureDetector detectorDeAceno;
		private bool usuarioAtivo;
	
		public MainWindow() {
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			gerenciadorDoKinect = new GerenciadorDoKinect();
			SensorChooserUI.KinectSensorChooser = gerenciadorDoKinect.SensorChooser;
			gerenciadorDoKinect.KinectAlterado += gerenciadorDoKinect_KinectAlterado;
			
			slideShow = new SlideShow();
			ProximaImagem();

			detectorDeGestosMaoDireita = new SwipeGestureDetector();
			detectorDeGestosMaoEsquerda = new SwipeGestureDetector();

			detectorDeGestosMaoDireita.OnGestureDetected += DetectorDeGestos_GestureDetected;
			detectorDeGestosMaoEsquerda.OnGestureDetected += DetectorDeGestos_GestureDetected;

			contextTracker = new ContextTracker();
			
			detectorDeAceno = new AlgorithmicPostureDetector();
			detectorDeAceno.PostureDetected += detectorDeAceno_PostureDetected;
			indicadorDeAtividade.Ativo = false;
		}

		private void detectorDeAceno_PostureDetected(string obj) {
			if (obj == "RightHello") {
				usuarioAtivo = true;
			} else if (obj == "LeftHello") {
				usuarioAtivo = false;
			}
			indicadorDeAtividade.Ativo = usuarioAtivo;
		}

		void gerenciadorDoKinect_KinectAlterado(KinectSensor kinect) {
			kinectAtivo = kinect;
			kinectAtivo.SkeletonFrameReady += kinectAtivo_SkeletonFrameReady;
			usuarios = new Skeleton[kinectAtivo.SkeletonStream.FrameSkeletonArrayLength];
		}

		private void DetectorDeGestos_GestureDetected(string gesto) {
			if (!usuarioAtivo) {
				return;
			}
			if (gesto == "SwipeToRight") {
				ProximaImagem();
			} else {
				ImagemAnterior();
			}
		}

		private void ProximaImagem() {
			var proximaImagem = slideShow.ProximaImagem();
			imagem.Source = proximaImagem;
			ExibaNomeDaImagem(proximaImagem);
		}

		private void ImagemAnterior() {
			var imagemAnterior = slideShow.ImagemAnterior();
			imagem.Source = imagemAnterior;
			ExibaNomeDaImagem(imagemAnterior);
		}

		private void ExibaNomeDaImagem(BitmapImage imagemCorrente) {
			nomeDaImagem.Content = Path.GetFileName(imagemCorrente.UriSource.AbsolutePath);
		}
		
		private void kinectAtivo_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
			using(var skeletonFrame = e.OpenSkeletonFrame()) {
				if (skeletonFrame == null) {
					return;
				}
				skeletonFrame.CopySkeletonDataTo(usuarios);
				var usuario = usuarios.Where(u => u.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
				if (usuario == null) {
					usuarioAtivo = false;
					return;
				}

				contextTracker.Add(usuario.Position.ToVector3(), usuario.TrackingId);
				if (!contextTracker.IsStableRelativeToCurrentSpeed(usuario.TrackingId)) {
					return;
				}

				detectorDeAceno.TrackPostures(usuario);

				var maoDireita = usuario.Joints[JointType.HandRight];
				var maoEsquerda = usuario.Joints[JointType.HandLeft];

				if (maoDireita.TrackingState == JointTrackingState.Tracked) {
					detectorDeGestosMaoDireita.Add(maoDireita.Position, kinectAtivo);
				}

				if (maoEsquerda.TrackingState == JointTrackingState.Tracked) {
					detectorDeGestosMaoEsquerda.Add(maoEsquerda.Position, kinectAtivo);
				}
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			if (gerenciadorDoKinect != null) {
				gerenciadorDoKinect.Dispose();
			}
		}
	}
}
