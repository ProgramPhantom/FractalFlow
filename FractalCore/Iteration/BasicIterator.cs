using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore
{
    /// <summary>
    /// This class holds the method that is used to iterate a point on the compelx plane to see if it diverges or converges
    /// </summary>
    public class BasicIterator : IIterator
    {
        private string _formulaString;
        private string _name;

        public string FormulaString
        {
            get { return _formulaString; }
            set { _formulaString = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public BasicIterator(string name, string formulaString)
        {
            _formulaString = FormulaString;
            _name = name;
        }

        public int Iterate(Complex c, int maxIterations, int bail)
        {
            throw new NotImplementedException();
        }
    }
}
