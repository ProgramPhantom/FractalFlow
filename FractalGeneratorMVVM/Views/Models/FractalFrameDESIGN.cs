using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels;
using System.Windows.Media;

namespace FractalGeneratorMVVM.Views.Models
{
    public class FractalFrameDESIGN : FractalFrameViewModel
    {
        public static FractalFrameDESIGN Instance => new FractalFrameDESIGN(2, Color.FromRgb(255, 255, 255));
        public static FractalFrame Frame => new FractalFrame();

        public FractalFrameDESIGN(int num, Color c) : base(num, Frame, c)
        {
            
            IsSelected = false;

            DependencyObject obj = new DependencyObject();
        }


    }
}
