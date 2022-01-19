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
        public BindableCollection<Fractal> FractalCollection { get; set; }

        public FormulaStackViewModel()
        {
            FractalCollection = new BindableCollection<Fractal>();

        }
    }
}
