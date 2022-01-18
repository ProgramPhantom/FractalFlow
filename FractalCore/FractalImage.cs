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

        private WriteableBitmap _fractalBitmap;

        public WriteableBitmap FractalBitmap
        {
            get { return _fractalBitmap; }
        }

        private IPainter _painter;

        public IPainter Painter
        {
            get { return _painter; }
            set { _painter = value; }
        }


        public FractalImage(ref Fractal fractal, IPainter painter)
        {
            // Need to now create the writeable bitmap with the colours
            _fractalBitmap = new WriteableBitmap(fractal.Width, fractal.Height, 96, 96, PixelFormats.Bgr32, null);  // Set up an empty WriteableBitmap with the correct dimensions
            // painter.Paint(ref _fractalBitmap, ref fractal);  // Paint the 

            _painter = painter;

            Painter.Paint(ref _fractalBitmap, ref fractal);

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
