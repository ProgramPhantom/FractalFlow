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
    public class PainterWhite : BasicPainter, IPainter
    {



        public PainterWhite(string name, byte red, byte green, byte blue) : base(name, red, green, blue)
        {

        }


        public override void  Paint(ref WriteableBitmap fractalBitmap, ref Fractal fractal)
        {
            byte[,,] pixels = new byte[fractal.Height, fractal.Width , 4];

            float iterations;
            float iterationCap = (float)fractal.Iterations;
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
                        pixels[y, x, 0] = 255;
                        pixels[y, x, 1] = 255;
                        pixels[y, x, 2] = 255;
                    }
                    else
                    {
                        pixels[y, x, 0] = Convert.ToByte(iterationRatio * Blue);
                        pixels[y, x, 1] = Convert.ToByte(iterationRatio * Green);
                        pixels[y, x, 2] = Convert.ToByte(iterationRatio * Red);
                    }
                }
            }
            #endregion

            // Then write the array to the WriteableBitmap
            WriteArrToBM(ref pixels, ref fractalBitmap);
        }
    }
}
