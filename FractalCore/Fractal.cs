using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;

namespace FractalCore
{
    public class Fractal : FractalFrame
    {
        #region Fields
        private IIterator _iterator;
        private uint[,] _iterationsArray;

        private int _height;
        private int _width;

        private bool _rendered = false;

        private FractalFrame _fractalFrame;


        private double _realStep;
        private double _imagStep;

        #endregion


        #region Properties
        public IIterator Iterator
        {
            get { return _iterator; }
            set { _iterator = value; }
        }

        public FractalFrame FractalFrame
        {
            get { return _fractalFrame; }
            set { _fractalFrame = value; }
        }

        public uint[,] IterationsArray
        {
            get { return _iterationsArray; }
            set { _iterationsArray = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public bool Rendered
        {
            get { return _rendered; }
            set { _rendered = value; }
        }


        public double RealStep
        {
            get { return _realStep; }
            set { _realStep = value; }
        }

        public double ImagStep
        {
            get { return _imagStep; }
            set { _imagStep = value; }
        }



        #endregion


        #region Constructors
        public Fractal(int width, int height, IIterator iterator) : base(new FractalFrame())
        {
            _iterator = iterator;
            _iterationsArray = new uint[height, width];

            _width = width;
            _height = height;

            _fractalFrame = new FractalFrame();


            _realStep = _fractalFrame.RealWidth / width;
            _imagStep = _fractalFrame.ImaginaryHeight / height;
        }

        public Fractal(int width, int height, FractalFrame fractalFrame, IIterator iterator) : base(fractalFrame)
        {

            _width = width;
            _height = height;
            _iterator = iterator;
            _iterationsArray = new uint[height, width];

            _fractalFrame = fractalFrame;

            _realStep = _fractalFrame.RealWidth / width;
            _imagStep = _fractalFrame.ImaginaryHeight / height;


        }
        #endregion

        #region Methods
        public uint IteratePoint(Complex p)
        {
            return Iterator.Iterate(p, Iterations, Bail);
        }
        #endregion
    }
}
