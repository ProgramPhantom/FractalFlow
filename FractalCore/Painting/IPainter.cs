﻿using OpenCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FractalCore.Painting
{
    /// <summary>
    /// Add this interface to all the painter classes.
    /// A painter class is a class which has a method which takes a reference to a 
    /// WriteableBitmap, a reference to a  Fractal, and puts colour on the 
    /// WriteableBitmap based on the Fractal's iterationsArray
    /// </summary>
    public interface IPainter
    {
        void Paint(ref WriteableBitmap fractalBitmap, ref Fractal fractal);

        public string PaintCLScript { get; set; }
        public void SetKernelParameters(ref MultiCL kernel, ref byte[] pixels, ref uint[] iterations, uint iterationsCap);
    }
}
