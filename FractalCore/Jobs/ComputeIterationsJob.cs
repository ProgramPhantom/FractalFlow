using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public class ComputeIterationsJob : Job
    {
        public Fractal Fractal;
        
        public ComputeIterationsJob(Fractal fractal)
        {
            Fractal = fractal;
        }
    }
}
