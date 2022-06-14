using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaParser
{
	public enum TokenType { Constant, Variable, Number, Function, OpenParenthesis, CloseParenthesis, Operator, WhiteSpace, DecimalPoint, Letter }


	public class Token
	{
		public TokenType Type { get; }
		public string Value { get; set; }

		public void AddNeg()
		{
			Value = "-" + Value;
		}

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



}
