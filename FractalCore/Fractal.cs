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
        }

        public Fractal(int width, int height, FractalFrame fractalFrame, IIterator iterator) : base()
        {

            _width = width;
            _height = height;
            _iterator = iterator;
            _iterationsArray = new uint[height, width];

            Name = fractalFrame.Name;
            Left = fractalFrame.Left;
            Right = fractalFrame.Right;
            Bottom = fractalFrame.Bottom;
            Top = fractalFrame.Top;
            Iterations = fractalFrame.Iterations;
            Iterations = fractalFrame.Iterations;
            Bail = fractalFrame.Bail;
        }

        /// <summary>
        /// Only use for really small fractals
        /// </summary>
        public void Generate()
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

                    uint iterations = IteratePoint(point);

                    _iterationsArray[y, x] = iterations;
                }

            }
        }

        /// <summary>
        /// This method is responsible for creating a bunch of complex numbers to check if they are in the mandelbrot set or not
        /// </summary>
        public async Task GenerateProgressAsync(IProgress<RenderProgressModel> progress, CancellationToken cancellationToken)
        {
            float realStep = (RealWidth / _width);
            float iStep = (ImaginaryHeight / _height);

            RenderProgressModel report = new RenderProgressModel();

            Complex point;

            // Iterate through every pixel on the complex plane
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    cancellationToken.ThrowIfCancellationRequested();  // CANCEL HERE IF TOKEN ACTIVATED

                    float realCoord = (realStep * x) + Left;
                    float imaginaryCoord = (iStep * y) + Bottom;

                    point = new Complex(realCoord, imaginaryCoord);

                    uint iterations = await Task.Run(() => IteratePoint(point));

                    _iterationsArray[y, x] = iterations;
                }

                // Add one as y starts at 0
                report.PercentageComplete = ((y+1) * 100) / _height;
                progress.Report(report);
            }
        }

        public async Task GenerateAsync()
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

                    uint iterations = await Task.Run(() => IteratePoint(point));

                    _iterationsArray[y, x] = iterations;
                }

            }
        }

        public uint IteratePoint(Complex p)
        {
            return _iterator.Iterate(p, Iterations, Bail);
        }
    }
}
