using System;
using FormulaParser;

namespace TestingZone
{
    public class Program
    {
        static void Main(string[] args)
        {
            RPN test = new RPN("z^2 + c");

            Console.WriteLine(test.LaTEX);
        }
    }
}
