using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ShuntingYardParser
{
	public enum TokenType { Constant, Variable, Integer, Decimal, Function, OpenParenthesis, CloseParenthesis, Operator, WhiteSpace, DecimalPoint}

	public struct Token
	{
		public TokenType Type { get; }
		public string Value { get; }

		public Token(TokenType type, string value)
		{
			Type = type;
			Value = value;

			// Would have had the operator as a nullable property in this struct but it caused some problems ):
		}
	}

	public struct Operator
	{
		public string Name { get; set; }
		public int Precedence { get; set; }
		public bool RightAssociative { get; set; }

		public Operator(string name, int precedence, bool rightAssociative = false)
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

		public static string[] Functions = new string[] { "sin", "cos", "tan", "sqrt", "ln", "log"};
		private List<Token> _RPNTokens;

		// public readonly string[] Constants = new string[] { "pi", "e" };

		public static readonly Dictionary<string, double> Constants = new Dictionary<string, double>()
		{
			["pi"] = Math.PI,
			["e"] = Math.E,
		};


		public List<Token> InfixTokens { get; set; }

		public List<Token> RPNTokens { get => _RPNTokens; set => _RPNTokens = value; }


		public string InfixString
		{
			get { return _infixString; }
			set { _infixString = value; }
		}

		public RPN(string infix)
		{
			if (CheckBrackets(infix))
            {
				_infixString = infix;

			} else {
				
				throw new Exception("Mismatched brackets");
            }


			InfixTokens = Tokenize(infix);
			_RPNTokens = new List<Token>();
		}

		private bool CheckBrackets(string formula)
        {
			int open = formula.Count(c => (c == '('));
			int close = formula.Count(c => (c == ')'));

			if (open == close)
            {
				return true;
            } else
            {
				return false;
            }

        }

		private bool CompareOperators(Operator op1, Operator op2) => op1.RightAssociative ? op1.Precedence < op2.Precedence : op1.Precedence <= op2.Precedence;

		public static TokenType DetermineType(string str)
		{
			if (Regex.IsMatch(str, @"^[a-zA-Z]+$")) // All letters?
				if (RPN.Functions.Contains(str))
				{
					return TokenType.Function;
				} else if (RPN.Constants.TryGetValue(str, out _))
                {
					Console.WriteLine($"{str} is a constant!");
					return TokenType.Constant;
                } else
				{
					return TokenType.Variable;
				}

			if (Regex.IsMatch(str, @"^[0-9]+$"))  // All numbers?
				return TokenType.Integer;

			if (string.IsNullOrWhiteSpace(str))
				return TokenType.WhiteSpace; 

			if (str == "(")
				return TokenType.OpenParenthesis;

			if (str == ")")
				return TokenType.CloseParenthesis;

			if (Operators.ContainsKey(str))
				return TokenType.Operator;

			if (str == ".")
            {
				return TokenType.DecimalPoint;
            }

			if (str.Contains("."))
            {
				return TokenType.Decimal;
            }

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
			int counter = 0;
			bool parenthesis;

			string tokenString = "";





			while ((curr = textReader.Read()) != -1)
			{
				counter++;
				ch = (char)curr;



				currType = DetermineType(ch.ToString());
				next = textReader.Peek();
				nextCh = (char)next;
				nextType = next != -1 ? DetermineType(nextCh.ToString()) : TokenType.WhiteSpace;
				parenthesis = (currType == TokenType.CloseParenthesis || currType == TokenType.OpenParenthesis) ? true : false;  // Is it a parenthesis?

				if (currType == TokenType.WhiteSpace) 
				{ 
					continue; // Remove the whitespace!
				} else
                {
					tokenString += ch;
				}


				if (currType != nextType || parenthesis || currType == TokenType.Operator)  // A change in type
				{
					if (currType == TokenType.Integer)  // CONSTANT
					{
						if (nextType == TokenType.OpenParenthesis)
						{
							tokens.Add(new Token(TokenType.Integer, tokenString));
							tokens.Add(new Token(TokenType.Operator, "*"));
						}
						else if (nextType == TokenType.Variable)
						{
							tokens.Add(new Token(TokenType.Integer, tokenString));
							tokens.Add(new Token(TokenType.Operator, "*"));
						} else if (nextType == TokenType.DecimalPoint)
                        {
							continue;
                        }
						else
						{
							tokens.Add(new Token(TokenType.Integer, tokenString));
						}
					}
					else if (DetermineType(tokenString) == TokenType.Constant)  // Is the bunch of letters a const
					{
						tokens.Add(new Token(TokenType.Constant, tokenString));
					}
					else if (currType == TokenType.Variable)  // VARIABLE eg. "z", "c" bunch of letters
					{  //    Determine type on the entire tokenString
						if (DetermineType(tokenString) == TokenType.Function)  // If it is a function
						{
							tokens.Add(new Token(TokenType.Function, tokenString));
							if (nextType != TokenType.OpenParenthesis)
							{  // Open bracket must follow a function
								throw new Exception($"There must be a bracket after function {tokens.LastOrDefault()}");
							}
						} 
						else if (nextType == TokenType.OpenParenthesis)  // 1, 
						{
							// Must be a varible
							tokens.Add(new Token(TokenType.Variable, tokenString));
							tokens.Add(new Token(TokenType.Operator, "*"));

						}
						else
						{
							tokens.Add(new Token(TokenType.Variable, tokenString));
						}
					} 
					else if (currType == TokenType.DecimalPoint)
                    {
						continue;
                    } 
					else if (currType == TokenType.Operator)
                    {
						if (ch == '-')  // If it's a minus
						{
							if (nextType == TokenType.Constant || nextType == TokenType.Variable || nextType == TokenType.Integer || nextType == TokenType.Decimal)
                            {
								if (counter != 1)  // Do this first to make sure there are no "no tokens in list" errors in the next if
                                {
									if (tokens.Last().Type != TokenType.OpenParenthesis && tokens.Last().Type != TokenType.Operator && counter != 1)
									{
										tokens.Add(new Token(TokenType.Operator, "+"));
										
									}

								}
								continue;  // Skip over replacing the tokenString 

							}
						} else
                        {
							tokens.Add(new Token(currType, tokenString));
						}

					} else if (currType == TokenType.CloseParenthesis && nextType == TokenType.OpenParenthesis)
                    {
						tokens.Add(new Token(currType, tokenString));
						tokens.Add(new Token(TokenType.Operator, "*"));
                    }
					else
					{
						tokens.Add(new Token(currType, tokenString));
					}

					tokenString = "";

				}
			}

			return tokens;
		}

		public void ShuntingYard()
		{
			Stack<Token> operatorStack = new Stack<Token>();
			foreach (var tok in InfixTokens)  // Iterate throught the tokens
			{
				switch (tok.Type)
				{
					case TokenType.Constant:
					case TokenType.Variable: // If number or varible put straight into the output
					case TokenType.Integer:
						RPNTokens.Add(tok);
						break;
					case TokenType.Function:  // If function, put on the op stack
						operatorStack.Push(tok);
						break;
					case TokenType.Operator:
						//   Any elements in opstack?    Top token is an operator?             Is the current token's precedeance less than the token on top?
						while (operatorStack.Any() && operatorStack.Peek().Type == TokenType.Operator && CompareOperators(Operators[tok.Value], Operators[operatorStack.Peek().Value]))
                        {
							RPNTokens.Add(operatorStack.Pop()); // Then put that on the output!
						}
						operatorStack.Push(tok);  // Then put the operator on top
						break;
					case TokenType.OpenParenthesis:
						operatorStack.Push(tok);
						break;
					case TokenType.CloseParenthesis:
						while (operatorStack.Peek().Type != TokenType.OpenParenthesis) { RPNTokens.Add(operatorStack.Pop()); }   // Pop off the opstack until the open bracket is found

						operatorStack.Pop();  // Then pop off the open bracket

						if (operatorStack.Count() != 0) // If not empty
						{
							if (operatorStack.Peek().Type == TokenType.Function)
								RPNTokens.Add(operatorStack.Pop());  // If it was a function before that, add it to the output
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
				RPNTokens.Add(tok);
			}
		}

		public double Compute(Dictionary<string, double> variableContext)
        {


			double tempNumber;
			Stack<double> valueStack = new Stack<double>();

			foreach (Token tok in RPNTokens)
            {

				if (tok.Type == TokenType.Integer)
				{
					valueStack.Push(double.Parse(tok.Value));
				} else if (tok.Type == TokenType.Constant) 
				{
					try
                    {
						valueStack.Push((double)RPN.Constants[tok.Value]);
					} catch (KeyNotFoundException e)
                    {
						Console.WriteLine($"Uknown variable: {tok.Value}");
						break;
                    }


				} else if (tok.Type == TokenType.Variable)
                {
					try
					{
						valueStack.Push((double)variableContext[tok.Value]);
					}
					catch (KeyNotFoundException e)
					{
						Console.WriteLine($"Uknown variable: {tok.Value}");
						break;
					}
				}
				else if (tok.Type == TokenType.Function)
				{
					switch (tok.Value)
					{
						case "sin":
							{
								valueStack.Push((double)Math.Sin(valueStack.Pop()));
								break;
							}
						case "cos":
							{
								valueStack.Push((double)Math.Cos(valueStack.Pop()));
								break;
							}
						case "tan":
							{
								valueStack.Push((double)Math.Tan((double)valueStack.Pop()));
								break;
							}
						case "sqrt":
							{
								valueStack.Push((double)Math.Sqrt(valueStack.Pop()));
								break;
							}
						case "ln":
							{
								valueStack.Push((double)Math.Log(valueStack.Pop(), Math.E));
								break;
							}
						case "log":
							{
								valueStack.Push((double)Math.Log10(valueStack.Pop()));
								break;
							}

					}
				} else if (tok.Type == TokenType.Operator)
				{
					switch (tok.Value)
					{
						case "^":
							{
								tempNumber = valueStack.Pop();
								valueStack.Push(Math.Pow(valueStack.Pop(), tempNumber));
								break;
							}
						case "*":
							{
								valueStack.Push(valueStack.Pop() * valueStack.Pop());
								break;
							}
						case "/":
							{
								tempNumber = valueStack.Pop();
								valueStack.Push(valueStack.Pop() / tempNumber);
								break;
							}
						case "+":
							{
								valueStack.Push(valueStack.Pop() + valueStack.Pop());
								break;
							}
						case "-":
							{
								tempNumber = valueStack.Pop();
								valueStack.Push(valueStack.Pop() - tempNumber);
								break;
							}
					}
				}
			}

			try
            {
				return valueStack.Pop();

			} catch (InvalidOperationException e)
            {
				Console.WriteLine("Problem parsing");
				return -1;
            }
		}

	}

	public class Program
	{
		public static void Main(string[] args)
		{
			
			while (true)
            {
				var text = Console.ReadLine();

				RPN polish = new RPN(text);

				Console.WriteLine(string.Join(" ", polish.InfixTokens.Select(v => v.Value)));

				polish.ShuntingYard();

				Console.WriteLine(string.Join(" ", polish.RPNTokens.Select(v => v.Value)));

				Console.WriteLine(polish.Compute(new Dictionary<string, double>() { }));
			}



		}
	}
}