namespace TTRL
{
    static class MathFunctions
    {
        /// <summary>
        /// Performs arithmetic operations: +, -, *, /, ^.
        /// </summary>
        public static string operate(string operand1, string operation, string operand2)
        {
            float op1 = float.Parse(operand1);
            float op2 = float.Parse(operand2);

            switch (operation)
            {
                case "/" when op2 == 0:
                    throw new DivideByZeroException("Error: Division by zero.");
                case "/":
                    return (op1 / op2).ToString();
                case "*" :
                    return (op1 * op2).ToString();
                case "+" :
                    return (op1 + op2).ToString();
                case "-" :
                    return (op1 - op2).ToString();
                case "^" :
                    return Math.Pow(op1, op2).ToString();
                default:
                    throw new InvalidOperationException($"Error: Unknown operation '{operation}'.");
            }

            return "";
        }

        /// <summary>
        /// Performs boolean operations: &&, ||, ==, !=, xor
        /// </summary>
        public static string operateBool(string operand1, string operation, string operand2)
        {
            bool op1 = bool.Parse(operand1);
            bool op2 = bool.Parse(operand2);

            switch (operation)
            {
                case "&&":
                    return (op1 && op2).ToString().ToLower();
                case "||":
                    return (op1 || op2).ToString().ToLower();
                case "==":
                    return (op1 == op2).ToString().ToLower();
                case "!=":
                    return (op1 != op2).ToString().ToLower();
                case "xor":
                    return (op1 ^ op2).ToString().ToLower();
                default:
                    throw new InvalidOperationException($"Error: Unknown boolean operation '{operation}'.");
            }

            return "";
        }

        /// <summary>
        /// Finds next operation in stack based on precedence.
        /// Arithmetic: ^ > * / > + - 
        /// Boolean: && > || > == !=
        /// </summary>
        public static int findOperation(List<string> stack)
        {
            string[] high = { "^" };
            string[] medium = { "*", "/" };
            string[] low = { "+", "-" };
            string[] boolHigh = { "&&" };
            string[] boolMedium = { "||" };
            string[] boolLow = { "==", "!=", "xor" };

            foreach (string[] ops in new[] { high, medium, low, boolHigh, boolMedium, boolLow })
            {
                for (int n = 0; n < stack.Count; n++)
                    if (ops.Contains(stack[n])) return n;
            }

            return -1; // no operators left
        }

        /// <summary>
        /// Builds a math/logic stack from tokens
        /// Converts variables to values, keeps operators
        /// </summary>
        public static List<string> MakeStack(IEnumerable<string> tokens)
        {
            List<string> stack = new List<string>();

            foreach (string token in tokens)
            {
                // Operators
                if (new[] { "+", "-", "*", "/", "^", "&&", "||", "==", "!=", "xor" }.Contains(token))
                {
                    stack.Add(token);
                }
                // Numbers
                else if (float.TryParse(token, out float f)) stack.Add(token);
                // Float variables
                else if (Interpreter.float_variables.ContainsKey(token)) stack.Add(Interpreter.float_variables[token].ToString());
                // Int variables
                else if (Interpreter.int_variables.ContainsKey(token)) stack.Add(Interpreter.int_variables[token].ToString());
                // Bool variables
                else if (Interpreter.bool_variables.ContainsKey(token)) stack.Add(Interpreter.bool_variables[token].ToString().ToLower());
            }

            return stack;
        }

        /// <summary>
        /// Evaluates a stack until a single result remains
        /// Automatically chooses arithmetic or boolean operation
        /// </summary>
        public static string EvaluateStack(List<string> stack)
        {
            int operationPosition = findOperation(stack);

            while (operationPosition != -1)
            {
                string op = stack[operationPosition];

                string answer;

                // Boolean operators
                if (new[] { "&&", "||", "==", "!=" }.Contains(op))
                {
                    answer = operateBool(stack[operationPosition - 1], op, stack[operationPosition + 1]);
                }
                // Arithmetic operators
                else
                {
                    answer = operate(stack[operationPosition - 1], op, stack[operationPosition + 1]);
                }

                // Replace left operand with result, remove operator and right operand
                stack[operationPosition - 1] = answer;
                stack.RemoveAt(operationPosition + 1);
                stack.RemoveAt(operationPosition);

                operationPosition = findOperation(stack);
            }

            return stack[0]; // final result
        }
    }
}
