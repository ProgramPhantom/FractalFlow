using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    /// <summary>
    /// This class presents a boundary and information on iterations for making a iterations array.
    /// </summary>
    public class FractalFrame : BaseScaffold
    {
        

        private float _left;
        private float _right;
        private float _bottom;
        private float _top;
        private string _name;
        private uint _iterations;
        private int _bail;


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
    }
}
