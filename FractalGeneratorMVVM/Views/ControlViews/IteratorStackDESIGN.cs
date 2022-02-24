using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalCore;
using FractalGeneratorMVVM.ViewModels;

namespace FractalGeneratorMVVM.Views.ControlViews
{
    public class IteratorStackDESIGN : IteratorStackViewModel
    {

        public static IteratorStackDESIGN INSTANCE => new IteratorStackDESIGN();

        public IteratorStackDESIGN()
        {
            AddIterator();
            AddIterator();
        }
    }
}
