using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalGeneratorMVVM.ViewModels;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels.Models.Painters;

namespace FractalGeneratorMVVM.Views.Models.Painters
{
    public class PainterWhiteDESIGN : PainterWhiteViewModel
    {
        public static PainterWhite PAINTERINSANCE => new PainterWhite("Test", 0x3D, 0xB5, 0x49);
        public static PainterWhiteDESIGN INSTANCE => new PainterWhiteDESIGN(PAINTERINSANCE, 1);

        public PainterWhiteDESIGN( PainterWhite p, int number) : base(p, number)
        {

        }
    }
}
