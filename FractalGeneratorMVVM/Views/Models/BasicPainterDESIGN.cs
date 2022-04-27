using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalGeneratorMVVM.ViewModels;
using FractalCore;
using FractalGeneratorMVVM.ViewModels.Models;

namespace FractalGeneratorMVVM.Views.Models
{
    public class BasicPainterDESIGN : BasicPainterViewModel
    {
        public static BasicPainter PAINTERINSANCE => new BasicPainter("Red", 255, 0, 0);
        public static BasicPainterDESIGN INSTANCE => new BasicPainterDESIGN(1, PAINTERINSANCE);

        public BasicPainterDESIGN(int number, BasicPainter p) : base(p, number)
        {

        }
    }
}
