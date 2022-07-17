using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace FractalCore
{

    [Serializable]
    public class Fractal : FractalFrame
    {
        #region Fields
        private BasicIterator _iterator;

        [XmlIgnore]
        private uint[,]? _iterationsArray;

        private int _height;
        private int _width;


        private FractalFrame _fractalFrame;


        private double _realStep;
        private double _imagStep;

        #endregion


        #region Properties
        public BasicIterator Iterator
        {
            get { return _iterator; }
            set { _iterator = value; }
        }

        public FractalFrame FractalFrame
        {
            get { return _fractalFrame; }
            set { _fractalFrame = value; }
        }

        [XmlIgnore]
        public uint[,]? IterationsArray
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
        internal Fractal()
        {
            _iterator = new BasicIterator();
            _fractalFrame = new FractalFrame();
        }

        public Fractal(int width, int height, BasicIterator iterator) : base(new FractalFrame())
        {
            _iterator = iterator;
            _iterationsArray = new uint[height, width];

            _width = width;
            _height = height;

            _fractalFrame = new FractalFrame();


            _realStep = _fractalFrame.RealWidth / width;
            _imagStep = _fractalFrame.ImaginaryHeight / height;
        }

        public Fractal(int width, int height, FractalFrame fractalFrame, BasicIterator iterator) : base(fractalFrame)
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


        public static void Save(Fractal f)
        {
            string xml;
            XmlSerializer s = new XmlSerializer(f.GetType());

            using (MemoryStream memoryStream = new MemoryStream())
            {
                s.Serialize(memoryStream, f);
                memoryStream.Position = 0;
                xml = new StreamReader(memoryStream).ReadToEnd();
            }

            System.Diagnostics.Debug.WriteLine(xml);
        }
        #endregion
    }
}
