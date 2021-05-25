using ChurchDataShow.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChurchDataShow.Views
{
    /// <summary>
    /// Interação lógica para ucInicial.xam
    /// </summary>
    public partial class ucInicial : UserControl
    {
        public ucInicial()
        {
            InitializeComponent();

            BuscarVersiculo();
        }

        private async void BuscarVersiculo()
        {
            LoadingSpinner.Visibility = Visibility.Visible;
            VersiculoService service = new VersiculoService();

            await Task.Run(() =>
            {
                service.BuscarVersiculo();
            });

            LoadingSpinner.Visibility = Visibility.Hidden;

            if (service.Texto != null)
            {
                lblReferencia.Content = service.Referencia.ToUpper();
                lblVersiculo.Text = service.Texto;
            }
            else
            {
                lblReferencia.Content = "Ops! Não foi possível buscar o versículo do dia ):";
            }
        }
    }
}
