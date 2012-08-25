using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectSlideShow {
	/// <summary>
	/// Interaction logic for IndicadorDeAtividade.xaml
	/// </summary>
	public partial class IndicadorDeAtividade : UserControl {
		private const string UriBase = "pack://application:,,,/KinectSlideShow;component/imagens/";

		private bool ativo;
		public bool Ativo {
			get { return ativo; }
			set {
				ativo = value;
				icone.Source = ativo ? new BitmapImage(new Uri(UriBase + "ativo.png")) : new BitmapImage(new Uri(UriBase + "inativo.png"));
			}
		}

		public IndicadorDeAtividade() {
			InitializeComponent();
		}
	}
}