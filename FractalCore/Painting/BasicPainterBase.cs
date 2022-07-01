using OpenCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FractalCore.Painting
{
    public abstract class BasicPainterBase : PainterBase, IPainter
    {

        #region Fields
        private Color _inSetColour;
        private Color _mainColour;


        #endregion

        #region Properties

        public Color InSetColour
        {
            get { return _inSetColour; }
            set { _inSetColour = value; }
        }

        public Color MainColour
        {
            get { return _mainColour; }
            set { _mainColour = value; }
        }

        public abstract string PaintCLScript { get; set; }
        #endregion

        public BasicPainterBase(string name, Color mainColour, Color inSetColour) : base(name)
        {

            _inSetColour = inSetColour;
            _mainColour = mainColour;

        }

        /// <summary>
        /// This HAS to be here so BasicPainterViewModelAbstract can reference BasicPainterBase's Painter model
        /// </summary>
        /// <param name="fractalBitmap"></param>
        /// <param name="fractal"></param>
        public abstract void Paint(ref WriteableBitmap fractalBitmap, ref Fractal fractal);

        public abstract void SetKernelParameters(ref MultiCL kernel, ref byte[] pixels, ref uint[] iterations, uint iterationsCap);
    }
}
