using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace KinectSlideShow {
	public class SlideShow {

		private readonly LinkedList<BitmapImage> bitmaps;
		private LinkedListNode<BitmapImage> nodoCorrente;

		public SlideShow() {
			var diretorioImagens = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
			var arquivos = Directory.GetFiles(diretorioImagens, "*.jpg", SearchOption.AllDirectories);
			bitmaps = new LinkedList<BitmapImage>(CrieBitmaps(arquivos));
		}

		private static IEnumerable<BitmapImage> CrieBitmaps(IEnumerable<string> caminhoParaImagens) {
			return caminhoParaImagens.Select(caminho => new BitmapImage(new Uri(caminho)));
		}

		public BitmapImage ProximaImagem() {
			if (nodoCorrente == null) {
				nodoCorrente = bitmaps.First;
			} else {
				nodoCorrente = nodoCorrente.Next;
			}
			return nodoCorrente == null ? bitmaps.First.Value : nodoCorrente.Value;
		}

		public BitmapImage ImagemAnterior() {
			if (nodoCorrente == null) {
				nodoCorrente = bitmaps.Last;
			} else {
				nodoCorrente = nodoCorrente.Previous;
			}
			return nodoCorrente == null ? bitmaps.Last.Value : nodoCorrente.Value;
		}
	}
}