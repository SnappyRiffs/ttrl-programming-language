using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;

namespace TTRL
{
    static class Interpreter
    {
        public static Dictionary<string, string> string_variables = new();
        public static Dictionary<string, int> int_variables = new();
        public static Dictionary<string, float> float_variables = new();

        public static void Start()
        {
            List<string> Files = new List<string>();
            Console.WriteLine("started interpreter");

            string[] conflines = File.ReadAllLines("path.cfg");
            foreach (string confline in conflines)
            {
                string[] tok = confline.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tok.Length == 0) continue;

                if (tok[0].Equals("path", StringComparison.OrdinalIgnoreCase) && tok.Length > 1)
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
                string[] lines = File.ReadAllLines(Path.Combine(file, "main.ttrl"));
                foreach (string line in lines)
                {
                    string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0) continue;

                    if (tokens[0] == "print" && tokens.Length > 1)
                    {
                        string result;
                        if (string_variables.ContainsKey(tokens[1]))
                            result = string_variables[tokens[1]];
                        else if (int_variables.ContainsKey(tokens[1]))
                            result = int_variables[tokens[1]].ToString();
                        else if (float_variables.ContainsKey(tokens[1]))
                            result = float_variables[tokens[1]].ToString();
                        else
                            result = string.Join(' ', tokens.Skip(1));

                        Console.WriteLine(result);
                    }
                    else if (tokens[0] == "string" && tokens.Length > 2)
                    {
                        string_variables[tokens[1]] = string.Join(" ", tokens.Skip(2));
                    }
                    else if (tokens[0] == "int" && tokens.Length > 2)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        int_variables[tokens[1]] = int.Parse(result);
                    }
                    else if (tokens[0] == "float" && tokens.Length > 2)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        float_variables[tokens[1]] = float.Parse(result);
                    }
                    else if (string_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        string_variables[tokens[0]] = string.Join(" ", tokens.Skip(1));
                    }
                    else if (int_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        int_variables[tokens[0]] = int.Parse(result);
                    }
                    else if (float_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        float_variables[tokens[0]] = float.Parse(result);
                    }
                    else if (tokens[0] == "math" && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        Console.WriteLine(MathFunctions.EvaluateMathStack(stack));
                    }
                    else if (tokens[0].StartsWith("//"))
                    {
                        continue;
                    }
                    else if (tokens[0] == "exit")
                    {
                        Environment.Exit(0);
                        // Or use: return; // if you only want to stop interpreting
                    }
                }
            }
        }
    }
}
