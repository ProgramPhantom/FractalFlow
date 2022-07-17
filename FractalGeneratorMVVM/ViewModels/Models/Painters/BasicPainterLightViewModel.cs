using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Media;
using FractalCore.Painting;

namespace FractalGeneratorMVVM.ViewModels.Models.Painters
{

    public class BasicPainterLightViewModel : BasicPainterBaseViewModelAbstract
    {
        public BasicPainterLightViewModel(BasicPainterLight painter, int num, string name = "Untitled") : base(painter, num, true, name) { }

    }
}
