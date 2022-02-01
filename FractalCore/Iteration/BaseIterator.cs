using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FractalCore.Iteration
{
    enum TokenType { Number, Varible, Function, Parenthesis, Operator, Comma, WhiteSpace }
    struct Token {
        public TokenType Type { get; }

        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    public class BaseIterator
    {
        public static Dictionary<char, byte> PrecedenceTable = new Dictionary<char, byte>() {
            { '^', 4 },
            {'*', 3 },
            {'/', 3 },
            {'+', 2},
            { '-', 2}, 
            {'s', 2}};  // Sin

        public static Dictionary<string, string> FunctionTranslation = new Dictionary<string, string>() {
            { "sin", "ś"},
            { "cos", "ć"},
            { "tan", "ť" } };



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

        public void ParseInfix(string formula)
        {
            string parsedFormula = formula;
            // Fist, replace functions with singular letters:
            foreach (KeyValuePair<string, string> entry in FunctionTranslation)
            {
                parsedFormula.Replace(entry.Key, entry.Value);
            }

            // Implement Dijkstra's Shunting Yard algorithm

            Stack<char> input = new Stack<char>();
            formula.Reverse().ToList().ForEach((c) => input.Push(c));  // Create a stack of the input formula.

            Stack<char> operatorStack = new Stack<char>();
            Stack<char> output = new Stack<char>();

            foreach (char c in input)
            {
                if (char.IsNumber(c))
                {
                    // Token is number
                    output.Push(input.Pop());  // Put it on the output
                } else if (FunctionTranslation.ContainsKey(c.ToString()))
                {
                    // The token is a function
                    operatorStack.Push(input.Pop());
                } else if (c == '(')
                {

                } else if (c == ')')
                {

                } else if (PrecedenceTable.ContainsKey(c))
                {
                    // An operator
                    if (operatorStack.First() == 0)
                    {

                    }
                }

            }

            
        }
    }
}
