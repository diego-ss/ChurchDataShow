using ChurchDataShow.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChurchDataShow.Views.TemplateViews
{
    /// <summary>
    /// Interação lógica para ucConfiguracaoTexto.xam
    /// </summary>
    public partial class ucConfiguracaoTexto : UserControl
    {
        public ucConfiguracaoTexto()
        {
            InitializeComponent();
        }

        private void TextColor_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

        }

        private void MudarCor(object sender, RoutedEventArgs e)
        {
            var color = (sender as Button).Background;
            DataShowScreen.GetInstance().MudarCorTexto(color);
        }

        private void MudarAlinhamento(object sender, RoutedEventArgs e)
        {
            var name = (sender as ListBoxItem).Name;
            TextAlignment alignment = TextAlignment.Center;

            switch (name)
            {
                case "AlignLeft":
                    alignment = TextAlignment.Left;
                    break;
                case "AlignCenter":
                    alignment = TextAlignment.Center;
                    break;
                case "AlignRight":
                    alignment = TextAlignment.Right;
                    break;
                case "AlignJustify":
                    alignment = TextAlignment.Justify;
                    break;
            }

            DataShowScreen.GetInstance().MudarAlinhamento(alignment);
        }

        private void MudarNegrito(object sender, RoutedEventArgs e)
        {
            DataShowScreen.GetInstance().MudarNegrito((sender as ToggleButton).IsChecked.Value);
        }
    }
}
