using ChurchDataShow.Database;
using ChurchDataShow.Services;
using ChurchDataShow.Views;
using ChurchDataShow.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChurchDataShow
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ucInicial ucInicial;
        private static ucBiblia ucBiblia;
        private static ucLetras ucLetras;
        private static ucMidia ucMidia;
        private static ucMusicas ucMusicas;

        private static Dictionary<string, UserControl> userControls;

        public MainWindow()
        {
            InitializeComponent();

            VerificarBancoDeDados();
            CarregarTelas();

            DataShowScreen dataShowScreen = DataShowScreen.GetInstance();
            dataShowScreen.Show();
        }

        private async void VerificarBancoDeDados()
        {
            gdInicial.Opacity = 0.1;
            Menu.IsEnabled = false;

            DbConnection dbConnection = DbConnection.GetInstance();
            await dbConnection.CreateDatabase();

            Menu.IsEnabled = true;
            gdLoading.Visibility = Visibility.Hidden;
            gdInicial.Opacity = 1;
        }
        private async void CarregarTelas()
        {
            ucInicial = new ucInicial();
            ucBiblia = new ucBiblia();
            ucLetras = new ucLetras();
            ucMidia = new ucMidia();
            ucMusicas = new ucMusicas();

            gdInicial.Children.Clear();
            gdInicial.Children.Add(ucInicial);

            userControls = new Dictionary<string, UserControl>()
            {
                { "Home", ucInicial},
                { "Bíblia", ucBiblia},
                { "Letras", ucLetras},
                { "Mídia", ucMidia},
                { "Músicas", ucMusicas},
            };
        }

        private void EfeitoSelecao(object sender, MouseButtonEventArgs e)
        {
            var labels = Menu.Children.OfType<Label>();

            foreach (var label in labels)
                label.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#0F0F2D"));

            var item = sender as Label;
            item.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6B66F8"));

            gdInicial.Children.Clear();
            gdInicial.Children.Add(userControls[item.Content.ToString()]);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnRedimensionar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
        }

        private void btnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
