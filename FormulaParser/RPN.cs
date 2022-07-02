using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Numerics;

using Exversion.Analytics;

namespace FormulaParser
{
	public class RPN
	{
		#region Fields
		private string _infixString;
		private List<Token> _RPNTokens;
		private List<Token> _infixTokens;
		#endregion

		#region Important Data
		/// <summary>
		/// A dictionary containing the primary single-character operators, realating to an instance of the Operator struct 
		/// </summary>
		public static Dictionary<string, Operator> Operators = new Dictionary<string, Operator>()
		{
			["+"] = new Operator("+", 1),
			["-"] = new Operator("-", 1),
			["*"] = new Operator("*", 2),
			["/"] = new Operator("/", 2),
			["^"] = new Operator("^", 3, true),
		};

		/// <summary>
		/// A list of strings containing the names of functions that can be used in the infix string
		/// </summary>
		public static string[] Functions = new string[] { "sin", "cos", "tan", "sqrt", "log", "log10", "abs", "pos", "arg", "conj", "exp", "sinh", "cosh", "tanh", "asin", "acos", "atan",};

		/// <summary>
		/// A dictionary containing names of mathematical constants that can be used in 
		/// the infix string relating to the actual numerical value (double)
		/// </summary>
		public static readonly Dictionary<string, double> Constants = new Dictionary<string, double>()
		{
			["pi"] = Math.PI,
			["e"] = Math.E,
		};

		/// <summary>
		/// A grid of integers, where the Keys represent different types of characters in the infix string,
		/// and the integer representing the behaviour expected from those two characters paired one after the other
		/// </summary>
		/// <remarks>The first key is the first token, the second key is the second token in the token pair</remarks>
		public static readonly Dictionary<TokenType, Dictionary<TokenType, int>> OrderRules = new Dictionary<TokenType, Dictionary<TokenType, int>>()
		{
			[TokenType.CloseParenthesis] = new Dictionary<TokenType, int>
			{
				[TokenType.CloseParenthesis] = 0,
				[TokenType.Constant] = 1,
				[TokenType.Function] = 1,
				[TokenType.Number] = 1,
				[TokenType.OpenParenthesis] = 1,
				[TokenType.Operator] = 0,
				[TokenType.Variable] = 1,
			},
			[TokenType.Constant] = new Dictionary<TokenType, int>()
			{
				[TokenType.CloseParenthesis] = 0,
				[TokenType.Constant] = 2,
				[TokenType.Function] = 2,
				[TokenType.Number] = 1,
				[TokenType.OpenParenthesis] = 1,
				[TokenType.Operator] = 0,
				[TokenType.Variable] = 2
			},
			[TokenType.Function] = new Dictionary<TokenType, int>()
			{
				[TokenType.CloseParenthesis] = -1,
				[TokenType.Constant] = 2,
				[TokenType.Function] = 2,
				[TokenType.Number] = -1,
				[TokenType.OpenParenthesis] = 0,
				[TokenType.Operator] = -1,
				[TokenType.Variable] = 2
			},
			[TokenType.Number] = new Dictionary<TokenType, int>()
			{
				[TokenType.CloseParenthesis] = 0,
				[TokenType.Constant] = 1,
				[TokenType.Function] = 1,
				[TokenType.Number] = 2,
				[TokenType.OpenParenthesis] = 1,
				[TokenType.Operator] = 0,
				[TokenType.Variable] = 1
			},
			[TokenType.OpenParenthesis] = new Dictionary<TokenType, int>()
			{
				[TokenType.CloseParenthesis] = 0,
				[TokenType.Constant] = 0,
				[TokenType.Function] = 0,
				[TokenType.Number] = 0,
				[TokenType.OpenParenthesis] = 0,
				[TokenType.Operator] = -1,
				[TokenType.Variable] = 0
			},
			[TokenType.Operator] = new Dictionary<TokenType, int>()
			{
				[TokenType.CloseParenthesis] = -1,
				[TokenType.Constant] = 0,
				[TokenType.Function] = 0,
				[TokenType.Number] = 0,
				[TokenType.OpenParenthesis] = 0,
				[TokenType.Operator] = -1,
				[TokenType.Variable] = 0
			},
			[TokenType.Variable] = new Dictionary<TokenType, int>()
			{
				[TokenType.CloseParenthesis] = 0,
				[TokenType.Constant] = 2,
				[TokenType.Function] = 2,
				[TokenType.Number] = 1,
				[TokenType.OpenParenthesis] = 1,
				[TokenType.Operator] = 0,
				[TokenType.Variable] = 2
			},
		};
		#endregion

		#region Properties
		/// <summary>
		/// The list holding the tokens representing the non-shunted formula
		/// </summary>
		public List<Token> InfixTokens
		{
			get
			{
				return _infixTokens;
			}
			set
			{
				_infixTokens = value;
			}
		}

		/// <summary>
		/// The list holding the tokens representing the reverse polish notation formula
		/// </summary>
		public List<Token> RPNTokens
		{
			get
			{
				return _RPNTokens;
			}
			set
            {
				_RPNTokens = value;
            }
		}

		/// <summary>
		/// The infix formula in string form
		/// </summary>
        public string InfixString
		{
			get { return _infixString; }
			set { _infixString = value; }
		}

		/// <summary>
		/// A helpful Property for quickly printing out the Shunted formula
		/// </summary>
		public string RPNString
        {
			get
            {
				return string.Join(" ", RPNTokens.Select(t => t.Value));
            }
        }

		public List<string> VariableNames = new List<string>();

		public string LaTEX { get; set; }
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="infix">The input string.</param>
		/// <exception cref="Exception">Problem parsing string</exception>
		public RPN(string infix)
		{
			if (CheckBrackets(infix))
			{
				_infixString = infix;
			}
			else
			{

				throw new Exception("Mismatched brackets");
			}


			_infixTokens = Tokenize(infix);
			_RPNTokens = ShuntingYard();

			// Generate a list of the variable names
			foreach (Token token in _infixTokens)
            {
				if (token.Type == TokenType.Variable && !(VariableNames.Contains(token.Value)))
                {
					VariableNames.Add(token.Value);
                }
            }

			
			AnalyticsConverter translator = new AnalyticsTeXConverter();
			try
            {
				LaTEX = translator.Convert(InfixString);
            }
			catch
            {
				throw new Exception("Could not parse infix string to LaTEX");
            }

		}

		/// <summary>
		/// A quick check right at the start if the brackets match
		/// </summary>
		/// <param name="formula"></param>
		/// <returns>True if there is an equal number of open "(" and close ")" brackets, false if not</returns>
		private bool CheckBrackets(string formula)
		{
			int open = formula.Count(c => (c == '('));
			int close = formula.Count(c => (c == ')'));

			if (open == close)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Compare precedence of operators
		/// </summary>
		/// <param name="op1">The first operator</param>
		/// <param name="op2">The second operator</param>
		/// <returns>True or false</returns>
		private bool CompareOperators(Operator op1, Operator op2) => op1.RightAssociative ? op1.Precedence < op2.Precedence : op1.Precedence <= op2.Precedence;

		#region Type Determiners
		public static TokenType DetermineType(string str)
		{
			if (Regex.IsMatch(str, @"^[a-zA-Z]+$")) // All letters?
				if (RPN.Functions.Contains(str))
				{
					return TokenType.Function;
				}
				else if (RPN.Constants.ContainsKey(str))
				{
					Console.WriteLine($"{str} is a constant!");
					return TokenType.Constant;
				}
				else
				{
					return TokenType.Variable;
				}

			if (double.TryParse(str, out _))  // All numbers?
				return TokenType.Number;

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


			throw new Exception($"Unknown character {str}");
		}

		public static TokenType DetermineCharType(char ch)
		{
			if (Regex.IsMatch(ch.ToString(), @"^[a-zA-Z]+$"))
			{
				return TokenType.Letter;
			}
			if (double.TryParse(ch.ToString(), out _))
			{
				return TokenType.Number;
			}
			if (string.IsNullOrWhiteSpace(ch.ToString()))
				return TokenType.WhiteSpace;

			if (ch == '(')
				return TokenType.OpenParenthesis;

			if (ch == ')')
				return TokenType.CloseParenthesis;

			if (Operators.ContainsKey(ch.ToString()))
				return TokenType.Operator;

			if (ch == '.')
			{
				return TokenType.DecimalPoint;
			}

			throw new Exception($"Unknown character {ch}");
		}
		#endregion

		public List<Token> Tokenize(string infix)
		{
			#region Variable Setup
			List<Token> tokens = new List<Token>();

			TextReader textReader = new StringReader(infix);

			TokenType nextType;
			int curr;
			char ch;
			int next;
			char nextCh;
			bool parenthesis;
			int operation;

			string readCharacters = "";
			#endregion

			#region FIRST PASS
			// FIRST ITERATION: split the infix expression crudely up into it's seconctions
			while ((curr = textReader.Read()) != -1)
			{
				ch = (char)curr;  // Current Character
				next = textReader.Peek();  // Integer holding character value for next character
				nextCh = (char)next;  // Next character


				TokenType currentType = DetermineCharType(ch);


				parenthesis = (currentType == TokenType.CloseParenthesis || currentType == TokenType.OpenParenthesis) ? true : false;  // Is it a parenthesis?

				nextType = next != -1 ? DetermineCharType(nextCh) : TokenType.WhiteSpace;

				if (currentType == TokenType.WhiteSpace)
				{
					continue; // Remove the whitespace! This will mean the "currentType != nextType" will always fire on the last letter, so there are no errors
				}
				else
				{
					readCharacters += ch;  //
				}

				// Do not worry about the type changing if it is going to form a decimal number!
				if (currentType == TokenType.Number && nextType == TokenType.DecimalPoint || currentType == TokenType.DecimalPoint && nextType == TokenType.Number)
				{
					continue;
				}


				// Multiple brackets are the only situation where you want to add similar types in a row as seperate tokens
				if (currentType != nextType || parenthesis || currentType == TokenType.Operator)  // Operators are also immediately added
				{
					TokenType cumulativeType = DetermineType(readCharacters);

					tokens.Add(new Token(cumulativeType, readCharacters));

					readCharacters = "";
				}

			}
			#endregion

			#region SECOND PASS
			// SECOND ITERATION: Combine and add in operators where needed
			for (int i = 0; i < tokens.Count() - 1; i++)
			{
				Token currentToken = tokens[i];
				Token nextToken = tokens[i + 1];

				// DECIMAL POINT WAS NOT IN DICTIONARY ERROR
				operation = RPN.OrderRules[currentToken.Type][nextToken.Type];

				Console.WriteLine($"{currentToken.Value} to {nextToken.Value}: {operation}");


				// Get subtract's special stuff out the way
				if (currentToken.Value == "-")
				{
					// Check next value first for simplicity
					if (nextToken.Type == TokenType.Constant || nextToken.Type == TokenType.Number || nextToken.Type == TokenType.Variable)
					{

						// First value is Open Parenthesis, Operator, or it is the first value

						if (i == 0)
						{
							// ConSUME the next token, and add a "-"
							tokens.RemoveRange(i, 1);  // Remove the -
							tokens[i].AddNeg(); // Not i + 1 because we just removed it on the line above
						}
						else
						{  // Only try to do this if we defo know we are not on the first token!
							if (tokens[i - 1].Type == TokenType.OpenParenthesis || tokens[i - 1].Type == TokenType.Operator)
							{

								// ConSUME the next token, and add a "-"
								tokens.RemoveRange(i, 1);  // Remove the -
								tokens[i].AddNeg();  // Not i + 1 because we just removed it on the line above
							}

						}

					}
				}

				if ((currentToken.Type == TokenType.OpenParenthesis || currentToken.Type == TokenType.Operator) && nextToken.Value == "-") { continue; }  // To fix the "Syntax error unless minus"


				switch (operation)
				{
					case -1:
						{
							throw new Exception($"Illegal Syntax {currentToken.Type}:{nextToken.Type}");
						}
					case 0:
						break;
					case 1:
						{
							tokens.Insert(i + 1, new Token(TokenType.Operator, "*"));
							i += 2;  // Might as well skip over the * too
							break;
						}
					case 2:
						Console.WriteLine("Should not be possible");
						break;

						// NEED TO FIX: when making a variable: cannot use the phrases that referr to the constants, including the letter "e" ):
				}

				Console.WriteLine($"{currentToken.Value} to {nextToken.Value}: {operation}");
			}
			#endregion

			return tokens;
		}

		public List<Token> ShuntingYard()
		{
			List<Token> outputList = new List<Token>();
			Stack<Token> operatorStack = new Stack<Token>();
			foreach (var tok in InfixTokens)  // Iterate throught the tokens
			{
				switch (tok.Type)
				{
					case TokenType.Constant:
					case TokenType.Variable: // If number or varible put straight into the output
					case TokenType.Number:
						outputList.Add(tok);
						break;
					case TokenType.Function:  // If function, put on the op stack
						operatorStack.Push(tok);
						break;
					case TokenType.Operator:
						//   Any elements in opstack?    Top token is an operator?             Is the current token's precedeance less than the token on top?
						while (operatorStack.Any() && operatorStack.Peek().Type == TokenType.Operator && CompareOperators(Operators[tok.Value], Operators[operatorStack.Peek().Value]))
						{
							outputList.Add(operatorStack.Pop()); // Then put that on the output!
						}
						operatorStack.Push(tok);  // Then put the operator on top
						break;
					case TokenType.OpenParenthesis:
						operatorStack.Push(tok);
						break;
					case TokenType.CloseParenthesis:
						while (operatorStack.Peek().Type != TokenType.OpenParenthesis) { outputList.Add(operatorStack.Pop()); }   // Pop off the opstack until the open bracket is found

						operatorStack.Pop();  // Then pop off the open bracket

						if (operatorStack.Count() != 0) // If not empty
						{
							if (operatorStack.Peek().Type == TokenType.Function)
								outputList.Add(operatorStack.Pop());  // If it was a function before that, add it to the output
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
				outputList.Add(tok);
			}

			return outputList;
		}

		public Complex ComputeComplex(Dictionary<string, Complex> variableContext)
		{


			Complex tempNumber;  // Needed.
			Stack<Complex> valueStack = new Stack<Complex>();

			foreach (Token tok in RPNTokens)
			{

				if (tok.Type == TokenType.Number)
				{
					valueStack.Push(double.Parse(tok.Value));
				}
				else if (tok.Type == TokenType.Constant)
				{
					try
					{
						valueStack.Push((double)RPN.Constants[tok.Value]);
					} 
					catch (KeyNotFoundException)
					{
						Console.WriteLine($"Uknown variable: {tok.Value}");
						break;
					}


				}
				else if (tok.Type == TokenType.Variable)
				{
					if (variableContext.ContainsKey(tok.Value))
                    {
						valueStack.Push(variableContext[tok.Value]);
                    } 
					else
                    {
						throw new Exception($"No variable: {tok.Value} found!");
                    }


				}
				else if (tok.Type == TokenType.Function)
				{
					switch (tok.Value)
					{
						case "sin":
							{
								valueStack.Push(Complex.Sin(valueStack.Pop()));
								break;
							}
						case "cos":
							{
								valueStack.Push(Complex.Cos(valueStack.Pop()));
								break;
							}
						case "tan":
							{
								valueStack.Push(Complex.Tan(valueStack.Pop()));
								break;
							}
						case "sqrt":
							{
								valueStack.Push(Complex.Sqrt(valueStack.Pop()));
								break;
							}
						case "log":
							{
								valueStack.Push(Complex.Log(valueStack.Pop()));
								break;
							}
						case "log10":
							{
								valueStack.Push(Complex.Log10(valueStack.Pop()));
								break;
							}
						case "abs":
                            {
								valueStack.Push(Complex.Abs(valueStack.Pop()));
								break;
							}
						case "arg":
							valueStack.Push(valueStack.Pop().Phase);
							break;
						case "conj":
							valueStack.Push(Complex.Conjugate(valueStack.Pop()));
							break;
						case "exp":
							valueStack.Push(Complex.Exp(valueStack.Pop()));
                            break;
                        case "sinh":
							valueStack.Push(Complex.Sinh(valueStack.Pop()));
                            break;
                        case "cosh":
							valueStack.Push(Complex.Cosh(valueStack.Pop()));
							break;
						case "tanh":
							valueStack.Push(Complex.Tanh(valueStack.Pop()));
                            break;
                        case "asin":
							valueStack.Push(Complex.Asin(valueStack.Pop()));
							break;
						case "acos":
							valueStack.Push(Complex.Acos(valueStack.Pop()));
							break;
						case "atan":
							valueStack.Push(Complex.Atan(valueStack.Pop()));
                            break;
                       


                    }
                }
				else if (tok.Type == TokenType.Operator)
				{
					switch (tok.Value)
					{
						case "^":
							{
								tempNumber = valueStack.Pop();
								valueStack.Push(Complex.Pow(valueStack.Pop(), tempNumber));
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

			} catch
            {
				throw new Exception("Problem parsing");

			}


		}

		/// <summary>
		/// Huge amount of repeated code in this method, fix please.
		/// Can be optimised as new variables do not need to be created every time there is an operation on a non-subject variable
		/// </summary>
		/// <param name="formulaSubject">The subject variable name in the formula string</param>
		/// <param name="codeSubject">The name of the subject variable expected in the C code</param>
		/// <param name="cConstants">The names in the C code of the constants, eg "pi" in the formula is called M_PI in CL</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
        public List<string> GenerateOpenCLC(string formulaSubject, string codeSubject, Dictionary<string, string> cConstants)
        {
			List<string> otherVariables = VariableNames;
			otherVariables.Remove(formulaSubject);  // Remove the formula subject to make this work

			Token nextToken;  // Needed.
            

            List<string> lines = new List<string>();
            Stack<Token> tokenStack = new Stack<Token>();

			Dictionary<string, Tuple<string, int>> tempVariableCount = new Dictionary<string, Tuple<string, int>>();

			foreach (string s in otherVariables)
            {
				tempVariableCount[s] = new Tuple<string, int>(s, 0);
            }

			// Shit, fix it immediately 
			foreach (Token tok in RPNTokens)
            {

                if (tok.Type == TokenType.Number)
                {
                    tokenStack.Push(tok);
                }
                else if (tok.Type == TokenType.Constant)
                {
                    tokenStack.Push(tok);
                }
                else if (tok.Type == TokenType.Variable)
                {
                    if (otherVariables.Contains(tok.Value))
                    {
                        tokenStack.Push(tok);
                    } else if (tok.Value == formulaSubject)
                    {
						tokenStack.Push(tok);
                    }
                    else
                    {
                        throw new Exception($"No variable {tok.Value} expected in CL code");
                    }
                }
                else if (tok.Type == TokenType.Function)
                {
                    nextToken = tokenStack.Pop();

                    

                    switch (tok.Value)
                    {
                        case "sin":
                            {
                                if (nextToken.Value == formulaSubject)  // Subject operation
                                {
                                    // Return line of C code
                                    lines.Add($"{codeSubject} = csin({codeSubject});");
                                    tokenStack.Push(nextToken);
                                    break;
                                } else if (otherVariables.Contains(nextToken.Value)) // Variable operation without the subject
                                {
									// Increase this temp variable by one

									string currentName = tempVariableCount[nextToken.Value].Item1;
									int currentNumber = tempVariableCount[nextToken.Value].Item2;

									// Set this new c as the working one 
									tempVariableCount[nextToken.Value] = new Tuple<string, int>(currentName+currentNumber.ToString(), currentNumber++);

									lines.Add($"cdouble {tempVariableCount[nextToken.Value].Item1} = csin({currentName});");
									tokenStack.Push(nextToken);  // Put c back on the stack
									break;
                                }

								// Else it is a statement that can be simlified here
                                Token simplifiedTok = new Token(TokenType.Number, Math.Sin(double.Parse(nextToken.Value)).ToString());

                                tokenStack.Push(simplifiedTok);
                                break;
                            }
                        case "cos":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation
								{
									// Return line of C code
									lines.Add($"{codeSubject} = ccos({codeSubject});");
									tokenStack.Push(nextToken);
									break;
								}
								else if (otherVariables.Contains(nextToken.Value)) // Variable operation without the subject
								{
									// Increase this temp variable by one

									string currentName = tempVariableCount[nextToken.Value].Item1;
									int currentNumber = tempVariableCount[nextToken.Value].Item2;

									// Set this new c as the working one 
									tempVariableCount[nextToken.Value] = new Tuple<string, int>(currentName + currentNumber.ToString(), currentNumber++);

									lines.Add($"cdouble {tempVariableCount[nextToken.Value].Item1} = ccos({currentName});");
									tokenStack.Push(nextToken);  // Put c back on the stack
									break;
								}

								// Else it is a statement that can be simlified here
								Token simplifiedTok = new Token(TokenType.Number, Math.Cos(double.Parse(nextToken.Value)).ToString());

								tokenStack.Push(simplifiedTok);
								break;
							}
                        case "tan":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation
								{
									// Return line of C code
									lines.Add($"{codeSubject} = ctan({codeSubject});");
									tokenStack.Push(nextToken);
									break;
								}
								else if (otherVariables.Contains(nextToken.Value)) // Variable operation without the subject
								{
									// Increase this temp variable by one

									string currentName = tempVariableCount[nextToken.Value].Item1;
									int currentNumber = tempVariableCount[nextToken.Value].Item2;

									// Set this new c as the working one 
									tempVariableCount[nextToken.Value] = new Tuple<string, int>(currentName + currentNumber.ToString(), currentNumber++);

									lines.Add($"cdouble {tempVariableCount[nextToken.Value].Item1} = ctan({currentName});");
									tokenStack.Push(nextToken);  // Put c back on the stack
									break;
								}

								// Else it is a statement that can be simlified here
								Token simplifiedTok = new Token(TokenType.Number, Math.Tan(double.Parse(nextToken.Value)).ToString());

								tokenStack.Push(simplifiedTok);
								break;
							}
                        case "sqrt":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation
								{
									// Return line of C code
									lines.Add($"{codeSubject} = csqrt({codeSubject});");
									tokenStack.Push(nextToken);
									break;
								}
								else if (otherVariables.Contains(nextToken.Value)) // Variable operation without the subject
								{
									// Increase this temp variable by one

									string currentName = tempVariableCount[nextToken.Value].Item1;
									int currentNumber = tempVariableCount[nextToken.Value].Item2;

									// Set this new c as the working one 
									tempVariableCount[nextToken.Value] = new Tuple<string, int>(currentName + currentNumber.ToString(), currentNumber++);

									lines.Add($"cdouble {tempVariableCount[nextToken.Value].Item1} = csqrt({currentName});");
									tokenStack.Push(nextToken);  // Put c back on the stack
									break;
								}

								// Else it is a statement that can be simlified here
								Token simplifiedTok = new Token(TokenType.Number, Math.Sqrt(double.Parse(nextToken.Value)).ToString());

								tokenStack.Push(simplifiedTok);
								break;
							}
                        case "log":  //ln
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation
								{
									// Return line of C code
									lines.Add($"{codeSubject} = clog({codeSubject});");
									tokenStack.Push(nextToken);
									break;
								}
								else if (otherVariables.Contains(nextToken.Value)) // Variable operation without the subject
								{
									// Increase this temp variable by one

									string currentName = tempVariableCount[nextToken.Value].Item1;
									int currentNumber = tempVariableCount[nextToken.Value].Item2;

									// Set this new c as the working one 
									tempVariableCount[nextToken.Value] = new Tuple<string, int>(currentName + currentNumber.ToString(), currentNumber++);

									lines.Add($"cdouble {tempVariableCount[nextToken.Value].Item1} = clog({currentName});");
									tokenStack.Push(nextToken);  // Put c back on the stack
									break;
								}

								// Else it is a statement that can be simlified here
								Token simplifiedTok = new Token(TokenType.Number, Math.Log(double.Parse(nextToken.Value)).ToString());

								tokenStack.Push(simplifiedTok);
								break;
							}
                        case "log10":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation
								{
									// Return line of C code
									lines.Add($"{codeSubject} = clog10({codeSubject});");
									tokenStack.Push(nextToken);
									break;
								}
								else if (otherVariables.Contains(nextToken.Value)) // Variable operation without the subject
								{
									// Increase this temp variable by one

									string currentName = tempVariableCount[nextToken.Value].Item1;
									int currentNumber = tempVariableCount[nextToken.Value].Item2;

									// Set this new c as the working one 
									tempVariableCount[nextToken.Value] = new Tuple<string, int>(currentName + currentNumber.ToString(), currentNumber++);

									lines.Add($"cdouble {tempVariableCount[nextToken.Value].Item1} = clog10({currentName});");
									tokenStack.Push(nextToken);  // Put c back on the stack
									break;
								}

								// Else it is a statement that can be simlified here
								Token simplifiedTok = new Token(TokenType.Number, Math.Log10(double.Parse(nextToken.Value)).ToString());

								tokenStack.Push(simplifiedTok);
								break;
							}

                    }


                }
                else if (tok.Type == TokenType.Operator)
                {
                    nextToken = tokenStack.Pop();  // Exponent
                    Token nextNextToken = tokenStack.Pop();  // Base

                    

                    switch (tok.Value)
                    {
                        case "^":
                            {
                                if (nextToken.Value == formulaSubject)  // Subject operation, stuff to power of z
                                {
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of z
                                    {
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;  // Do not need to get new c name
										lines.Add($"{codeSubject} = cpow({currentVarName}, {codeSubject});");
                                    } 
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of z
                                    {
										lines.Add($"{codeSubject} = cpow(complex({nextNextToken.Value}, 0), {codeSubject});");
										
                                    } 
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to the power of z
                                    {
										lines.Add($"{codeSubject} = cpow(complex({cConstants[nextNextToken.Value]}, 0), {codeSubject});");
										
                                    }
									else if (nextNextToken.Value == formulaSubject)  // z to power of z
                                    {
										lines.Add($"{codeSubject} = cpow({codeSubject}, {codeSubject});");
                                    }

									tokenStack.Push(nextToken);  // z gets put back onto the stack
								}
								else if (otherVariables.Contains(nextToken.Value))  // stuff to power of variable 
                                {
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of variable
                                    {
										// Choose the base to be added back onto the stack

										string baseVarName = tempVariableCount[nextNextToken.Value].Item1;
										int baseVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										string exponentVarName = tempVariableCount[nextToken.Value].Item1;

										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + baseVarNumber++.ToString(), baseVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;
										lines.Add($"cdoulbe {newVariableName} = cpow({baseVarName}, {exponentVarName});");
										// Here, the variable which is the exponent gets consumed into the next rendition of the base variable

										tokenStack.Push(nextNextToken);
                                    } 
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of variable 
                                    {
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Update the temp variable dictionary
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cpow(complex({nextNextToken.Value}, 0), {currentVarName});");
										tokenStack.Push(nextToken);
                                    }
									else if (nextNextToken.Value == formulaSubject)  // z to power of variable
                                    {
										lines.Add($"{codeSubject} = cpow({codeSubject}, {tempVariableCount[nextToken.Value].Item1});");
										tokenStack.Push(nextNextToken);
                                    }
									else if (nextNextToken.Type == TokenType.Constant) // Constant to power of variable
                                    {
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

									    // Increment the name of the variable
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cpow(complex({cConstants[nextNextToken.Value]}, 0), {currentVarName});");
										tokenStack.Push(nextToken);

                                    }
                                }
								else if (nextToken.Type == TokenType.Constant)  // Stuff to power of constant
                                {
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of constant
                                    {
										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"{newVariableName} = cpow({currentVarName}, complex({cConstants[nextToken.Value]}, 0));");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of constant
                                    {
										lines.Add($"{codeSubject} = cpow({codeSubject}, complex({cConstants[nextToken.Value]}, 0));");
										tokenStack.Push(nextNextToken);
                                    }
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of constant
                                    {
										// Simplify 

										double result = (double)Math.Pow(Constants[nextNextToken.Value], Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
                                    } 
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of constant
                                    {
										// Simplify
										double result = (double)Math.Pow(double.Parse(nextNextToken.Value), Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
                                    }
                                }
								else if (nextToken.Type == TokenType.Number)  // Stuff to power of number
                                {
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of number
                                    {
										// Get a new temp variable

										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cpow({currentVarName}, complex({nextToken.Value}, 0));");
										tokenStack.Push(nextNextToken);

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of number
                                    {
										lines.Add($"{codeSubject} = cpow({codeSubject}, complex({nextToken.Value}, 0));");
										tokenStack.Push(nextNextToken);
                                    }
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of number
                                    {
										// Simplify

										double result = (double)Math.Pow(Constants[nextNextToken.Value], double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));

                                    }
									else if (nextNextToken.Type == TokenType.Number)  // number to power of number
                                    {
										// Simplify

										double result = (double)Math.Pow(double.Parse(nextNextToken.Value), double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
                                    }
                                }

                                
                                break;
                            }
                        case "*":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation, stuff to power of z
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of z
									{
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;  // Do not need to get new c name
										lines.Add($"{codeSubject} = cmul({currentVarName}, {codeSubject});");
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of z
									{
										lines.Add($"{codeSubject} = cmul(complex({nextNextToken.Value}, 0), {codeSubject});");

									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to the power of z
									{
										lines.Add($"{codeSubject} = cmul(complex({cConstants[nextNextToken.Value]}, 0), {codeSubject});");

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of z
									{
										lines.Add($"{codeSubject} = cmul({codeSubject}, {codeSubject});");
									}

									tokenStack.Push(nextToken);  // z gets put back onto the stack
								}
								else if (otherVariables.Contains(nextToken.Value))  // stuff to power of variable 
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of variable
									{
										// Choose the base to be added back onto the stack

										string baseVarName = tempVariableCount[nextNextToken.Value].Item1;
										int baseVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										string exponentVarName = tempVariableCount[nextToken.Value].Item1;

										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + baseVarNumber++.ToString(), baseVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;
										lines.Add($"cdoulbe {newVariableName} = cmul({baseVarName}, {exponentVarName});");
										// Here, the variable which is the exponent gets consumed into the next rendition of the base variable

										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of variable 
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Update the temp variable dictionary
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cmul(complex({nextNextToken.Value}, 0), {currentVarName});");
										tokenStack.Push(nextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of variable
									{
										lines.Add($"{codeSubject} = cmul({codeSubject}, {tempVariableCount[nextToken.Value].Item1});");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant) // Constant to power of variable
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cmul(complex({cConstants[nextNextToken.Value]}, 0), {currentVarName});");
										tokenStack.Push(nextToken);

									}
								}
								else if (nextToken.Type == TokenType.Constant)  // Stuff to power of constant
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of constant
									{
										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"{newVariableName} = cmul({currentVarName}, complex({cConstants[nextToken.Value]}, 0));");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of constant
									{
										lines.Add($"{codeSubject} = cmul({codeSubject}, complex({cConstants[nextToken.Value]}, 0));");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of constant
									{
										// Simplify 

										double result = (double)(Constants[nextNextToken.Value] * Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of constant
									{
										// Simplify
										double result = (double)(double.Parse(nextNextToken.Value) * Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}
								else if (nextToken.Type == TokenType.Number)  // Stuff to power of number
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of number
									{
										// Get a new temp variable

										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cmul({currentVarName}, complex({nextToken.Value}, 0));");
										tokenStack.Push(nextNextToken);

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of number
									{
										lines.Add($"{codeSubject} = cmul({codeSubject}, complex({nextToken.Value}, 0));");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of number
									{
										// Simplify

										double result = (double)(Constants[nextNextToken.Value] * double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));

									}
									else if (nextNextToken.Type == TokenType.Number)  // number to power of number
									{
										// Simplify

										double result = (double)(double.Parse(nextNextToken.Value) + double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}


								break;
							}
                        case "/":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation, stuff to power of z
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of z
									{
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;  // Do not need to get new c name
										lines.Add($"{codeSubject} = cdiv({currentVarName}, {codeSubject});");
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of z
									{
										lines.Add($"{codeSubject} = cdiv(complex({nextNextToken.Value}, 0), {codeSubject});");

									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to the power of z
									{
										lines.Add($"{codeSubject} = cdiv(complex({cConstants[nextNextToken.Value]}, 0), {codeSubject});");

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of z
									{
										lines.Add($"{codeSubject} = cdiv({codeSubject}, {codeSubject});");
									}

									tokenStack.Push(nextToken);  // z gets put back onto the stack
								}
								else if (otherVariables.Contains(nextToken.Value))  // stuff to power of variable 
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of variable
									{
										// Choose the base to be added back onto the stack

										string baseVarName = tempVariableCount[nextNextToken.Value].Item1;
										int baseVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										string exponentVarName = tempVariableCount[nextToken.Value].Item1;

										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + baseVarNumber++.ToString(), baseVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;
										lines.Add($"cdoulbe {newVariableName} = cdiv({baseVarName}, {exponentVarName});");
										// Here, the variable which is the exponent gets consumed into the next rendition of the base variable

										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of variable 
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Update the temp variable dictionary
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cdiv(complex({nextNextToken.Value}, 0), {currentVarName});");
										tokenStack.Push(nextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of variable
									{
										lines.Add($"{codeSubject} = cdiv({codeSubject}, {tempVariableCount[nextToken.Value].Item1});");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant) // Constant to power of variable
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cdiv(complex({cConstants[nextNextToken.Value]}, 0), {currentVarName});");
										tokenStack.Push(nextToken);

									}
								}
								else if (nextToken.Type == TokenType.Constant)  // Stuff to power of constant
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of constant
									{
										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"{newVariableName} = cdiv({currentVarName}, complex({cConstants[nextToken.Value]}, 0));");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of constant
									{
										lines.Add($"{codeSubject} = cdiv({codeSubject}, complex({cConstants[nextToken.Value]}, 0));");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of constant
									{
										// Simplify 

										double result = (double)Decimal.Divide((decimal)Constants[nextNextToken.Value], (decimal)Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of constant
									{
										// Simplify
										double result = (double)Decimal.Divide((decimal)double.Parse(nextNextToken.Value), (decimal)Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}
								else if (nextToken.Type == TokenType.Number)  // Stuff to power of number
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of number
									{
										// Get a new temp variable

										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = cdiv({currentVarName}, complex({nextToken.Value}, 0));");
										tokenStack.Push(nextNextToken);

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of number
									{
										lines.Add($"{codeSubject} = cdiv({codeSubject}, complex({nextToken.Value}, 0));");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of number
									{
										// Simplify

										double result = (double)Decimal.Divide((decimal)Constants[nextNextToken.Value], (decimal)double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));

									}
									else if (nextNextToken.Type == TokenType.Number)  // number to power of number
									{
										// Simplify

										double result = (double)Decimal.Divide((decimal)double.Parse(nextNextToken.Value), (decimal)double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}

								break;
							}
                        case "+":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation, stuff to power of z
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of z
									{
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;  // Do not need to get new c name
										lines.Add($"{codeSubject} = {currentVarName} + {codeSubject};");
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of z
									{
										lines.Add($"{codeSubject} = complex({nextNextToken.Value}, 0) + {codeSubject};");

									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to the power of z
									{
										lines.Add($"{codeSubject} = complex({cConstants[nextNextToken.Value]}, 0) + {codeSubject};");

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of z
									{
										lines.Add($"{codeSubject} = {codeSubject} + {codeSubject};");
									}

									tokenStack.Push(nextToken);  // z gets put back onto the stack
								}
								else if (otherVariables.Contains(nextToken.Value))  // stuff to power of variable 
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of variable
									{
										// Choose the base to be added back onto the stack

										string baseVarName = tempVariableCount[nextNextToken.Value].Item1;
										int baseVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										string exponentVarName = tempVariableCount[nextToken.Value].Item1;

										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + baseVarNumber++.ToString(), baseVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;
										lines.Add($"cdoulbe {newVariableName} = {baseVarName} + {exponentVarName};");
										// Here, the variable which is the exponent gets consumed into the next rendition of the base variable

										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of variable 
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Update the temp variable dictionary
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = complex({nextNextToken.Value}, 0) + {currentVarName};");
										tokenStack.Push(nextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of variable
									{
										lines.Add($"{codeSubject} = {codeSubject} + {tempVariableCount[nextToken.Value].Item1};");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant) // Constant to power of variable
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = complex({cConstants[nextNextToken.Value]}, 0) + {currentVarName};");
										tokenStack.Push(nextToken);

									}
								}
								else if (nextToken.Type == TokenType.Constant)  // Stuff to power of constant
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of constant
									{
										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"{newVariableName} = {currentVarName} + complex({cConstants[nextToken.Value]}, 0);");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of constant
									{
										lines.Add($"{codeSubject} = {codeSubject} + complex({cConstants[nextToken.Value]}, 0);");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of constant
									{
										// Simplify 

										double result = (double)(Constants[nextNextToken.Value] + Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of constant
									{
										// Simplify
										double result = (double)(double.Parse(nextNextToken.Value) + Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}
								else if (nextToken.Type == TokenType.Number)  // Stuff to power of number
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of number
									{
										// Get a new temp variable

										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = {currentVarName} + complex({nextToken.Value}, 0);");
										tokenStack.Push(nextNextToken);

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of number
									{
										lines.Add($"{codeSubject} = {codeSubject} + complex({nextToken.Value}, 0);");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of number
									{
										// Simplify

										double result = (double)(Constants[nextNextToken.Value] + double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));

									}
									else if (nextNextToken.Type == TokenType.Number)  // number to power of number
									{
										// Simplify

										double result = (double)(double.Parse(nextNextToken.Value) + double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}

								break;
							}
                        case "-":
                            {
								if (nextToken.Value == formulaSubject)  // Subject operation, stuff to power of z
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of z
									{
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;  // Do not need to get new c name
										lines.Add($"{codeSubject} = {currentVarName} - {codeSubject};");
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of z
									{
										lines.Add($"{codeSubject} = complex({nextNextToken.Value}, 0) - {codeSubject};");

									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to the power of z
									{
										lines.Add($"{codeSubject} = complex({cConstants[nextNextToken.Value]}, 0) - {codeSubject};");

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of z
									{
										lines.Add($"{codeSubject} = {codeSubject} - {codeSubject};");
									}

									tokenStack.Push(nextToken);  // z gets put back onto the stack
								}
								else if (otherVariables.Contains(nextToken.Value))  // stuff to power of variable 
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of variable
									{
										// Choose the base to be added back onto the stack

										string baseVarName = tempVariableCount[nextNextToken.Value].Item1;
										int baseVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										string exponentVarName = tempVariableCount[nextToken.Value].Item1;

										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + baseVarNumber++.ToString(), baseVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;
										lines.Add($"cdoulbe {newVariableName} = {baseVarName} - {exponentVarName};");
										// Here, the variable which is the exponent gets consumed into the next rendition of the base variable

										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of variable 
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Update the temp variable dictionary
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = complex({nextNextToken.Value}, 0) - {currentVarName};");
										tokenStack.Push(nextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of variable
									{
										lines.Add($"{codeSubject} = {codeSubject} - {tempVariableCount[nextToken.Value].Item1};");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant) // Constant to power of variable
									{
										// Get a new temp variable
										string currentVarName = tempVariableCount[nextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextToken.Value] = new Tuple<string, int>(nextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = complex({cConstants[nextNextToken.Value]}, 0) - {currentVarName};");
										tokenStack.Push(nextToken);

									}
								}
								else if (nextToken.Type == TokenType.Constant)  // Stuff to power of constant
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of constant
									{
										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"{newVariableName} = {currentVarName} - complex({cConstants[nextToken.Value]}, 0);");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of constant
									{
										lines.Add($"{codeSubject} = {codeSubject} - complex({cConstants[nextToken.Value]}, 0);");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of constant
									{
										// Simplify 

										double result = (double)(Constants[nextNextToken.Value] - Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
									else if (nextNextToken.Type == TokenType.Number)  // Number to power of constant
									{
										// Simplify
										double result = (double)(double.Parse(nextNextToken.Value) - Constants[nextToken.Value]);
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}
								else if (nextToken.Type == TokenType.Number)  // Stuff to power of number
								{
									if (otherVariables.Contains(nextNextToken.Value))  // Variable to power of number
									{
										// Get a new temp variable

										// Make a new temporary variable
										string currentVarName = tempVariableCount[nextNextToken.Value].Item1;
										int currentVarNumber = tempVariableCount[nextNextToken.Value].Item2;

										// Increment the name of the variable
										tempVariableCount[nextNextToken.Value] = new Tuple<string, int>(nextNextToken.Value + currentVarNumber++.ToString(), currentVarNumber++);
										string newVariableName = tempVariableCount[nextNextToken.Value].Item1;

										lines.Add($"cdouble {newVariableName} = {currentVarName} - complex({nextToken.Value}, 0);");
										tokenStack.Push(nextNextToken);

									}
									else if (nextNextToken.Value == formulaSubject)  // z to power of number
									{
										lines.Add($"{codeSubject} = {codeSubject} - complex({nextToken.Value}, 0);");
										tokenStack.Push(nextNextToken);
									}
									else if (nextNextToken.Type == TokenType.Constant)  // Constant to power of number
									{
										// Simplify

										double result = (double)(Constants[nextNextToken.Value] - double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));

									}
									else if (nextNextToken.Type == TokenType.Number)  // number to power of number
									{
										// Simplify

										double result = (double)(double.Parse(nextNextToken.Value) - double.Parse(nextToken.Value));
										tokenStack.Push(new Token(TokenType.Number, result.ToString()));
									}
								}

								break;
							}
                    }
                }

				
            }

			return lines;

		}

    }
}