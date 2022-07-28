using FractalCore.Painting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public class PaintJob : Job
    {
        
        public IPainter Painter;
        public FractalImage FractalImage;
        public Fractal Fractal;


        public PaintJob(Fractal fractal, IPainter painter, FractalImage image, int num) : base(num)
        {
            Fractal = fractal;
            Painter = painter;
            FractalImage = image;
        }


    }
}
