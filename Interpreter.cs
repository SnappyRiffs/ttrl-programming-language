using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TTRL
{
    static class Interpreter
    {
        // Dictionaries for each variable type
        public static Dictionary<string, string> string_variables = new();
        public static Dictionary<string, int> int_variables = new();
        public static Dictionary<string, float> float_variables = new();
        public static Dictionary<string, bool> bool_variables = new();

        // Store functions
        public static Dictionary<string, (List<string> parameters, List<string> body)> functions = new();

        public static void Start()
        {
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
                string code = File.ReadAllText(Path.Combine(file, "main.ttrl"));

                // --- FUNCTION PARSING ---
                var matches = Regex.Matches(code, @"func\s+(\w+)\((.*?)\)\s*\{([\s\S]*?)\}");
                foreach (Match m in matches)
                {
                    string funcName = m.Groups[1].Value;
                    string paramList = m.Groups[2].Value.Trim();
                    string funcBody = m.Groups[3].Value.Trim();

                    var parameters = paramList.Length > 0
                        ? paramList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.Trim()).ToList()
                        : new List<string>();

                    functions[funcName] = (parameters, funcBody.Split('\n').Select(l => l.Trim()).ToList());
                }

                // Remove function definitions from the main execution
                string codeNoFuncs = Regex.Replace(code, @"func\s+\w+\([^)]*\)\s*\{[\s\S]*?\}", "");

                // Now process line by line
                string[] lines = codeNoFuncs.Split('\n');
                foreach (string rawLine in lines)
                {
                    ExecuteLine(rawLine.Trim());
                }
            }
        }

        private static void ExecuteLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) return;

            // --- COMMAND HANDLING ---

            // PRINT command
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
                    result = bool_variables[tokens[1]].ToString().ToLower();
                else
                    result = string.Join(' ', tokens.Skip(1));

                Console.WriteLine(result);
            }

            // STRING declaration
            else if (tokens[0] == "string" && tokens.Length > 2)
            {
                string_variables[tokens[1]] = string.Join(" ", tokens.Skip(2));
            }

            // INT declaration
            else if (tokens[0] == "int" && tokens.Length > 2)
            {
                var stack = MathFunctions.MakeStack(tokens.Skip(2));
                string result = MathFunctions.EvaluateStack(stack);
                int_variables[tokens[1]] = int.Parse(result);
            }

            // FLOAT declaration
            else if (tokens[0] == "float" && tokens.Length > 2)
            {
                var stack = MathFunctions.MakeStack(tokens.Skip(2));
                string result = MathFunctions.EvaluateStack(stack);
                float_variables[tokens[1]] = float.Parse(result);
            }

            // BOOL declaration
            else if (tokens[0] == "bool" && tokens.Length > 1)
            {
                var stack = MathFunctions.MakeStack(tokens.Skip(1));
                string result = MathFunctions.EvaluateStack(stack);
                bool_variables[tokens[1]] = bool.Parse(result);
            }

            // Assign to existing int variable
            else if (int_variables.ContainsKey(tokens[0]) ||
                     (tokens.Length == 2 && (tokens[0] == "++" || tokens[0] == "--") && int_variables.ContainsKey(tokens[1])))
            {
                if (int_variables.ContainsKey(tokens[0]) && tokens.Length == 2)
                {
                    if (tokens[1] == "++") int_variables[tokens[0]]++;
                    else if (tokens[1] == "--") int_variables[tokens[0]]--;
                }
                else if ((tokens[0] == "++" || tokens[0] == "--") && tokens.Length == 2 && int_variables.ContainsKey(tokens[1]))
                {
                    if (tokens[0] == "++") int_variables[tokens[1]]++;
                    else if (tokens[0] == "--") int_variables[tokens[1]]--;
                }
                else if (int_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                {
                    var stack = MathFunctions.MakeStack(tokens.Skip(1));
                    string result = MathFunctions.EvaluateStack(stack);
                    int_variables[tokens[0]] = int.Parse(result);
                }
            }

            // Assign to existing float variable
            else if (float_variables.ContainsKey(tokens[0]) ||
                     (tokens.Length == 2 && (tokens[0] == "++" || tokens[0] == "--") && float_variables.ContainsKey(tokens[1])))
            {
                if (float_variables.ContainsKey(tokens[0]) && tokens.Length == 2)
                {
                    if (tokens[1] == "++") float_variables[tokens[0]]++;
                    else if (tokens[1] == "--") float_variables[tokens[0]]--;
                }
                else if ((tokens[0] == "++" || tokens[0] == "--") && tokens.Length == 2 && float_variables.ContainsKey(tokens[1]))
                {
                    if (tokens[0] == "++") float_variables[tokens[1]]++;
                    else if (tokens[0] == "--") float_variables[tokens[1]]--;
                }
                else if (float_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
                {
                    var stack = MathFunctions.MakeStack(tokens.Skip(1));
                    string result = MathFunctions.EvaluateStack(stack);
                    float_variables[tokens[0]] = float.Parse(result);
                }
            }

            // Assign to existing string variable
            else if (string_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
            {
                string_variables[tokens[0]] = string.Join(" ", tokens.Skip(1));
            }

            // Assign to existing bool variable
            else if (bool_variables.ContainsKey(tokens[0]) && tokens.Length > 1)
            {
                var stack = MathFunctions.MakeStack(tokens.Skip(1));
                string result = MathFunctions.EvaluateStack(stack);
                bool_variables[tokens[0]] = bool.Parse(result);
            }

            // MATH command
            else if (tokens[0] == "math" && tokens.Length > 1)
            {
                var stack = MathFunctions.MakeStack(tokens.Skip(1));
                Console.WriteLine(MathFunctions.EvaluateStack(stack));
            }

            // COMMENT line
            else if (tokens[0].StartsWith("//"))
            {
                return;
            }

            // FUNCTION CALL with optional arguments
            else if (line.Contains("(") && line.EndsWith(")"))
            {
                string funcName = line.Substring(0, line.IndexOf("("));
                string argsPart = line.Substring(line.IndexOf("(") + 1, line.Length - funcName.Length - 2);

                var args = argsPart.Length > 0
                    ? argsPart.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList()
                    : new List<string>();

                if (functions.ContainsKey(funcName))
                {
                    var (parameters, body) = functions[funcName];

                    // Map args to parameters (basic support: strings, numbers, vars)
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        string paramName = parameters[i];
                        string argValue = (i < args.Count) ? args[i] : "";

                        if (int.TryParse(argValue, out int intVal))
                            int_variables[paramName] = intVal;
                        else if (float.TryParse(argValue, out float floatVal))
                            float_variables[paramName] = floatVal;
                        else if (bool.TryParse(argValue, out bool boolVal))
                            bool_variables[paramName] = boolVal;
                        else if (string_variables.ContainsKey(argValue))
                            string_variables[paramName] = string_variables[argValue];
                        else if (int_variables.ContainsKey(argValue))
                            int_variables[paramName] = int_variables[argValue];
                        else if (float_variables.ContainsKey(argValue))
                            float_variables[paramName] = float_variables[argValue];
                        else if (bool_variables.ContainsKey(argValue))
                            bool_variables[paramName] = bool_variables[argValue];
                        else
                            string_variables[paramName] = argValue; // default to string
                    }

                    // Execute function body
                    foreach (var funcLine in body)
                    {
                        ExecuteLine(funcLine);
                    }
                }
            }
            

            // EXIT command
            else if (tokens[0] == "exit")
            {
                Environment.Exit(0);
            }
        }
    }
}
