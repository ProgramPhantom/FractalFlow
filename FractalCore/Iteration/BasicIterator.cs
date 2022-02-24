using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FormulaParser;

namespace FractalCore
{
    /// <summary>
    /// This class holds the method that is used to iterate a point on the compelx plane to see if it diverges or converges
    /// </summary>
    public class BasicIterator : IIterator
    {
        private string _formulaString;
        private string _name;
        private RPN _formulaObject;

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


        public RPN FormulaObject
        {
            get { return _formulaObject; }
            set { _formulaObject = value; }
        }


        public BasicIterator(string formulaString, string name="Unitiled")
        {
            _formulaString = Regex.Replace(formulaString, @"\s+", "");
            _name = name;

            _formulaObject = new RPN(_formulaString);
        }

        public uint Iterate(Complex c, uint maxIterations, int bail)
        {
            uint currentIterations = 0;
            Complex z = Complex.Zero;
            Dictionary<string, Complex> variables = new Dictionary<string, Complex>()
            {
                ["z"] = z,
                ["c"] = c
            };

            for (int i = 0; i < maxIterations; i++)
            {
                if (Complex.Abs(z) > bail)
                {
                    break;
                } else
                {
                    currentIterations++; // Increment current iterations!
                }

                variables["z"] = z;

                z = _formulaObject.ComputeComplex(variables);  // Calculate the next z value!!!
            }

            return currentIterations;
        }
    }
}
