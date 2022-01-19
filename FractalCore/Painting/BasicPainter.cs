using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Windows;
using FractalCore;

namespace FractalCore
{
    public class BasicPainter : PainterBase, IPainter
    {
        private byte _red;
        private byte _green;
        private byte _blue;

        public byte Red
        {
            get { return _red; }
            set { _red = value; }
        }

        public byte Green
        {
            get { return _green; }
            set { _green = value; }
        }

        public byte Blue
        {
            get { return _blue; }
            set { _blue = value; }
        }

        public BasicPainter(string name, byte red, byte green, byte blue) : base(name)
        {
            _red = red;
            _green = green;
            _blue = blue;

            Name = name;
        }



        public void Paint(ref WriteableBitmap fractalBitmap, ref Fractal fractal)
        {
            byte[,,] pixels = new byte[fractal.Height, fractal.Width , 4];

            float iterations;
            float iterationCap = (float)fractal.IterationCap;
            float iterationRatio;

            #region Write to the array
            for (int y = 0; y < fractal.Height; y++)
            {
                for (int x = 0; x < fractal.Width; x++)
                {  // Iterate through every pixel in the fractal, first check if
                   // the number of iterations at that pixel is equal to the max it was allowed to go to
                    iterations = (float)fractal.IterationsArray[y, x];
                    iterationRatio = (iterations / iterationCap);


                    pixels[y, x, 3] = 255;  // Set the Alpha to full always.

                    if (iterationCap == iterations)  // In the set
                    {  // ^^ Checking if iteration ratio is 1 will have the same effect
                        // Paint it, black.
                        pixels[y, x, 0] = 0;
                        pixels[y, x, 1] = 0;
                        pixels[y, x, 2] = 0;
                    }
                    else
                    {
                        pixels[y, x, 0] = Convert.ToByte(iterationRatio * _red);
                        pixels[y, x, 1] = Convert.ToByte(iterationRatio * _green);
                        pixels[y, x, 2] = Convert.ToByte(iterationRatio * _blue);
                    }
                }
            }
            #endregion

            // Then write the array to the WriteableBitmap
            WriteArrToBM(ref pixels, ref fractalBitmap);
        }
    }
}
