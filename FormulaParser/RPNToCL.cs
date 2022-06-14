using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaParser
{
    public class RPNToCL
    {
        public static Dictionary<string, string> ConstantTranslation;
        public static Dictionary<string, string> VariableTranslation;
        public static Dictionary<string, string> FunctionTranslation;
        public static Dictionary<string, string> OperatorTranslation;
        public RPN RPNObj;
        Stack<IHasCCode> ExpressionStack;
        public string CCode;

        internal interface IHasCCode
        {
            string CExpression { get; }
        }



        internal record OneArg(string Content) : ExpressionElement(Content), IHasCCode
        {
            public OneArg(string Content, IHasCCode arg) : this(Content)
            {
                Arg = arg;
                
            }

            public string CExpression
            {
                get
                {
                    return $"{FunctionTranslation[Content]}({Arg.CExpression})";
                }
            }

            IHasCCode Arg { get; set; }
        }

        // Operator
        internal record TwoArg(string Content) : ExpressionElement(Content), IHasCCode
        {
            public TwoArg(string Content, IHasCCode arg1, IHasCCode arg2) : this(Content)
            {
                Arg1 = arg1; 
                Arg2 = arg2;
            }

            public string CExpression
            {
                get
                {
                    return $"{OperatorTranslation[Content]}({Arg1.CExpression}, {Arg2.CExpression})";
                }
            }


            IHasCCode Arg1 { get; set; }
            IHasCCode Arg2 { get; set; }
        } 

        internal record Variable(string Content) : ExpressionElement(Content), IHasCCode
        {
            // The code is just the name of the variable
            public string CExpression
            {
                get
                {
                    return VariableTranslation[Content];
                }
            }
        }

        internal record Number(string Conent) : ExpressionElement(Conent), IHasCCode
        {
            public string CExpression
            {
                get
                {
                    return $"complex({Conent}, 0)";
                }
            }
        }

        internal record Constant(string Content) : ExpressionElement(Content), IHasCCode
        {
            public string CExpression
            {
                get
                {
                    return $"complex({ConstantTranslation[Content]}, 0)";
                }
            }
        }

        internal record ExpressionElement(string Content);


        public RPNToCL(RPN rpnObj, string subject, Dictionary<string, string> constantTranslation, Dictionary<string, string> variableTranslation, Dictionary<string, string> functionTranslation,
            Dictionary<string, string> operatorTranslation)
        {
            RPNObj = rpnObj;

            ConstantTranslation = constantTranslation;
            VariableTranslation = variableTranslation;
            FunctionTranslation = functionTranslation;
            OperatorTranslation = operatorTranslation;

            ExpressionStack = new Stack<IHasCCode>();

            Stack<Token> tokStack = new Stack<Token>();

            foreach (Token tok in rpnObj.RPNTokens)
            {
                switch (tok.Type)
                {
                    case TokenType.Number:
                        ExpressionStack.Push(new Number(tok.Value));
                        break;
                    case TokenType.Variable:
                        ExpressionStack.Push(new Variable(tok.Value));
                        break;
                    case TokenType.Constant:
                        ExpressionStack.Push(new Constant(tok.Value));
                        break;
                    case TokenType.Operator:
                        IHasCCode temp = ExpressionStack.Pop();
                        ExpressionStack.Push(new TwoArg(tok.Value, ExpressionStack.Pop(), temp));
                        break;
                    case TokenType.Function:
                        ExpressionStack.Push(new OneArg(tok.Value, ExpressionStack.Pop()));
                        break;

                }
            }



            

            CCode = $"{subject} = {ExpressionStack.Pop().CExpression};";

            Console.WriteLine(CCode);
        }

    }
}
