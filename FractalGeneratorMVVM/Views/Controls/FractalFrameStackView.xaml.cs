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

namespace FractalGeneratorMVVM.Views.Controls
{
    /// <summary>
    /// Interaction logic for FractalFrameStack.xaml
    /// </summary>
    public partial class FractalFrameStackView : UserControl
    {
        public FractalFrameStackView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Some cringe code to get it to scroll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scroll(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
