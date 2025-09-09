using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;

namespace TTRL
{
    static class Interpreter
    {
        // Dictionaries to store variables by type
        // Example: string_variables["name"] = "Alice"
        public static Dictionary<string, string> string_variables = new();
        public static Dictionary<string, int> int_variables = new();
        public static Dictionary<string, float> float_variables = new();

        public static void Start()
        {
            // Stores all paths loaded from path.cfg
            List<string> Files = new List<string>();
            Console.WriteLine("started interpreter");

            // Read configuration file (contains "path ..." lines)
            string[] conflines = File.ReadAllLines("path.cfg");
            foreach (string confline in conflines)
            {
                // Split each config line into tokens, ignoring extra spaces
                string[] tok = confline.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tok.Length == 0) continue; // skip empty lines

                // If the line starts with "path", store the directory
                if (tok[0].Equals("path", StringComparison.OrdinalIgnoreCase) && tok.Length > 1)
                {
                    Files.Add(tok[1]);
                }
                // Ignore comment lines starting with "#"
                else if (tok[0].StartsWith("#"))
                {
                    continue;
                }
            }

            // Loop through each configured path
            foreach (string file in Files)
            {
                // Load the script file "main.ttrl" inside that path
                string[] lines = File.ReadAllLines(Path.Combine(file, "main.ttrl"));

                // Process each line of the script
                foreach (string line in lines)
                {
                    // Split script line into tokens by spaces
                    string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0) continue; // skip empty lines

                    // --- Handle commands ---
                    
                    // PRINT command → prints variable or text
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
                            result = string.Join(' ', tokens.Skip(1)); // print raw text

                        Console.WriteLine(result);
                    }
                    // STRING command → declare or assign a string variable
                    else if (tokens[0] == "string" && tokens.Length > 2)
                    {
                        string_variables[tokens[1]] = string.Join(" ", tokens.Skip(2));
                    }
                    // INT command → declare or assign an int variable (evaluated via MathFunctions)
                    else if (tokens[0] == "int" && tokens.Length > 2)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        int_variables[tokens[1]] = int.Parse(result);
                    }
                    // FLOAT command → declare or assign a float variable (evaluated via MathFunctions)
                    else if (tokens[0] == "float" && tokens.Length > 2)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        float_variables[tokens[1]] = float.Parse(result);
                    }
                    // Assign to an existing string variable
                    else if (string_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        string_variables[tokens[0]] = string.Join(" ", tokens.Skip(1));
                    }
                    // Assign to an existing int variable
                    else if (int_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        int_variables[tokens[0]] = int.Parse(result);
                    }
                    // Assign to an existing float variable
                    else if (float_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        float_variables[tokens[0]] = float.Parse(result);
                    }
                    // MATH command → evaluate math expression and print result
                    else if (tokens[0] == "math" && tokens.Length > 1)
                    {
                        var stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                        Console.WriteLine(MathFunctions.EvaluateMathStack(stack));
                    }
                    // Comment line (starts with //) → skip
                    else if (tokens[0].StartsWith("//"))
                    {
                        continue;
                    }
                    // EXIT command → end interpreter
                    else if (tokens[0] == "exit")
                    {
                        Environment.Exit(0); // kills entire program
                        // Or: return; // use this if you only want to stop script execution
                    }
                }
            }
        }
    }
}
