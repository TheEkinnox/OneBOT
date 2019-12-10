using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathParserTK
{
    #region License agreement

    // This is light, fast and simple to understand mathematical parser 
    // designed in one class, which receives as input a mathematical 
    // expression (System.String) and returns the output value (System.Double). 
    // For example, if you input string is "√(625)+25*(3/3)" then parser return double value — 50. 
    // Copyright (C) 2012-2013 Yerzhan Kalzhani, kirnbas@gmail.com

    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.

    // This program is distributed in the hope that it will be useful,
    // but WITHOUT ANY WARRANTY; without even the implied warranty of
    // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    // GNU General Public License for more details.

    // You should have received a copy of the GNU General Public License
    // along with this program.  If not, see <http://www.gnu.org/licenses/> 

    #endregion

    #region Example usage

    // public static void Main()
    // {     
    //     MathParser parser = new MathParser();
    //     string s1 = "pi+5*5+5*3-5*5-5*3+1E1";
    //     string s2 = "sin(cos(tg(sh(ch(th(100))))))";
    //     bool isRadians = false;
    //     double d1 = parser.Parse(s1, isRadians);
    //     double d2 = parser.Parse(s2, isRadians);
    //
    //     Console.WriteLine(d1); // 13.141592...
    //     Console.WriteLine(d2); // 0.0174524023974442
    //     Console.ReadKey(true); 
    // }   

    #endregion

    #region Common tips to extend this math parser

    // If you want to add new operator, then add new item to supportedOperators
    // dictionary and check next methods:
    // (1) IsRightAssociated, (2) GetPriority, (3) SyntaxAnalysisRPN, (4) NumberOfArguments
    //
    // If you want to add new function, then add new item to supportedFunctions
    // dictionary and check next above methods: (2), (3), (4).
    // 
    // if you want to add new constant, then add new item to supportedConstants

    #endregion

    public class MathParser
    {
        #region Fields

        #region Markers (each marker should have length equals to 1)

        private const string numberMaker = "#";
        private const string operatorMarker = "$";
        private const string functionMarker = "@";

        #endregion

        #region Internal tokens

        private const string plus = MathParser.operatorMarker + "+";
        private const string unPlus = MathParser.operatorMarker + "un+";
        private const string minus = MathParser.operatorMarker + "-";
        private const string unMinus = MathParser.operatorMarker + "un-";
        private const string multiply = MathParser.operatorMarker + "*";
        private const string divide = MathParser.operatorMarker + "/";
        private const string degree = MathParser.operatorMarker + "^";
        private const string leftParent = MathParser.operatorMarker + "(";
        private const string rightParent = MathParser.operatorMarker + ")";
        private const string sqrt = MathParser.functionMarker + "sqrt";
        private const string sin = MathParser.functionMarker + "sin";
        private const string cos = MathParser.functionMarker + "cos";
        private const string tg = MathParser.functionMarker + "tg";
        private const string ctg = MathParser.functionMarker + "ctg";
        private const string sh = MathParser.functionMarker + "sh";
        private const string ch = MathParser.functionMarker + "ch";
        private const string th = MathParser.functionMarker + "th";
        private const string log = MathParser.functionMarker + "log";
        private const string ln = MathParser.functionMarker + "ln";
        private const string exp = MathParser.functionMarker + "exp";
        private const string abs = MathParser.functionMarker + "abs";

        #endregion

        #region Dictionaries (containts supported input tokens, exclude number)

        // Key -> supported input token, Value -> internal token or number

        /// <summary>
        /// Contains supported operators
        /// </summary>
        private readonly Dictionary<string, string> _supportedOperators =
            new Dictionary<string, string>
            {
                { "+", MathParser.plus },                
                { "-", MathParser.minus },
                { "*", MathParser.multiply },
                { "/", MathParser.divide },
                { "^", MathParser.degree },
                { "(", MathParser.leftParent },
                { ")", MathParser.rightParent }
            };

        /// <summary>
        /// Contains supported functions
        /// </summary>
        private readonly Dictionary<string, string> _supportedFunctions =
            new Dictionary<string, string>
            {
                { "sqrt", MathParser.sqrt },
                { "√", MathParser.sqrt },
                { "sin", MathParser.sin },
                { "cos", MathParser.cos },
                { "tg", MathParser.tg },
                { "ctg", MathParser.ctg },
                { "sh", MathParser.sh },
                { "ch", MathParser.ch },
                { "th", MathParser.th },
                { "log", MathParser.log },
                { "exp", MathParser.exp },
                { "abs", MathParser.abs }
            };

        private readonly Dictionary<string, string> _supportedConstants =
            new Dictionary<string, string>
            {
                {"pi", MathParser.numberMaker +  Math.PI.ToString() },
                {"e", MathParser.numberMaker + Math.E.ToString() }
            };

        #endregion

        #endregion

        private readonly char _decimalSeparator;
        private bool _isRadians;

        #region Constructors

        /// <summary>
        /// Initialize new instance of MathParser
        /// (symbol of decimal separator is read
        /// from regional settings in system)
        /// </summary>
        public MathParser()
        {
            try
            {
                this._decimalSeparator = Char.Parse(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Error: can't read char decimal separator from system, check your regional settings.", ex);
            }
        }

        /// <summary>
        /// Initialize new instance of MathParser
        /// </summary>
        /// <param name="decimalSeparator">Set decimal separator</param>
        public MathParser(char decimalSeparator)
        {
            this._decimalSeparator = decimalSeparator;
        }

        #endregion

        /// <summary>
        /// Produce result of the given math expression
        /// </summary>
        /// <param name="expression">Math expression (infix/standard notation)</param>
        /// <returns>Result</returns>
        public double Parse(string expression, bool isRadians = true)
        {
            this._isRadians = isRadians;

            try
            {
                return Calculate(ConvertToRpn(FormatString(expression)));
            }
            catch (DivideByZeroException e)
            {
                throw e;
            }
            catch (FormatException e)
            {
                throw e;
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Produce formatted string by the given string
        /// </summary>
        /// <param name="expression">Unformatted math expression</param>
        /// <returns>Formatted math expression</returns>
        private string FormatString(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Expression is null or empty");
            }

            StringBuilder formattedString = new StringBuilder();
            int balanceOfParenth = 0; // Check number of parenthesis

            // Format string in one iteration and check number of parenthesis
            // (this function do 2 tasks because performance priority)
            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];

                if (ch == '(')
                {
                    balanceOfParenth++;
                }
                else if (ch == ')')
                {
                    balanceOfParenth--;
                }

                if (Char.IsWhiteSpace(ch))
                {
                    continue;
                }
                else if (Char.IsUpper(ch))
                {
                    formattedString.Append(Char.ToLower(ch));
                }
                else
                {
                    formattedString.Append(ch);
                }
            }

            if (balanceOfParenth != 0)
            {
                throw new FormatException("Number of left and right parenthesis is not equal");
            }

            return formattedString.ToString();
        }

        #region Convert to Reverse-Polish Notation

        /// <summary>
        /// Produce math expression in reverse polish notation
        /// by the given string
        /// </summary>
        /// <param name="expression">Math expression in infix notation</param>
        /// <returns>Math expression in postfix notation (RPN)</returns>
        private string ConvertToRpn(string expression)
        {
            int pos = 0; // Current position of lexical analysis
            StringBuilder outputString = new StringBuilder();
            Stack<string> stack = new Stack<string>();

            // While there is unhandled char in expression
            while (pos < expression.Length)
            {
                string token = LexicalAnalysisInfixNotation(expression, ref pos);

                outputString = SyntaxAnalysisInfixNotation(token, outputString, stack);
            }

            // Pop all elements from stack to output string            
            while (stack.Count > 0)
            {
                // There should be only operators
                if (stack.Peek()[0] == MathParser.operatorMarker[0])
                {
                    outputString.Append(stack.Pop());
                }
                else
                {
                    throw new FormatException("Format exception,"
                    + " there is function without parenthesis");
                }
            }

            return outputString.ToString();
        }

        /// <summary>
        /// Produce token by the given math expression
        /// </summary>
        /// <param name="expression">Math expression in infix notation</param>
        /// <param name="pos">Current position in string for lexical analysis</param>
        /// <returns>Token</returns>
        private string LexicalAnalysisInfixNotation(string expression, ref int pos)
        {
            // Receive first char
            StringBuilder token = new StringBuilder();
            token.Append(expression[pos]);

            // If it is a operator
            if (this._supportedOperators.ContainsKey(token.ToString()))
            {
                // Determine it is unary or binary operator
                bool isUnary = pos == 0 || expression[pos - 1] == '(';
                pos++;

                switch (token.ToString())
                {
                    case "+":
                        return isUnary ? MathParser.unPlus : MathParser.plus;
                    case "-":
                        return isUnary ? MathParser.unMinus : MathParser.minus;
                    default:
                        return this._supportedOperators[token.ToString()];
                }
            }
            else if (Char.IsLetter(token[0])
                || this._supportedFunctions.ContainsKey(token.ToString())
                || this._supportedConstants.ContainsKey(token.ToString()))
            {
                // Read function or constant name

                while (++pos < expression.Length
                    && Char.IsLetter(expression[pos]))
                {
                    token.Append(expression[pos]);
                }

                if (this._supportedFunctions.ContainsKey(token.ToString()))
                {
                    return this._supportedFunctions[token.ToString()];
                }
                else if (this._supportedConstants.ContainsKey(token.ToString()))
                {
                    return this._supportedConstants[token.ToString()];
                }
                else
                {
                    throw new ArgumentException("Unknown token");
                }

            }
            else if (Char.IsDigit(token[0]) || token[0] == this._decimalSeparator)
            {
                // Read number

                // Read the whole part of number
                if (Char.IsDigit(token[0]))
                {
                    while (++pos < expression.Length
                    && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                }
                else
                {
                    // Because system decimal separator
                    // will be added below
                    token.Clear();
                }

                // Read the fractional part of number
                if (pos < expression.Length
                    && expression[pos] == this._decimalSeparator)
                {
                    // Add current system specific decimal separator
                    token.Append(CultureInfo.CurrentCulture
                        .NumberFormat.NumberDecimalSeparator);

                    while (++pos < expression.Length
                    && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                }

                // Read scientific notation (suffix)
                if (pos + 1 < expression.Length && expression[pos] == 'e'
                    && (Char.IsDigit(expression[pos + 1])
                        || (pos + 2 < expression.Length
                            && (expression[pos + 1] == '+'
                                || expression[pos + 1] == '-')
                            && Char.IsDigit(expression[pos + 2]))))
                {
                    token.Append(expression[pos++]); // e

                    if (expression[pos] == '+' || expression[pos] == '-')
                        token.Append(expression[pos++]); // sign

                    while (pos < expression.Length
                        && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos++]); // power
                    }

                    // Convert number from scientific notation to decimal notation
                    return MathParser.numberMaker + Convert.ToDouble(token.ToString());
                }

                return MathParser.numberMaker + token.ToString();
            }
            else
            {
                throw new ArgumentException("Unknown token in expression");
            }
        }

        /// <summary>
        /// Syntax analysis of infix notation
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="outputString">Output string (math expression in RPN)</param>
        /// <param name="stack">Stack which contains operators (or functions)</param>
        /// <returns>Output string (math expression in RPN)</returns>
        private StringBuilder SyntaxAnalysisInfixNotation(string token, StringBuilder outputString, Stack<string> stack)
        {
            // If it's a number just put to string            
            if (token[0] == MathParser.numberMaker[0])
            {
                outputString.Append(token);
            }
            else if (token[0] == MathParser.functionMarker[0])
            {
                // if it's a function push to stack
                stack.Push(token);
            }
            else if (token == MathParser.leftParent)
            {
                // If its '(' push to stack
                stack.Push(token);
            }
            else if (token == MathParser.rightParent)
            {
                // If its ')' pop elements from stack to output string
                // until find the ')'

                string elem;
                while ((elem = stack.Pop()) != MathParser.leftParent)
                {
                    outputString.Append(elem);
                }

                // if after this a function is in the peek of stack then put it to string
                if (stack.Count > 0 &&
                    stack.Peek()[0] == MathParser.functionMarker[0])
                {
                    outputString.Append(stack.Pop());
                }
            }
            else
            {
                // While priority of elements at peek of stack >= (>) token's priority
                // put these elements to output string
                while (stack.Count > 0 &&
                    Priority(token, stack.Peek()))
                {
                    outputString.Append(stack.Pop());
                }

                stack.Push(token);
            }

            return outputString;
        }

        /// <summary>
        /// Is priority of token less (or equal) to priority of p
        /// </summary>
        private bool Priority(string token, string p)
        {
            return IsRightAssociated(token) ?
                GetPriority(token) < GetPriority(p) :
                GetPriority(token) <= GetPriority(p);
        }

        /// <summary>
        /// Is right associated operator
        /// </summary>
        private bool IsRightAssociated(string token)
        {
            return token == MathParser.degree;
        }

        /// <summary>
        /// Get priority of operator
        /// </summary>
        private int GetPriority(string token)
        {
            switch (token)
            {
                case MathParser.leftParent:
                    return 0;
                case MathParser.plus:
                case MathParser.minus:
                    return 2;
                case MathParser.unPlus:
                case MathParser.unMinus:
                    return 6;
                case MathParser.multiply:
                case MathParser.divide:
                    return 4;
                case MathParser.degree:
                case MathParser.sqrt:
                    return 8;
                case MathParser.sin:
                case MathParser.cos:
                case MathParser.tg:
                case MathParser.ctg:
                case MathParser.sh:
                case MathParser.ch:
                case MathParser.th:
                case MathParser.log:
                case MathParser.ln:
                case MathParser.exp:
                case MathParser.abs:
                    return 10;
                default:
                    throw new ArgumentException("Unknown operator");
            }
        }

        #endregion

        #region Calculate expression in RPN

        /// <summary>
        /// Calculate expression in reverse-polish notation
        /// </summary>
        /// <param name="expression">Math expression in reverse-polish notation</param>
        /// <returns>Result</returns>
        private double Calculate(string expression)
        {
            int pos = 0; // Current position of lexical analysis
            Stack<double> stack = new Stack<double>(); // Contains operands

            // Analyse entire expression
            while (pos < expression.Length)
            {
                string token = LexicalAnalysisRpn(expression, ref pos);

                stack = SyntaxAnalysisRpn(stack, token);
            }

            // At end of analysis in stack should be only one operand (result)
            if (stack.Count > 1)
            {
                throw new ArgumentException("Excess operand");
            }

            return stack.Pop();
        }

        /// <summary>
        /// Produce token by the given math expression
        /// </summary>
        /// <param name="expression">Math expression in reverse-polish notation</param>
        /// <param name="pos">Current position of lexical analysis</param>
        /// <returns>Token</returns>
        private string LexicalAnalysisRpn(string expression, ref int pos)
        {
            StringBuilder token = new StringBuilder();

            // Read token from marker to next marker

            token.Append(expression[pos++]);

            while (pos < expression.Length && expression[pos] != MathParser.numberMaker[0]
                && expression[pos] != MathParser.operatorMarker[0]
                && expression[pos] != MathParser.functionMarker[0])
            {
                token.Append(expression[pos++]);
            }

            return token.ToString();
        }

        /// <summary>
        /// Syntax analysis of reverse-polish notation
        /// </summary>
        /// <param name="stack">Stack which contains operands</param>
        /// <param name="token">Token</param>
        /// <returns>Stack which contains operands</returns>
        private Stack<double> SyntaxAnalysisRpn(Stack<double> stack, string token)
        {
            // if it's operand then just push it to stack
            if (token[0] == MathParser.numberMaker[0])
            {
                stack.Push(double.Parse(token.Remove(0, 1)));
            }
            // Otherwise apply operator or function to elements in stack
            else if (NumberOfArguments(token) == 1)
            {
                double arg = stack.Pop();
                double rst;

                switch (token)
                {
                    case MathParser.unPlus:
                        rst = arg;
                        break;
                    case MathParser.unMinus:
                        rst = -arg;
                        break;
                    case MathParser.sqrt:
                        rst = Math.Sqrt(arg);
                        break;
                    case MathParser.sin:
                        rst = ApplyTrigFunction(Math.Sin, arg);
                        break;
                    case MathParser.cos:
                        rst = ApplyTrigFunction(Math.Cos, arg);
                        break;
                    case MathParser.tg:
                        rst = ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case MathParser.ctg:
                        rst = 1 / ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case MathParser.sh: 
                        rst = Math.Sinh(arg); 
                        break;
                    case MathParser.ch:
                        rst = Math.Cosh(arg); 
                        break;
                    case MathParser.th:
                        rst = Math.Tanh(arg); 
                        break;
                    case MathParser.ln: 
                        rst = Math.Log(arg); 
                        break;
                    case MathParser.exp: 
                        rst = Math.Exp(arg); 
                        break;
                    case MathParser.abs: 
                        rst = Math.Abs(arg); 
                        break;
                    default:
                        throw new ArgumentException("Unknown operator");
                }

                stack.Push(rst);
            }
            else
            {
                // otherwise operator's number of arguments equals to 2

                double arg2 = stack.Pop();
                double arg1 = stack.Pop();

                double rst;

                switch (token)
                {
                    case MathParser.plus:
                        rst = arg1 + arg2;
                        break;
                    case MathParser.minus:
                        rst = arg1 - arg2;
                        break;
                    case MathParser.multiply:
                        rst = arg1 * arg2;
                        break;
                    case MathParser.divide:
                        if (arg2 == 0)
                        {
                            throw new DivideByZeroException("Second argument is zero");
                        }
                        rst = arg1 / arg2;
                        break;
                    case MathParser.degree:
                        rst = Math.Pow(arg1, arg2);
                        break;
                    case MathParser.log:
                        rst = Math.Log(arg2, arg1);
                        break;
                    default:
                        throw new ArgumentException("Unknown operator");
                }

                stack.Push(rst);
            }

            return stack;
        }

        /// <summary>
        /// Apply trigonometric function
        /// </summary>
        /// <param name="func">Trigonometric function</param>
        /// <param name="arg">Argument</param>
        /// <returns>Result of function</returns>
        private double ApplyTrigFunction(Func<double, double> func, double arg)
        {
            if (!this._isRadians)
            {
                arg = arg * Math.PI / 180; // Convert value to degree
            }

            return func(arg);
        }

        /// <summary>
        /// Produce number of arguments for the given operator
        /// </summary>
        private int NumberOfArguments(string token)
        {
            switch (token)
            {
                case MathParser.unPlus:
                case MathParser.unMinus:
                case MathParser.sqrt:
                case MathParser.tg:
                case MathParser.sh:
                case MathParser.ch:
                case MathParser.th:
                case MathParser.ln:
                case MathParser.ctg:
                case MathParser.sin:
                case MathParser.cos:
                case MathParser.exp:
                case MathParser.abs:
                    return 1;
                case MathParser.plus:
                case MathParser.minus:
                case MathParser.multiply:
                case MathParser.divide:
                case MathParser.degree:
                case MathParser.log:
                    return 2;
                default:
                    throw new ArgumentException("Unknown operator");
            }
        }

        #endregion
    }
}