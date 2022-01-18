using Caliburn.Micro;
using FractalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalGeneratorMVVM.ViewModels
{
    public class FormulaStackViewModel : Screen
    {
        public List<Fractal> FormulaStackList;
        public FormulaStackViewModel()
        {
            FormulaStackList = new List<Fractal>();

            FormulaStackList.Add(new Fractal(1000, 1000));
        }
    }
}
