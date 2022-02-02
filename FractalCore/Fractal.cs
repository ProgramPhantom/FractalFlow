using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FractalCore
{
    public class Fractal : FractalScaffold
    {
        private IIterator _iterator;
        private uint[,] _iterationsArray;

        private int _height;
        private int _width;


        public IIterator Iterator
        {
            get { return _iterator; }
            set { _iterator = value; }
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


        public Fractal(int width, int height, IIterator iterator) : base()
        {
            _iterator = iterator;
            _iterationsArray = new uint[height, width];

            _width = width;
            _height = height;

            IterateComplexPlane();  // RENDER
        }

        private void IterateComplexPlane()
        {
            float realStep = (RealWidth / _width);
            float iStep = (ImaginaryHeight / _height);

            Complex point;

            // Iterate through every pixel on the complex plane
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float realCoord = (realStep * x) + Left;
                    float imaginaryCoord = (iStep * y) + Bottom;

                    point = new Complex(realCoord, imaginaryCoord);

                    _iterationsArray[y, x] = IteratePoint(point);  // This will be replaced with whatever recursive formula the fractal is using
                }
            }

            System.Diagnostics.Trace.WriteLine("Done!");
        }

        public uint IteratePoint(Complex p)
        {
            return _iterator.Iterate(p, Iterations, Bail);
        }
    }
}
