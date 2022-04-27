using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FractalGeneratorMVVM.Views.Windows
{
    /// <summary>
    /// Interaction logic for AddPainterWindow.xaml
    /// </summary>
    public partial class AddPainterWindowView : UserControl
    {
        public AddPainterWindowView()
        {
            InitializeComponent();
        }


        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");  // MAKE THIS A 0 - 255 RANGE

            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
