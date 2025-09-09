using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;

namespace TTRL
{
    static class Interpreter
    {
        // Dictionaries for each variable type
        public static Dictionary<string, string> string_variables = new();
        public static Dictionary<string, int> int_variables = new();
        public static Dictionary<string, float> float_variables = new();
        public static Dictionary<string, bool> bool_variables = new(); // new bool dictionary

        public static void Start()
        {
            // List to store file paths from config
            List<string> Files = new List<string>();
            Console.WriteLine("started interpreter");

            // Read path.cfg to get script directories
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
                    continue; // skip comment lines
                }
            }

            // Loop through each file path
            foreach (string file in Files)
            {
                string[] lines = File.ReadAllLines(Path.Combine(file, "main.ttrl"));

                foreach (string line in lines)
                {
                    string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0) continue; // skip empty lines

                    // --- COMMAND HANDLING ---

                    // PRINT command → print variables or literal text
                    if (tokens[0] == "print" && tokens.Length > 1)
                    {
                        string result;
                        if (string_variables.ContainsKey(tokens[1]))
                            result = string_variables[tokens[1]];
                        else if (int_variables.ContainsKey(tokens[1]))
                            result = int_variables[tokens[1]].ToString();
                        else if (float_variables.ContainsKey(tokens[1]))
                            result = float_variables[tokens[1]].ToString();
                        else if (bool_variables.ContainsKey(tokens[1]))
                            result = bool_variables[tokens[1]].ToString().ToLower(); // print bool as "true"/"false"
                        else
                            result = string.Join(' ', tokens.Skip(1)); // print raw text

                        Console.WriteLine(result);
                    }
                    // STRING declaration/assignment
                    else if (tokens[0] == "string" && tokens.Length > 2)
                    {
                        string_variables[tokens[1]] = string.Join(" ", tokens.Skip(2));
                    }
                    // INT declaration/assignment
                    else if (tokens[0] == "int" && tokens.Length > 2)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        int_variables[tokens[1]] = int.Parse(result);
                    }
                    // FLOAT declaration/assignment
                    else if (tokens[0] == "float" && tokens.Length > 2)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        float_variables[tokens[1]] = float.Parse(result);
                    }
                    // BOOL declaration/assignment
                    else if (tokens[0] == "bool" && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        bool_variables[tokens[1]] = bool.Parse(result); // parse string "true"/"false"
                    }
                    // Assign to existing string variable
                    else if (string_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        string_variables[tokens[0]] = string.Join(" ", tokens.Skip(1));
                    }
                    // Assign to existing int variable
                    else if (int_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        int_variables[tokens[0]] = int.Parse(result);
                    }
                    // Assign to existing float variable
                    else if (float_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        float_variables[tokens[0]] = float.Parse(result);
                    }
                    // Assign to existing bool variable
                    else if (bool_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        bool_variables[tokens[0]] = bool.Parse(result);
                    }
                    // MATH command → evaluate expression
                    else if (tokens[0] == "math" && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        Console.WriteLine(MathFunctions.EvaluateMathStack(stack));
                    }
                    // COMMENT line → skip
                    else if (tokens[0].StartsWith("//"))
                    {
                        continue;
                    }
                    // EXIT command → stop interpreter
                    else if (tokens[0] == "exit")
                    {
                        Environment.Exit(0); // stops whole program
                        // return; // alternative to stop only this script
                    }
                }
            }
        }
    }
}
