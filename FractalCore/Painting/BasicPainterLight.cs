using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Windows;
using FractalCore;
using System.Windows.Media;

namespace FractalCore.Painting
{
    public class BasicPainterLight : BasicPainterBase, IPainter
    {
        

        public BasicPainterLight() : base("Default", Color.FromRgb(255, 0, 0), Color.FromRgb(0, 0, 0)) { }


        public BasicPainterLight(string name, Color mainColour, Color inSetColour) : base(name, mainColour, inSetColour) { }


        public override void Paint(ref WriteableBitmap fractalBitmap, ref Fractal fractal)
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
                        pixels[y, x, 0] = InSetColour.B;
                        pixels[y, x, 1] = InSetColour.G;
                        pixels[y, x, 2] = InSetColour.R;
                    }
                    else
                    {
                        // Invert the colours
                        int b = 255 - MainColour.B;
                        int g = 255 - MainColour.G;
                        int r = 255 - MainColour.R;
                        
                                              // Dim white light based on this (this inverses again)
                        pixels[y, x, 0] =  Convert.ToByte(255 - iterationRatio * b);
                        pixels[y, x, 1] =  Convert.ToByte(255 - iterationRatio * g);
                        pixels[y, x, 2] =  Convert.ToByte(255 - iterationRatio * r);
                        
                    }
                }
            }
            #endregion

            // Then write the array to the WriteableBitmap
            WriteArrToBM(ref pixels, ref fractalBitmap);
        }

        
    }
}
