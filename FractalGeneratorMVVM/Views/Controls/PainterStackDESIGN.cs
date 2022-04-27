using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalGeneratorMVVM.ViewModels;
using Caliburn.Micro;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Controls;

namespace FractalGeneratorMVVM.Views.Controls
{
    public class PainterStackDESIGN : PainterStackViewModel
    {
        public static PainterStackDESIGN INSTANCE => new PainterStackDESIGN();

        public PainterStackDESIGN()
        {
            //FractalFrameViewModels = new BindableCollection<FractalFrameViewModel>();
            //FractalFrameCollection = new List<FractalFrame>();

            NewBasicPainter(new BasicPainter("Test", 1, 255, 25));
            NewPainterWhite(new PainterWhite("Test", 233, 2, 2));

        }
    }
}
