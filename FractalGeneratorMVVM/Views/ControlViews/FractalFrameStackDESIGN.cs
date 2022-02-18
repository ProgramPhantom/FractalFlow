using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalGeneratorMVVM.ViewModels;
using Caliburn.Micro;
using FractalCore;

namespace FractalGeneratorMVVM.Views
{
    public class FractalFrameStackDESIGN : FractalFrameStackViewModel
    {
        public static FractalFrameStackDESIGN Instance => new FractalFrameStackDESIGN();

        public FractalFrameStackDESIGN()
        {
            //FractalFrameViewModels = new BindableCollection<FractalFrameViewModel>();
            //FractalFrameCollection = new List<FractalFrame>();

            AddFractalFrame(new FractalFrame());
            AddFractalFrame(new FractalFrame());
            AddFractalFrame(new FractalFrame());

        }
    }
}
