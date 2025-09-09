
using System;
using System.Linq;
using System.Numerics;

namespace TTRL
{
    static class Interpreter
    {

        public static Dictionary<string, string> string_variables = new Dictionary<string, string>();
        public static Dictionary<string, int> int_variables = new Dictionary<string, int>();
        public static Dictionary<string, float> float_variables = new Dictionary<string, float>();

        
       


        public static void Start()
        {
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
                            List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                            string result = MathFunctions.EvaluateMathStack(stack);
                            int_variables.Add(tokens[1], int.Parse(result));
                        }
                        else if (tokens[0] == "float")
                        {
                            List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                            string result = MathFunctions.EvaluateMathStack(stack);
                            float_variables.Add(tokens[1], float.Parse(result));
                        }
                        else if (string_variables.ContainsKey(tokens[0]))
                        {
                            var ToSet = tokens.Skip(1);
                            string joined = String.Join(" ", ToSet);
                            string_variables[tokens[0]] = joined;
                        }
                        else if (int_variables.ContainsKey(tokens[0]))
                        {
                            List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                            string result = MathFunctions.EvaluateMathStack(stack);
                            int_variables[tokens[0]] = int.Parse(result);
                        }
                        else if (float_variables.ContainsKey(tokens[0]))
                        {
                            List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                            string result = MathFunctions.EvaluateMathStack(stack);
                            float_variables[tokens[0]] = float.Parse(result);
                        }

                        else if (tokens[0] == "math")
                        {
                            List<string> stack = MathFunctions.MakeMathStack(tokens);
                            Console.WriteLine(MathFunctions.EvaluateMathStack(stack));

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
