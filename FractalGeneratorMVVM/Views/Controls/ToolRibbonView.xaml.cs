using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace FractalGeneratorMVVM.Views.Controls
{
    /// <summary>
    /// Interaction logic for ToolRibbonView.xaml
    /// </summary>
    public partial class ToolRibbonView : UserControl
    {
        public ToolRibbonView()
        {
            InitializeComponent();
        }

        private void NumbersOnlyValidaiton(object sender, TextCompositionEventArgs e)
        {
            Regex numbersOnly = new Regex("[^0-9]+");
            e.Handled = numbersOnly.IsMatch(e.Text);
        }

        private void Width_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
