using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalGeneratorMVVM.ViewModels;


namespace FractalGeneratorMVVM.Views
{
    public class FractalFrameDESIGN : FractalFrameViewModel
    {
        public static FractalFrameDESIGN Instance => new FractalFrameDESIGN(1);

        public FractalFrameDESIGN(int num) : base(num)
        {
            Number = num;
            IsSelected = true;

            // DependencyProperty prop = new DependencyProperty();
        }
    }
}
