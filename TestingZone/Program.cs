using System;
using System.Collections.Generic;
using FormulaParser;
using System.Numerics;

namespace TestingZone
{
    internal class Program
    {
        static void Main(string[] args)
        {

            RPN test = new RPN("(x+2)+5");

            Dictionary<string, Complex> variables = new Dictionary<string, Complex> { ["x"] = 3 };

            Console.WriteLine(test.RPNString);

            Console.WriteLine(test.ComputeComplex(variables));


        }
    }
}
