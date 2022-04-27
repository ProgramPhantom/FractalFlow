using FractalCore;
using FractalGeneratorMVVM.ViewModels;
using FractalGeneratorMVVM.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.Views.Models
{
    public class IteratorDESIGN
    {
        public static BasicIterator iterator => new BasicIterator("z^2+c", "Mandelbrot");
        public static IteratorViewModel INSTANCE => new IteratorViewModel(iterator, 1);
    }
}
