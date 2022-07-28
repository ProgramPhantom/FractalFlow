using FractalCore.Painting;
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


        #endregion

        #region Properties
        public WriteableBitmap FractalBitmap
        {
            get { return _fractalBitmap; }
        }

        public IPainter? CurrentPaint = null;

        public int Width;

        public int Height;
        #endregion

        public FractalImage(int width, int height)
        {
            // Need to now create the writeable bitmap with the colours
            _fractalBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);  // Set up an empty WriteableBitmap with the correct dimensions

            Width = width;
            Height = height;
        }

        public void Render(ref IPainter painter, ref Fractal fractal)
        {
            if (!(fractal.Width == Width && fractal.Height == Height))
            {
                throw new Exception("Cannot draw that fractal to this fractal image");
            }

            CurrentPaint = painter;
            painter.Paint(ref _fractalBitmap, ref fractal);
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
