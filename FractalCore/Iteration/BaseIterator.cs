using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore.Iteration
{
    public class BaseIterator
    {
        public static Dictionary<char, byte> PrecedenceTable = new Dictionary<char, byte>() {
            { '^', 4 },
            {'*', 3 },
            {'/', 3 }, 
            {'+', 2}, 
            { '-', 2} };

        /// <summary>
        /// Takes the 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Complex RPNExec(Complex z, Complex c)
        {
            throw new NotImplementedException();
        }

        public string[] ParseInfix(string formula)
        {
            // Implement Dijkstra's Shunting Yard algorithm

            Stack<char> input = new Stack<char>();
            formula.Reverse().ToList().ForEach((c) => input.Push(c));  // Create a stack of the input formula.

            
        }
    }
}
