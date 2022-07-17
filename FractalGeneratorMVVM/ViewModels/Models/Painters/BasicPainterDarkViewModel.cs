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

    public class BasicPainterDarkViewModel : BasicPainterBaseViewModelAbstract
    {

        public BasicPainterDarkViewModel(BasicPainterDark painter, int num, string name = "Untitled") : base(painter, num, false, name) { }

    }
}
