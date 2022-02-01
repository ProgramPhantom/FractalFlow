using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FormulaParser
{
    class InfixParser
    {
        /// <summary>
        /// <see cref="Enum"/> to hold types of <see cref="Token"/>
        /// </summary>
        enum TokenType { Number, Variable, Function, OpenParenthesis, CloseParenthesis, Operator, Comma, WhiteSpace };

        /// <summary>
        /// <see cref="struct"/> to hold type and value of each character in the formula string
        /// </summary>
        struct Token
        {
            /// <summary>
            /// Type of token (<see cref="enum"/>)
            /// </summary>
            public TokenType Type { get; }

            /// <summary>
            /// Value of token (eg: 6, +, etc)
            /// </summary>
            public string Value { get; }

            public override string ToString() => $"{Type}: {Value}";

            public Token(TokenType type, string value)
            {
                Type = type;
                Value = value;
            }
        }

        /// <summary>
        /// Operator object to hold information on how the Shunting-Yard algorithm should move it around the yard.
        /// </summary>
        class Operator
        {
            /// <summary>
            /// Name of operator.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// The order in which to execute the operations, the larger, the higher prioirty the operation has.
            /// </summary>
            public int Precedence { get; set; }
            /// <summary>
            /// Distuinguishes if this operator associates with the right or left 
            /// </summary>
            public bool RightAssociative { get; set; }

            public Operator(string name, int precedence, bool associativity = false)
            {
                Name = name;
                Precedence = precedence;
                RightAssociative = associativity;
            }
        }

        /// <summary>
        /// Parser class
        /// </summary>
        class Parser
        {
            /// <summary>
            /// Dictionary setting up the operator objects with their 
            /// </summary>
            private IDictionary<string, Operator> operators = new Dictionary<string, Operator>
            {
                ["+"] = new Operator("+", 1),
                ["-"] = new Operator("-", 1),
                ["*"] = new Operator("*", 2),
                ["/"] = new Operator("/", 2),
                ["^"] = new Operator("^", 3, true),
            };

            /// <summary>
            /// Used for compaing <see cref="FormulaParser.Operator"/> to see which will be placed in the output queue.
            /// </summary>
            private bool CompareOperators(Operator op1, Operator op2) => op1.RightAssociative ? op1.Precedence < op2.Precedence : op1.Precedence <= op2.Precedence;

            /// <summary>
            /// Compare operators but using operator in string form, references the <see cref="operators"/> Dictionary to get the right operator
            /// </summary>
            private bool CompareOperators(string op1, string op2) => CompareOperators(operators[op1], operators[op2]);

            /// <summary>
            /// Determine the type of token from a character input
            /// </summary>
            /// <param name="ch">Character to be determined</param>
            /// <exception cref="Exception">The parser could not figure out what type of token that character is</exception>
            private TokenType DetermineType(char ch)
            {
                if (char.IsLetter(ch))
                    return TokenType.Variable;
                if (char.IsDigit(ch))
                    return TokenType.Number;
                if (char.IsWhiteSpace(ch))
                    return TokenType.WhiteSpace;
                if (ch == ',')
                    return TokenType.Comma;
                if (ch == '(')
                    return TokenType.OpenParenthesis;
                if (ch == ')')
                    return TokenType.CloseParenthesis;
                if (operators.ContainsKey(Convert.ToString(ch)))
                    return TokenType.Operator;

                throw new Exception("Wrong character");
            }

            /// <summary>
            /// Iterable method for creating a list of tokens from a string.
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            public List<Token> Tokenize(TextReader reader)
            {
                var tokenString = new StringBuilder(); // Set up
                List<Token> tokens = new List<Token>();

                int curr;  // Clever!
                while ((curr = reader.Read()) != -1) // Loop until got to the end of the text reader
                {

                    char ch = (char)curr;  // Get the character we are on
                    TokenType currType = DetermineType(ch);  // Get the token type of this character
                    if (currType == TokenType.WhiteSpace)  // Get the whitespace outta here
                        continue;

                    bool parenthesis = (currType == TokenType.CloseParenthesis || currType == TokenType.OpenParenthesis) ? true : false;  // Is it a parenthesis?

                    tokenString.Append(ch);  // Add the character to the string 

                    int next = reader.Peek();  // Get the next character
                    TokenType nextType = next != -1 ? DetermineType((char)next) : TokenType.WhiteSpace;  // If the next is the end, set to white space, so it always goes into the if

                    if (currType != nextType || parenthesis)  // So if "sin", keep going until next token is not letter, then package it up and send it off as a function with that name. 
                                                              // Cases where there are two brackets in a row eg "((" should be added as unique tokens, not one with the value "((", thus if on a parenthesis, always add it immediately
                    {
                        if (nextType == TokenType.OpenParenthesis && !parenthesis)  // If it was something like "sin(", it knows what it had been reading is a function.
                            tokens.Add(new Token(TokenType.Function, tokenString.ToString()));  // Send off the function

                        else
                            tokens.Add(new Token(currType, tokenString.ToString()));  // Everything else would have been a singular letter, and therefore not a function

                        tokenString.Clear();
                    }

                }

                return tokens;
            }

            /// <summary>
            /// Apply the Shunting-Yard algorithm on a list of tokens
            /// </summary>
            public IEnumerable<Token> ShuntingYard(List<Token> tokens)
            {
                var operatorStack = new Stack<Token>();  // Create a new stack for the operator tokens to be added to
                foreach (var tok in tokens)  // Iterate throught the tokens
                {
                    switch (tok.Type)  // Set up a switch case involving the type of the token
                    {
                        case TokenType.Number:
                        case TokenType.Variable: // If number or varible put straight into the output
                            yield return tok;
                            break;
                        case TokenType.Function:  // If function, put on the op stack
                            operatorStack.Push(tok);
                            break;
                        case TokenType.Comma:
                            while (operatorStack.Peek().Value != "(")  // Do mysterious stuff
                                yield return operatorStack.Pop();
                            break;
                        case TokenType.Operator:
                            //   Any elements in opstack?    Top token is an operator?             Is the current token's precedeance less than the token on top?
                            while (operatorStack.Any() && operatorStack.Peek().Type == TokenType.Operator && CompareOperators(tok.Value, operatorStack.Peek().Value))
                                yield return operatorStack.Pop(); // Then put that on the output!
                            operatorStack.Push(tok);  // Then put the operator on top
                            break;
                        case TokenType.OpenParenthesis:
                            operatorStack.Push(tok);
                            break;
                        case TokenType.CloseParenthesis:
                            while (operatorStack.Peek().Type != TokenType.OpenParenthesis) { yield return operatorStack.Pop(); }   // Pop off the opstack until the open bracket is found

                            operatorStack.Pop();  // Then pop off the open bracket

                            if (operatorStack.Count() != 0) // If not empty
                            {
                                if (operatorStack.Peek().Type == TokenType.Function)
                                    yield return operatorStack.Pop();  // If it was a function before that, add it to the output
                            }


                            break;
                        default:
                            throw new Exception("Wrong token");
                    }
                }
                while (operatorStack.Any()) // If any operators left on the opstack
                {
                    var tok = operatorStack.Pop();
                    if (tok.Type == TokenType.OpenParenthesis || tok.Type == TokenType.CloseParenthesis)
                        throw new Exception("Mismatched parentheses");  // Invalid infix! ):
                    yield return tok;
                }
            }
        }
    }
}
