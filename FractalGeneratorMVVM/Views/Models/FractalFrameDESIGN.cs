using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels;


namespace FractalGeneratorMVVM.Views.Models
{
    public class FractalFrameDESIGN : FractalFrameViewModel
    {
        public static FractalFrameDESIGN Instance => new FractalFrameDESIGN(2, 255, 255, 255);
        public static FractalFrame Frame => new FractalFrame();

        public FractalFrameDESIGN(int num, byte red, byte green, byte blue) : base(num, Frame, red, green, blue)
        {
            
            IsSelected = false;

            DependencyObject obj = new DependencyObject();
        }


    }
}
