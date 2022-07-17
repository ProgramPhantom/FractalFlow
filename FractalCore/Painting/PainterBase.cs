using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FractalCore.Painting
{
    public abstract class PainterBase 
    {

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PainterBase(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Writes a 2d array of pixels each with 4 bytes to hold R, G, B, A values, to a WriteableBitmap
        /// </summary>
        /// <param name="arr">A 2d array with pixel values reprisented by 4 bytes</param>
        /// <param name="bm">The bitmap object that is to be written to</param>
        public static void WriteArrToBM(ref byte[,,] arr, WriteableBitmap bm)
        {
            int arrayX = arr.GetLength(1);  // This might be breaking it plz double check
            int arrayY = arr.GetLength(0);

            // Squash the array down into 1 dimension.
            byte[]? pixel1d = new byte[arrayX * arrayY * 4];
            int index = 0;
            for (int y = 0; y < arrayY; y++)
            {
                for (int x = 0; x < arrayX; x++)
                {
                    for (int i = 0; i < 4; i++)  // Iterate the 4 values at each pixel
                        pixel1d[index++] = arr[y, x, i];
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, arrayX, arrayY);
            int stride = 4 * arrayX;

            bm.WritePixels(rect, pixel1d, stride, 0);

            pixel1d = null;  // Make sure this is removed from memory.

        }

        /// <summary>
        /// Does the same thing but takes a 1d array
        /// </summary>
        /// <param name="arr">A 2d array with pixel values reprisented by 4 bytes</param>
        /// <param name="bm">The bitmap object that is to be written to</param>
        public static void WriteArrToBM(ref byte[] arr, int width, int height, WriteableBitmap bm)
        {
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            int stride = 4 * width;

            bm.WritePixels(rect, arr, stride, 0);
        }
    }
}
