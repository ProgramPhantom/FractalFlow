using System;
using System.Collections.Generic;
using System.IO;
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
        Dictionary<string, string> constantTranslation = new Dictionary<string, string>()
        {
            ["pi"] = "M_PI",
            ["e"] = "M_E"
        };

        Dictionary<string, string> variableTranslation = new Dictionary<string, string>()
        {
            ["z"] = "z1",
            ["c"] = "c"
        };

        Dictionary<string, string> functionTranslation = new Dictionary<string, string>()
        {
            
            ["log"] = "clog",
            ["log10"] = "clog10",
            ["exp"] = "cexp",

            ["pos"] = "cpos",
            ["arg"] = "carg",
            ["arg"] = "carg",
            ["conj"] = "cconj",
            ["abs"] = "cabs",

            ["sqrt"] = "csqrt",

            ["sin"] = "csin",
            ["cos"] = "ccos",
            ["tan"] = "ctan",

            ["sinh"] = "csinh",
            ["cosh"] = "ccosh",
            ["tanh"] = "ctanh",

            ["asin"] = "casin",
            ["acos"] = "cacos",
            ["atan"] = "catan",

            ["asinh"] = "casinh",
            ["acosh"] = "cacosh",
            ["atanh"] = "catanh",


        };

        Dictionary<string, string> operatorTranslation = new Dictionary<string, string>()
        {
            ["+"] = "cadd",
            ["-"] = "csub",
            ["*"] = "cmul",
            ["/"] = "cdiv",
            ["^"] = "cpow"

        };

        public string[] IterateCLLines;
        public int InsertLine = 319;
        public string IterateCLPath = @"C/IterateCL.c";

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

        public List<string> IterationsCode { get; set; }

        public string FullIterationScript { get; set; }


        public BasicIterator(string formulaString, string name="Unitiled")
        {
            _formulaString = Regex.Replace(formulaString, @"\s+", "");
            _name = name;

            _formulaObject = new RPN(_formulaString);

            IterationsCode = new List<string>();
            RPNToCL RPNToCLObj = new RPNToCL(_formulaObject, "z1", constantTranslation, variableTranslation, functionTranslation, operatorTranslation);
            IterationsCode.Add(RPNToCLObj.CCode);


            // Open IterateCL file
            IterateCLLines = File.ReadAllLinesAsync(IterateCLPath).Result;
            IterateCLLines[InsertLine - 1] = IterationsCode[0];  // Put the computation line at the special line
            // It would be better to search for a specific string and replace it so if lines are changed it does not completely break

            FullIterationScript = String.Join("\n", IterateCLLines);

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

                z = FormulaObject.ComputeComplex(variables);  // Calculate the next z value!!!
            }

            return currentIterations;
        }
    }
}
