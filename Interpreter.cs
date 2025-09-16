using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TTRL
{
    public class IncompatibleParamTypeError : Exception
    {
        public IncompatibleParamTypeError(string message) : base(message) { }
    }

    static class Interpreter
    {
        // Dictionaries for each variable type
        public static Dictionary<string, string> string_variables = new();
        public static Dictionary<string, int> int_variables = new();
        public static Dictionary<string, float> float_variables = new();
        public static Dictionary<string, bool> bool_variables = new();

        // Store functions: Name -> (ReturnType, Parameters, Body)
        public static Dictionary<string, (string ReturnType, List<(string Name, string Type)> Parameters, List<string> Body)> functions = new();

        public static void Start()
        {
            List<string> Files = new List<string>();
            Console.WriteLine("Started interpreter");

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
                var matches = Regex.Matches(code, @"func:\s*(\w+)\s+(\w+)\((.*?)\)\s*\{([\s\S]*?)\}");
                foreach (Match m in matches)
                {
                    // Group 1 from the regex captures the return type of the function.
                    // Example: "int" in "func: int add(a: int, b: int) { ... }"
                    string returnType = m.Groups[1].Value;

                    // Group 2 captures the function name.
                    // Example: "add" in "func: int add(a: int, b: int) { ... }"
                    string funcName = m.Groups[2].Value;

                    // Group 3 captures the parameter list inside the parentheses.
                    // Example: "a: int, b: int" in "func: int add(a: int, b: int) { ... }"
                    // Trim() removes any leading or trailing whitespace.
                    string paramList = m.Groups[3].Value.Trim();

                    // Group 4 captures the function body inside the curly braces.
                    // Example: "return a + b" in the add function.
                    // Trim() removes leading/trailing whitespace from the whole body string.
                    string funcBody = m.Groups[4].Value.Trim();


                    // Check if the parameter list is not empty
                    var parameters = paramList.Length > 0
                        ? paramList
                            // Split the parameters by comma (a: int, b: int → ["a: int", "b: int"])
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            // For each parameter, process it into a (Name, Type) tuple
                            .Select(p => {
                                // Split the parameter by ':' into name and type (e.g., "a: int" → ["a", " int"])
                                var parts = p.Split(':', 2);
                                // Trim whitespace and return a tuple with Name and Type
                                return (Name: parts[0].Trim(), Type: parts[1].Trim());
                            })
                            // Convert the IEnumerable of tuples into a List
                            .ToList()
                        // If there are no parameters, create an empty list
                        : new List<(string Name, string Type)>();

                    // Store the parsed function in the 'functions' dictionary
                    // Key: funcName (e.g., "add")
                    // Value: a tuple containing:
                    //   ReturnType: the return type of the function (e.g., "int")
                    //   Parameters: the list of (Name, Type) tuples parsed above
                    //   Body: list of lines in the function body, each trimmed of whitespace
                    functions[funcName] = (
                        ReturnType: returnType,
                        Parameters: parameters,
                        Body: funcBody.Split('\n')
                                    .Select(l => l.Trim())
                                    .ToList()
                    );

                }

                // Remove function definitions from the main execution
                string codeNoFuncs = Regex.Replace(code, @"func:\s*(\w+)\s+(\w+)\((.*?)\)\s*\{([\s\S]*?)\}", "");

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
                return;
            }

            // VARIABLE DECLARATIONS
            if (tokens.Length > 2 && (tokens[0] == "string" || tokens[0] == "int" || tokens[0] == "float" || tokens[0] == "bool"))
            {
                string varName = tokens[1];
                string valueStr = string.Join(' ', tokens.Skip(2));

                switch (tokens[0])
                {
                    case "string":
                        string_variables[varName] = valueStr;
                        break;

                    case "int":
                        int_variables[varName] = int.Parse(valueStr);
                        break;

                    case "float":
                        float_variables[varName] = float.Parse(valueStr);
                        break;

                    case "bool":
                        bool_variables[varName] = bool.Parse(valueStr);
                        break;
                }
                return;
            }

            // COMMENT
            if (tokens[0].StartsWith("//")) return;

            // FUNCTION CALL
            if (line.Contains("(") && line.EndsWith(")"))
            {
                string funcName = line.Substring(0, line.IndexOf("("));
                string argsPart = line.Substring(line.IndexOf("(") + 1, line.Length - funcName.Length - 2);

                var args = argsPart.Length > 0
                    ? argsPart.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList()
                    : new List<string>();

                if (!functions.ContainsKey(funcName))
                    throw new Exception($"Function '{funcName}' not found.");

                var (returnType, parameters, body) = functions[funcName];

                if (args.Count != parameters.Count)
                    throw new Exception($"Function '{funcName}' expects {parameters.Count} arguments, got {args.Count}.");

                // Typed enforcement
                for (int i = 0; i < parameters.Count; i++)
                {
                    string paramName = parameters[i].Name;
                    string paramType = parameters[i].Type;
                    string argValue = args[i];

                    switch (paramType)
                    {
                        case "string":
                            if (string_variables.ContainsKey(argValue))
                                string_variables[paramName] = string_variables[argValue];
                            else
                                string_variables[paramName] = argValue;
                            break;
                        
                        case "int":
                            if (int.TryParse(argValue, out int intVal))
                                int_variables[paramName] = intVal;
                            else if (int_variables.ContainsKey(argValue))
                                int_variables[paramName] = int_variables[argValue];
                            else
                                throw new IncompatibleParamTypeError($"Parameter '{paramName}' expects type 'int' but got '{argValue}'.");
                            break;
                        
                        case "float":
                            if (float.TryParse(argValue, out float floatVal))
                                float_variables[paramName] = floatVal;
                            else if (float_variables.ContainsKey(argValue))
                                float_variables[paramName] = float_variables[argValue];
                            else
                                throw new IncompatibleParamTypeError($"Parameter '{paramName}' expects type 'float' but got '{argValue}'.");
                            break;

                        case "bool":
                            if (bool.TryParse(argValue, out bool boolVal))
                                bool_variables[paramName] = boolVal;
                            else if (bool_variables.ContainsKey(argValue))
                                bool_variables[paramName] = bool_variables[argValue];
                            else
                                throw new IncompatibleParamTypeError($"Parameter '{paramName}' expects type 'bool' but got '{argValue}'.");
                            break;

                        default:
                            throw new Exception($"Unknown parameter type '{paramType}' for '{paramName}'.");
                    }
                }

                // Execute function body
                foreach (var funcLine in body)
                    ExecuteLine(funcLine);

                return;
            }

            // EXIT
            if (tokens[0] == "exit")
            {
                Environment.Exit(0);
            }
        }
    }
}
