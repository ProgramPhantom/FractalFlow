using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FractalCore
{
    public delegate void ComplexHovered(Complex c);

    /// <summary>
    /// This class presents a boundary and information on iterations for making a iterations array.
    /// </summary>
    public class FractalFrame : BaseScaffold
    {
        #region Events
        public event ComplexHovered? ComplexHoveredEvent;
        #endregion

        #region Fields
        private float _left;
        private float _right;
        private float _bottom;
        private float _top;
        private string _name;
        private uint _iterations;
        private int _bail;
        #endregion

        #region Properties
        public float Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public float Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public float Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        public float Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public uint Iterations
        {
            get { return _iterations; }
            set { _iterations = value; }
        }

        public int Bail
        {
            get { return _bail; }
            set { _bail = value; }
        }

        public float RealWidth { get { return Math.Abs(_left) + Math.Abs(_right); } }

        public float ImaginaryHeight { get { return Math.Abs(_bottom) + Math.Abs(_top); } }
        #endregion

        public FractalFrame()
        {
            _left = LeftDefault;
            _right = RightDefault;
            _top = TopDefault;
            _bottom = BottomDefault;
            _iterations = IterationsDefault;
            _bail = BailDefault;
            _name = NameDefault;
        }

        public FractalFrame(FractalFrame fractalFrame)
        {
            _left = fractalFrame.Left;
            _right = fractalFrame.Right;
            _top = fractalFrame.Top;
            _bottom = fractalFrame.Bottom;
            _iterations = fractalFrame.Iterations;
            _bail = fractalFrame.Bail;
            _name = fractalFrame.Name;
        }

        public FractalFrame(float left, float right, float top, float bottom, string name, uint iterations, int bail)
        {
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
            _iterations = iterations;
            _bail = bail;
            _name = name;
        }

        public void PxToComplex(Point p, double width, double height)
        {
            double xStep = RealWidth / width;
            double yStep = ImaginaryHeight / height;

            double real = Left + xStep * p.X;
            double imaginary = Top - (yStep * p.Y);

            Complex pos = new Complex(real, imaginary);

            // Event trigger
            ComplexHoveredEvent?.Invoke(pos);
        }
    }
}
