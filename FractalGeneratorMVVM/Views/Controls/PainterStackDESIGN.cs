using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalGeneratorMVVM.ViewModels;
using Caliburn.Micro;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Controls;
using FractalCore.Painting;

namespace FractalGeneratorMVVM.Views.Controls
{
    public class PainterStackDESIGN : PainterStackViewModel
    {
        public static PainterStackDESIGN INSTANCE => new PainterStackDESIGN();

        public PainterStackDESIGN()
        {
            //FractalFrameViewModels = new BindableCollection<FractalFrameViewModel>();
            //FractalFrameCollection = new List<FractalFrame>();

            NewBasicPainterLight(new BasicPainterLight());
                           
            NewBasicPainterLight(new BasicPainterLight());
                           
            NewBasicPainterLight(new BasicPainterLight());
            

        }
    }
}
