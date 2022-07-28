using FractalCore.Painting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public class FullRenderJob : Job
    {
        public PaintJob PaintJob { get; set; }
        public BasicIterator Iterator
        {
            get
            {
                return Fractal.Iterator;
            }
        }
        public Fractal Fractal
        {
            get
            {
                return PaintJob.Fractal;
            }
        }
        
        public FullRenderJob(PaintJob pJob, int num) : base(num)
        {
            PaintJob = pJob;
        }

        public FullRenderJob(Fractal fractal, IPainter painter, FractalImage image, int num) : base(num)
        {
            PaintJob  = new PaintJob(fractal, painter, image, num);
        }
    }
}
