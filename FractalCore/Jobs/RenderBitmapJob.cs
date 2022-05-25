﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    public class RenderBitmapJob : Job
    {
        public ComputeIterationsJob ComputeIterationsJob;
        public IPainter Painter;
        public FractalImage FractalImage;

        // Revealers 
        public Fractal Fractal
        {
            get
            {
                return ComputeIterationsJob.Fractal;
            }
        }
        public FractalFrame FractalFrame
        {
            get
            {
                return ComputeIterationsJob.Fractal.FractalFrame;
            }
        }
        public IIterator Iterator
        {
            get
            {
                return ComputeIterationsJob.Fractal.Iterator;
            }
        }

        public RenderBitmapJob(Fractal fractal, IPainter painter, FractalImage image)
        {
            ComputeIterationsJob = new ComputeIterationsJob(fractal);
            Painter = painter;
            FractalImage = image;
        }
    }
}
