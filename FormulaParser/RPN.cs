using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

#nullable enable


namespace FormulaParser
{
    public enum TokenType { Constant, Variable, Function, OpenParenthesis, CloseParenthesis, Operator, WhiteSpace}

    public struct Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Operator? Operation { get; }

        public Token(TokenType type, string value, Operator? operation=null)
        {
            Type = type;
            Value = value;
            Operation = RPN.DetermineType(value) == TokenType.Operator ? RPN.Operators[value] : null;
        }
    }

    public struct Operator
    {
        public string Name { get; set; }
        public int Precedence { get; set; }
        public bool RightAssociative { get; set; }

        public Operator(string name, int precedence, bool rightAssociative=false)
        {
            Name = name;
            Precedence = precedence;
            RightAssociative = rightAssociative;
        }
    }



    public class RPN
    {
        private string _infixString;


        public readonly static Dictionary<string, Operator> Operators = new Dictionary<string, Operator>()
        {
            ["+"] = new Operator("+", 1),
            ["-"] = new Operator("-", 1),
            ["*"] = new Operator("*", 2),
            ["/"] = new Operator("/", 2),
            ["^"] = new Operator("^", 3, true),
        };

        public static string[] Functions = new string[] { "sin", "cos", "tan" };

        public static string[] Constants = new string[] { "pi", "e" };


        public List<Token> Tokens { get; set; }

        public string InfixString
        {
            get { return _infixString; }
            set { _infixString = value; }
        }

        public RPN(string infix)
        {
            _infixString = infix;

        }

        private bool CompareOperators(Operator op1, Operator op2) => op1.RightAssociative ? op1.Precedence < op2.Precedence : op1.Precedence <= op2.Precedence;

        public static TokenType DetermineType(string str)
        {
            if (Regex.IsMatch(str, @"^[a-zA-Z]+$")) // All letters?
                if (RPN.Functions.Contains(str))
                {
                    return TokenType.Function;
                } else
                {
                    return TokenType.Variable;
                }

            if (Regex.IsMatch(str, @"^[0-9]+$"))  // All numbers?
                return TokenType.Constant;

            if (string.IsNullOrWhiteSpace(str))
                return TokenType.WhiteSpace;

            if (str == "(")
                return TokenType.OpenParenthesis;

            if (str == ")")
                return TokenType.CloseParenthesis;

            if (Operators.ContainsKey(str))
                return TokenType.Operator;

            throw new Exception($"Unknown character {str}");
        }

        public List<Token> Tokenize(string infix)
        {
            List<Token> tokens = new List<Token>();

            TextReader textReader = new StringReader(infix);

            TokenType currType;
            TokenType nextType;
            int curr;
            char ch;
            int next;
            char nextCh;
            bool parenthesis;

            string tokenString = "";
            StringBuilder tokenS = new StringBuilder();

            while ((curr = textReader.Read()) != -1)
            {
                ch = (char)curr;
                currType = DetermineType(ch.ToString());
                next = textReader.Peek();
                nextCh = (char)next;
                nextType = next != -1 ? DetermineType(nextCh.ToString()) : TokenType.WhiteSpace;
                parenthesis = (currType == TokenType.CloseParenthesis || currType == TokenType.OpenParenthesis) ? true : false;  // Is it a parenthesis?

                if (currType == TokenType.WhiteSpace) { continue; }

                tokenString.Append(ch);

                if (parenthesis)
                {
                    tokenString = "";
                    continue;
                }

                if (currType != nextType)  // A change in type
                {
                    if (currType == TokenType.Constant)  // If it is a multi-digit number eg "7432"
                    {
                        if (nextType == TokenType.OpenParenthesis)
                        {
                            tokens.Add(new Token(TokenType.Constant, tokenString));
                            tokens.Add(new Token(TokenType.Operator, "*"));
                        } else if (nextType == TokenType.Variable)
                        {
                            tokens.Add(new Token(TokenType.Constant, tokenString));
                            tokens.Add(new Token(TokenType.Operator, "*"));
                        }
                    } else if (currType == TokenType.Variable)
                    {  //    Determine type on the entire tokenString
                        if (DetermineType(tokenString) == TokenType.Function)
                        {
                            tokens.Add(new Token(TokenType.Function, tokenString));
                            if (nextType != TokenType.CloseParenthesis)
                            {  // Open bracket must follow a function
                                throw new Exception($"There must be a bracket after function {tokens.LastOrDefault()}");
                            } 
                        } else if (nextType == TokenType.OpenParenthesis)  // 1, 
                        {
                            // Must be a varible
                            tokens.Add(new Token(TokenType.Variable, tokenString));
                            tokens.Add(new Token(TokenType.Operator, "*"));

                        }
                    }

                    tokenString = "";

                }
            }

            return tokens;
        }

    }
}