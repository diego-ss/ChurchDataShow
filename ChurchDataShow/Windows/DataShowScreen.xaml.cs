using ChurchDataShow.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfScreenHelper;

namespace ChurchDataShow.Windows
{
    /// <summary>
    /// Lógica interna para DataShowScreen.xaml
    /// </summary>
    public partial class DataShowScreen : Window
    {
        private static DataShowScreen _screenInstance;

        private DataShowScreen()
        {
            InitializeComponent();
        }

        public static DataShowScreen GetInstance()
        {
            if (_screenInstance == null)
            {
                _screenInstance = new DataShowScreen();
            }

            return _screenInstance;
        }

        public void ExibirTexto(string Texto, ConfiguracaoTexto configuracaoTexto)
        {
            tbTextos.Text = Texto;


        }

        public void MudarCorTexto(Brush Color)
        {
            tbTextos.Foreground = Color;
        }

        public void MudarAlinhamento(TextAlignment Align)
        {
            tbTextos.TextAlignment = Align;
        }

        public void MudarNegrito(bool Negrito)
        {
            tbTextos.FontWeight = Negrito ? FontWeights.Bold : FontWeights.Normal;
        }

        public void OcultarTexto()
        {
            tbTextos.Text = "";
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var screen = Screen.AllScreens.Where(s => s != Screen.PrimaryScreen).ToList()[0];
            this.Top = screen.WorkingArea.Top;
            this.Left = screen.WorkingArea.Left;
        }
    }
}
