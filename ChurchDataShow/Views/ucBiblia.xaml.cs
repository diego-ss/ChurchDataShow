using ChurchDataShow.Models;
using ChurchDataShow.Repositories;
using ChurchDataShow.ViewModels;
using ChurchDataShow.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ChurchDataShow.Views
{
    /// <summary>
    /// Interação lógica para ucBiblia.xam
    /// </summary>
    public partial class ucBiblia : UserControl
    {
        private readonly BibliaRepositorio _bibliaRepositorio;

        private List<Livro> _lstLivros;
        private List<RecentesViewModel> _lstTextosRecentes;
        private TextoBiblia _textoAtual;

        public ucBiblia()
        {
            InitializeComponent();

            _bibliaRepositorio = new BibliaRepositorio();
            _lstTextosRecentes = new List<RecentesViewModel>();

            PreencherLivros();
        }

        private async void PreencherLivros()
        {
            _lstLivros = (await _bibliaRepositorio.ListarLivros()).OrderBy(l=>l.Nome).ToList();

            cbLivros.ItemsSource = _lstLivros;
            cbLivros.DisplayMemberPath = "Nome";
            cbLivros.SelectedValuePath = "Id";
        }

        private async void cbLivros_DropDownClosed(object sender, System.EventArgs e)
        {
            if (cbLivros.SelectedIndex == -1)
                return;

            cbCapitulos.SelectedItem = -1;
            cbCapitulos.Text = "";

            cbVersiculos.SelectedItem = -1;
            cbVersiculos.Text = "";

            var lstCapitulos = (await _bibliaRepositorio.ListarCapitulos(int.Parse(cbLivros.SelectedValue.ToString())));
            cbCapitulos.ItemsSource = lstCapitulos;
        }

        private async void cbCapitulos_DropDownClosed(object sender, System.EventArgs e)
        {
            if (cbCapitulos.SelectedIndex == -1)
                return;

            cbVersiculos.SelectedItem = -1;
            cbVersiculos.Text = "";

            int IdLivro = int.Parse(cbLivros.SelectedValue.ToString());
            int Capitulo = int.Parse(cbCapitulos.Text);

            var lstVersiculos = (await _bibliaRepositorio.ListarVersiculos(IdLivro, Capitulo));
            cbVersiculos.ItemsSource = lstVersiculos;
        }

        private async void btnBuscar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (cbLivros.SelectedIndex == -1 || cbCapitulos.SelectedIndex == -1 || cbVersiculos.SelectedIndex == -1)
                return;

            int IdLivro = int.Parse(cbLivros.SelectedValue.ToString());
            int Capitulo = int.Parse(cbCapitulos.Text);
            int Versiculo = int.Parse(cbVersiculos.Text);
            int IdTraducao = 0;

            var lstTraducoes = (await _bibliaRepositorio.ListarTraducoes());

            foreach(var radio in RadioPanel.Children.OfType<RadioButton>())
                if (radio.IsChecked == true)
                    IdTraducao = lstTraducoes.Where(t => t.Nome == radio.Content.ToString()).FirstOrDefault().Id;

            var textoBiblia = (await _bibliaRepositorio.BuscarTexto(IdLivro, Capitulo, Versiculo, IdTraducao));
            SetarVersiculo(textoBiblia);
        }

        private async void SetarVersiculo(TextoBiblia textoBiblia, bool addHistorico = true)
        {
            tbTextoBiblia.Text = textoBiblia.Texto;
            _textoAtual = textoBiblia;
            lblVersiculo.Content = "Versiculo atual: " + textoBiblia.Versiculo;

            if (addHistorico)
            {
                var viewModel = new RecentesViewModel();
                viewModel.TextoBiblia = textoBiblia;
                viewModel.Livro = (await _bibliaRepositorio.ListarLivros()).Where(l => l.Id == textoBiblia.IdLivro).FirstOrDefault();

                if(!_lstTextosRecentes.Exists(x=>x.TextoBiblia.Id == textoBiblia.Id))
                    _lstTextosRecentes.Insert(0, viewModel);

                lbxTextosRecentes.ItemsSource = null;
                lbxTextosRecentes.Items.Clear();
                lbxTextosRecentes.ItemsSource = _lstTextosRecentes;
                lbxTextosRecentes.UpdateLayout();
            }
        }

        private void btnOcultar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataShowScreen.GetInstance().OcultarTexto();
        }

        private void btnExibir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ExibirTexto();
        }

        private void ExibirTexto()
        {
            DataShowScreen.GetInstance().ExibirTexto(tbTextoBiblia.Text, null);
        }

        private void SetarVersiculoHistorio(object sender, SelectionChangedEventArgs e)
        {
            if (lbxTextosRecentes.SelectedIndex == -1)
                return;

            var textoBiblia = (lbxTextosRecentes.SelectedItem as RecentesViewModel).TextoBiblia;
            SetarVersiculo(textoBiblia, false);
        }

        private async void btnAnterior_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_textoAtual == null)
                return;

            var proximoVersiculo = _textoAtual.Versiculo - 1;
            var textoSeguinte = (await _bibliaRepositorio.BuscarTexto(_textoAtual.IdLivro, _textoAtual.Capitulo, proximoVersiculo, _textoAtual.IdTraducao));

            if (textoSeguinte == null)
                return;

            SetarVersiculo(textoSeguinte, false);
            ExibirTexto();
        }

        private async void btnProximo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_textoAtual == null)
                return;

            var proximoVersiculo = _textoAtual.Versiculo + 1;
            var textoSeguinte = (await _bibliaRepositorio.BuscarTexto(_textoAtual.IdLivro, _textoAtual.Capitulo, proximoVersiculo, _textoAtual.IdTraducao));

            if (textoSeguinte == null)
                return;

            SetarVersiculo(textoSeguinte, false);
            ExibirTexto();
        }
    }
}
