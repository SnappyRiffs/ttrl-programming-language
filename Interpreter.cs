
using System;
using System.Linq;
using System.Numerics;

namespace TTL
{
    static class Interpreter
    {

        private static string operate(string operand1, string operation, string operand2)
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


        private static int findOperation(List<string> stack)
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


        public static void Start()
        {
            Dictionary<string, string> string_variables = new Dictionary<string, string>();
            Dictionary<string, int> int_variables = new Dictionary<string, int>();
            Dictionary<string, float> float_variables = new Dictionary<string, float>();
            List<string> Files = [ ];
            Console.WriteLine("started interpreter");
            string[] conflines = File.ReadAllLines("path.cfg");
            foreach (string confline in conflines)
            {
                string[] tok = confline.Split(' ');
                if (tok[0].Equals("path"))
                {
                    Files.Add(tok[1]);
                }
                else if (tok[0].StartsWith("#"))
                {
                    continue;
                }
            }
            foreach (string file in Files)
            {

                string[] lines = File.ReadAllLines(file + "main.ttrl");
                foreach (string line in lines)
                {
                    string[] tokens = line.Split(' ');
                    if (tokens.Length > 0)
                    {

                        if (tokens[0] == "print")
                        {
                            string result;
                            if (string_variables.ContainsKey(tokens[1]))
                            {
                                result = string_variables[tokens[1]];
                            }
                            else if (int_variables.ContainsKey(tokens[1]))
                            {
                                result = int_variables[tokens[1]].ToString();
                            }
                            else if (float_variables.ContainsKey(tokens[1]))
                            {
                                result = float_variables[tokens[1]].ToString();
                            }
                            else
                            {
                                var ToPrint = tokens.Skip(1);
                                result = String.Join(' ', ToPrint);
                            }
                            Console.WriteLine($"{result}");
                        }
                        else if (tokens[0] == "string")
                        {
                            var ToJoin = tokens.Skip(2);
                            string joined = String.Join(" ", ToJoin);
                            string_variables.Add(tokens[1], joined);
                        }
                        else if (tokens[0] == "int")
                        {
                            var ToJoin = tokens.Skip(2);
                            string joined = String.Join(" ", ToJoin);
                            int_variables.Add(tokens[1], int.Parse(joined));
                        }
                        else if (tokens[0] == "float")
                        {
                            var ToJoin = tokens.Skip(2);
                            string joined = String.Join(" ", ToJoin);
                            float_variables.Add(tokens[1], float.Parse(joined));
                        }
                        else if (string_variables.ContainsKey(tokens[0]))
                        {
                            var ToSet = tokens.Skip(1);
                            string joined = String.Join(" ", ToSet);
                            string_variables[tokens[0]] = joined;
                        }
                        else if (int_variables.ContainsKey(tokens[0]))
                        {
                            var ToSet = tokens.Skip(1);
                            string joined = String.Join("", ToSet);
                            int_variables[tokens[0]] = int.Parse(joined);
                        }
                        else if (float_variables.ContainsKey(tokens[0]))
                        {
                            var ToSet = tokens.Skip(1);
                            string joined = String.Join("", ToSet);
                            float_variables[tokens[0]] = float.Parse(joined);
                        }

                        else if (tokens[0] == "math")
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
                                else if (float_variables.ContainsKey(token))
                                {
                                    stack.Add(float_variables[token].ToString());
                                }
                                else if (int_variables.ContainsKey(token))
                                {
                                    stack.Add(int_variables[token].ToString());
                                }
                            }
                            //Console.WriteLine(String.Join(',', stack));

                            int operationPosition = findOperation(stack);
                            while (operationPosition != -1)
                            {
                                string answer = operate(stack[operationPosition - 1], stack[operationPosition], stack[operationPosition + 1]);
                                stack[operationPosition - 1] = answer;
                                stack.RemoveAt(operationPosition + 1);
                                stack.RemoveAt(operationPosition);
                                operationPosition = findOperation(stack);
                            }
                            Console.WriteLine(stack[0]);

                        }
                        else if (tokens[0].StartsWith("//") || tokens[0].StartsWith("//"))
                        {
                            continue;
                        }
                        else if (tokens[0] == "exit")
                        {
                            Environment.Exit(0);
                        }
                    }
                }
            }
         
            
        }
    }
}
