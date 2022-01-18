using System;
using System.Numerics;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using System.IO;

namespace FractalCore
{
    public class Fractal
    {

        #region Varible Setup
        /// <summary>
        /// The big one, this array holds the number of iterations each complex point went through before triggering the bailout.
        /// </summary>
        private UInt16[,] _iterationsArray;  // MAX ITERATIONS WILL BE 65,536. ADD VALIDATION LATER
        public static int MaxIterations = 65536;

        private const int WIDTH = 1000;
        private const int HEIGHT = 1000;
        private const float TOP = 1.1f;
        private const float BOTTOM = -1.1f;
        private const float LEFT = -2f;
        private const float RIGHT = 0.7f;
        private const int ITERATIONS = 64;
        private const string NAME = "Untitled";
        private const int BAIL = 2;

        private int _width;
        private int _height;
        private float _topBound;
        private float _bottomBound;
        private float _leftBound;
        private float _rightBound;
        private string _name;

        private int _iterationCap;

        private float _planeWidth;
        private float _planeHeight;

        /// <summary>
        /// The width in pixels of the fractal image
        /// </summary>
        public int Width { get { return _width; } }
        /// <summary>
        /// The height in pixels of the fractal image
        /// </summary>
        public int Height { get { return _height; } }
        /// <summary>
        /// The top boundary for the image on the complex plain as a float
        /// </summary>
        public float TopBound { get { return _topBound; } }
        /// <summary>
        /// The bottom boundary for the image on the complex plain as a float
        /// </summary>
        public float BottomBound { get { return _bottomBound; } }
        /// <summary>
        /// The bottom boundary for the image on the complex plain as a float
        /// </summary>
        public float LeftBound { get { return _leftBound; } }
        /// <summary>
        /// The right boundary for the image on the complex plain as a float
        /// </summary>
        public float RightBound { get { return _rightBound; } }
        /// <summary>
        /// The number of times the complex number is put through the recursive formula until the code 
        /// decides the point is in the set
        /// </summary>
        public int IterationCap { get { return _iterationCap; } }

        public float PlaneWidth { get { return Math.Abs(_rightBound) + Math.Abs(_leftBound); } }

        public float PlaneHeight { get { return Math.Abs(_bottomBound) + Math.Abs(_topBound); } }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public UInt16[,] IterationsArray { get { return _iterationsArray; } }

        

        #endregion

        /// <summary>
        /// Create a new Mandelbrot set fractal
        /// </summary>
        /// <remarks>
        /// You can leave everything blank for a default Mandelbrot set
        /// </remarks>
        /// <param name="width">Width (px)</param>
        /// <param name="height">Height (px)</param>
        /// <param name="top">Top (iy)f</param>
        /// <param name="bottom">Bottom (iy)f</param>
        /// <param name="right">Right (ix)f</param>
        /// <param name="left">Left (ix)f</param>
        /// <param name="iterations">Iterations (x)</param>
        public Fractal(int width=WIDTH, int height=HEIGHT, float top=TOP, float bottom=BOTTOM, float left=LEFT, float right=RIGHT, int iterations=ITERATIONS, 
            string name=NAME) 
        {
            _width = width;
            _height = height;
            _topBound = top;
            _bottomBound = bottom;
            _rightBound = right;
            _leftBound = left;
            _iterationCap = iterations;
            _name = name;
            
            _planeWidth = Math.Abs(_rightBound) + Math.Abs(_leftBound);
            _planeHeight = Math.Abs(_bottomBound) + Math.Abs(_topBound);

            _iterationsArray = new UInt16[_height, _width];  // Create the massive array to hold the iterations value.


            IterateComplexPlane();  // Render!
        }

        public Fractal(int width, int height)
        {
            _width = width;
            _height = height;
            _topBound = TOP;
            _bottomBound = BOTTOM;
            _rightBound = RIGHT;
            _leftBound = LEFT;
            _iterationCap = ITERATIONS;
            _name = NAME;

            _planeWidth = Math.Abs(_rightBound) + Math.Abs(_leftBound);
            _planeHeight = Math.Abs(_bottomBound) + Math.Abs(_topBound);

            _iterationsArray = new UInt16[_height, _width];  // Create the massive array to hold the iterations value.


            IterateComplexPlane();  // Render!
        }

        /// <summary>
        /// This method fills the _iterationsArray with iterations based off the classic 
        /// </summary>
        public void MandelbrotGenerate()
        {
            float realStep = (_planeWidth / _width);
            float iStep = (_planeHeight / _height);

            Complex point;

            for (int y = 0; y < _width; y++)
            {
                for (int x = 0; x < _height; x++)
                {
                    float realCoord = (realStep * x) + _leftBound;
                    float imaginaryCoord = (iStep * y) + _bottomBound;

                    point = new Complex(realCoord, imaginaryCoord);

                    _iterationsArray[y, x] = IteratePointMandelbrot(point);
                }
            }

            System.Diagnostics.Trace.WriteLine("Done!");
        }

        /// <summary>
        /// This method iterates over every pixel in the complex plane, and sets the corresponding 
        /// coord in the _iterationsArray to the result of the object's GetIterations method when that 
        /// complex number is the input
        /// </summary>
        private void IterateComplexPlane()
        {
            float realStep = (_planeWidth / _width);
            float iStep = (_planeHeight / _height);

            Complex point;

            // Iterate through every pixel on the complex plane
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float realCoord = (realStep * x) + _leftBound;
                    float imaginaryCoord = (iStep * y) + _bottomBound;

                    point = new Complex(realCoord, imaginaryCoord);

                    _iterationsArray[y, x] = IteratePointMandelbrot(point);  // This will be replaced with whatever recursive formula the fractal is using
                }
            }

            System.Diagnostics.Trace.WriteLine("Done!");
        }

        public ushort IteratePointMandelbrot(Complex c)
        {
            // Create 2 complex numbers.
            Complex current;
            Complex last = new Complex(0, 0);

            ushort i = 0;

            for (i = 0; i < _iterationCap; i++)
            {
                current = Complex.Pow(last, 2) + c;

                if (Complex.Abs(current) > BAIL)
                {
                    return i;
                }

                last = current;  // Move it back one
            }

            return i;  // Does this work?
        }

        public void WriteToFile(string path)
        {
            StreamWriter file = new StreamWriter("path");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    file.Write(this.IterationsArray[y, x].ToString() + "\t");
                }
                file.Write("\n");
            }
            file.Close();
        }
    }
}
