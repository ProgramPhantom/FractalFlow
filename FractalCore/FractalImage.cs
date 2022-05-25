using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace FractalCore
{
    /// <summary>
    /// This class takes a Fractal object and uses it's iterations array to create an image using a colouring algorithm to be displayed on the screen
    /// </summary>
    public class FractalImage
    {
        #region Fields
        private WriteableBitmap _fractalBitmap;

        private Fractal _fractal;
        #endregion

        #region Properties
        public WriteableBitmap FractalBitmap
        {
            get { return _fractalBitmap; }
        }



        public Fractal Fractal
        {
            get { return _fractal; }
            set { _fractal = value; }
        }
        #endregion

        public FractalImage(ref Fractal fractal)
        {
            // Need to now create the writeable bitmap with the colours
            _fractalBitmap = new WriteableBitmap(fractal.Width, fractal.Height, 96, 96, PixelFormats.Bgr32, null);  // Set up an empty WriteableBitmap with the correct dimensions
            // painter.Paint(ref _fractalBitmap, ref fractal);  // Paint the 

            _fractal = fractal;
        }

        ///// <summary>
        ///// Temporary method for creating an image straight from a flat array rather than taking a 2D array
        ///// </summary>
        ///// <param name="flatArray"></param>
        ///// <param name="width"></param>
        ///// <param name="height"></param>
        ///// <param name="painter"></param>
        //public FractalImage(ref uint[] flatArray, int width, int height, BasicPainter painter)
        //{
        //    // Need to now create the writeable bitmap with the colours
        //    _fractalBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);  // Set up an empty WriteableBitmap with the correct dimensions
        //    // painter.Paint(ref _fractalBitmap, ref fractal);  // Paint the 

        //    _painter = painter;

            

        //}

        public void Render(IPainter painter)
        {
            painter.Paint(ref _fractalBitmap, ref _fractal);
        }

        public void SaveImage(string filename)
        {
            if (filename != string.Empty)
            {
                using (FileStream img = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(_fractalBitmap));
                    encoder5.Save(img);
                }
            }
        }

    }
}
