using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public interface IIterator
    {
        public int Iterate(Complex c, int maxIterations, int bail);
    }
}
