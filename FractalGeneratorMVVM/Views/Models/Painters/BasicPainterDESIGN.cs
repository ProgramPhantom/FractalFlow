using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalGeneratorMVVM.ViewModels;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels.Models.Painters;
using System.Windows.Media;
using FractalCore.Painting;

namespace FractalGeneratorMVVM.Views.Models.Painters
{
    public class BasicPainterDESIGN : BasicPainterLightViewModel
    {
        public static BasicPainterLight PAINTERINSANCE => new BasicPainterLight("Red", Color.FromRgb(255, 0, 0), Color.FromRgb(0, 0, 0));
        public static BasicPainterDESIGN INSTANCE => new BasicPainterDESIGN(1, PAINTERINSANCE);

        public BasicPainterDESIGN(int number, BasicPainterLight p) : base(p, number)
        {

        }
    }
}
