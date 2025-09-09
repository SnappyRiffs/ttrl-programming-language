namespace TTRL
{
    static class MathFunctions
    {
        public static string operate(string operand1, string operation, string operand2)
        {
            float op1 = float.Parse(operand1);
            float op2 = float.Parse(operand2);
            if (operation == "/")
            {
                return (op1 / op2).ToString();
            }
            else if (operation == "*")
            {
                return (op1 * op2).ToString();
            }
            else if (operation == "+")
            {
                return (op1 + op2).ToString();
            }
            else if (operation == "-")
            {
                return (op1 - op2).ToString();
            }
            else if (operation == "^")
            {
                return (Math.Pow(op1, op2)).ToString();
            }
            return "";
        }


        public static int findOperation(List<string> stack)
        {
            string[] Operators1 = { "^" };
            for (int n = 0; n < stack.Count; n++)
            {
                if (Operators1.Contains(stack[n]))
                {
                    return n;
                }
            }
            string[] priorityOperators = { "*", "/" };
            for (int n = 0; n < stack.Count; n++)
            {
                if (priorityOperators.Contains(stack[n]))
                {
                    return n;
                }
            }
            string[] lowOperators = { "+", "-" };
            for (int n = 0; n < stack.Count; n++)
            {
                if (lowOperators.Contains(stack[n]))
                {
                    return n;
                }
            }
            return -1;
        }

        public static List<string> MakeMathStack(IEnumerable<string> tokens)
        {
            List<string> stack = new List<string>();
            foreach (string token in tokens)
            {
                if (token.Equals("+"))
                {
                    stack.Add("+");
                }
                else if (token.Equals("-"))
                {
                    stack.Add("-");
                }
                else if (token.Equals("*"))
                {
                    stack.Add("*");
                }
                else if (token.Equals("/"))
                {
                    stack.Add("/");
                }
                else if (token.Equals("^"))
                {
                    stack.Add("^");
                }
                else if (float.TryParse(token, out float result))
                {
                    stack.Add(token);
                }
                else if (Interpreter.float_variables.ContainsKey(token))
                {
                    stack.Add(Interpreter.float_variables[token].ToString());
                }
                else if (Interpreter.int_variables.ContainsKey(token))
                {
                    stack.Add(Interpreter.int_variables[token].ToString());
                }
            }
            return stack;
        }


        public static string EvaluateMathStack(List<string> stack)
        {

            int operationPosition = findOperation(stack);
            while (operationPosition != -1)
            {
                string answer = operate(stack[operationPosition - 1], stack[operationPosition], stack[operationPosition + 1]);
                stack[operationPosition - 1] = answer;
                stack.RemoveAt(operationPosition + 1);
                stack.RemoveAt(operationPosition);
                operationPosition = findOperation(stack);
            }
            return stack[0];
        }
    }
}