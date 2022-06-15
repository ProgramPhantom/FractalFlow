using System;
using System.Collections.Generic;
using FormulaParser;
using System.Numerics;
using FractalCore;

namespace TestingZone
{
    internal class Program
    {
        static void Main(string[] args)
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
                ["sin"] = "csin",
                ["cos"] = "ccos",
                ["tan"] = "ctan",
                ["log"] = "clog",
                ["log10"] = "clog10",
                ["exp"] = "cexp",
                ["abs"] = "cabs"
            };

            Dictionary<string, string> operatorTranslation = new Dictionary<string, string>()
            {
                ["+"] = "cadd",
                ["-"] = "csub",
                ["*"] = "cmul",
                ["/"] = "cdiv",
                ["^"] = "cpow"

            };


            RPN test = new RPN("c*z^2 + 1 + (c*z^2 +1)^-1");

            Dictionary<string, Complex> variables = new Dictionary<string, Complex> { ["z"] = 3, ["c"] = 1 };

            Console.WriteLine(test.RPNString);

            Console.WriteLine(test.ComputeComplex(variables));

            List<string> list = test.GenerateOpenCLC("z", "z1", new Dictionary<string, string>() { ["pi"] = "M_PI", ["e"] = "M_E" });

            RPNToCL clCode = new RPNToCL(test, "z1", constantTranslation, variableTranslation, functionTranslation, operatorTranslation);

            
             

        }
    }
}
