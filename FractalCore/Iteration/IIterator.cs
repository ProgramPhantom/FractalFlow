using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FormulaParser;

namespace FractalCore
{
    public interface IIterator
    {
        public uint Iterate(Complex c, int maxIterations, int bail);

        RPN FormulaObject { get; set; }

        string Name { get; set; }
        string FormulaString { get; set; }
    }
}
