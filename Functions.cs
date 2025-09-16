using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TTRL
{
    static class Functions
    {
        public static void MakeFunction(string[] tokens, Dictionary<string, string[]> functions, string[] lines, Dictionary<string, string[]> function_args)
        {
            string function_name = tokens[1];
            string arguments = string.Join("", tokens[2..^0]);
            char[] parens = { '(', ')' };
            arguments = arguments.Trim(parens);
            string[] argument_list = arguments.Split(",");
            function_args[function_name] = argument_list;
            string[] remaining_lines = lines[Interpreter.line_counter..^0];
            string pat = @"\{(.*?)\}";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(string.Join("\n", remaining_lines));
            if (m.Success)
            {
                string Function_Contents = m.Groups[1].Captures[0].ToString();
                functions[function_name] = Function_Contents.Split("\n");
                int count = Regex.Matches(Function_Contents, "\n").Count;
                Interpreter.line_counter = Interpreter.line_counter + count;
            }
            else
            {
                Console.WriteLine("Start delimiter not found.");
            }
        }

        public static void CallFunction(string[] lines)
        {
            for (int line_counter = 0; line_counter < lines.Length; line_counter++)
            {
                string line = lines[line_counter].Trim();
                string[] tokens = line.Split(' ');
                if (tokens.Length > 0)
                {

                    if (tokens[0] == "print")
                    {
                        string result;
                        if (Interpreter.string_variables.ContainsKey(tokens[1]))
                        {
                            result = Interpreter.string_variables[tokens[1]];
                        }
                        else if (Interpreter.int_variables.ContainsKey(tokens[1]))
                        {
                            result = Interpreter.int_variables[tokens[1]].ToString();
                        }
                        else if (Interpreter.float_variables.ContainsKey(tokens[1]))
                        {
                            result = Interpreter.float_variables[tokens[1]].ToString();
                        }
                        else
                        {
                            var ToPrint = tokens.Skip(1);
                            result = string.Join(' ', ToPrint);
                        }
                        Console.WriteLine($"{result}");
                    }
                    else if (tokens[0] == "string")
                    {
                        var ToJoin = tokens.Skip(2);
                        string joined = string.Join(" ", ToJoin);
                        Interpreter.string_variables.Add(tokens[1], joined);
                    }
                    else if (tokens[0] == "int")
                    {
                        List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        Interpreter.int_variables.Add(tokens[1], int.Parse(result));
                    }
                    else if (tokens[0] == "float")
                    {
                        List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(2));
                        string result = MathFunctions.EvaluateMathStack(stack);
                        Interpreter.float_variables.Add(tokens[1], float.Parse(result));
                    }
                    // Keys Section
                    else if (Interpreter.string_variables.ContainsKey(tokens[0]))
                    {
                        var ToSet = tokens.Skip(1);
                        string joined = string.Join(" ", ToSet);
                        Interpreter.string_variables[tokens[0]] = joined;
                    }
                    else if (Interpreter.int_variables.ContainsKey(tokens[0]))
                    {
                        if (tokens[1].Equals("++"))
                        {
                            Interpreter.int_variables[tokens[0]] = Interpreter.int_variables[tokens[0]] + 1;
                        }
                        else
                        {
                            List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                            string result = MathFunctions.EvaluateMathStack(stack);
                            Interpreter.int_variables[tokens[0]] = int.Parse(result);
                        }
                    }
                    else if (Interpreter.float_variables.ContainsKey(tokens[0]))
                    {
                        if (tokens[1].Equals("++"))
                        {
                            Interpreter.float_variables[tokens[0]] = Interpreter.float_variables[tokens[0]] + 1;
                        }
                        else
                        {
                            List<string> stack = MathFunctions.MakeMathStack(tokens.Skip(1));
                            string result = MathFunctions.EvaluateMathStack(stack);
                            Interpreter.float_variables[tokens[0]] = float.Parse(result);
                        }

                    }
                    else if (Interpreter.functions.ContainsKey(tokens[0]))
                    {
                        string arguments = string.Join("", tokens[1..^0]);
                        char[] parens = { '(', ')' };
                        arguments = arguments.Trim(parens);
                        string[] argument_list = arguments.Split(",");
                        int counter = 0;
                        foreach (string arg in Interpreter.function_args[tokens[0]])
                        {
                            float parsed;
                            float.TryParse(argument_list[counter], out parsed);
                            Interpreter.float_variables[arg] = parsed;
                            Console.WriteLine(string.Join(", ", Interpreter.float_variables[arg]));
                            Environment.Exit(0);
                            counter++;
                        }
                        Functions.CallFunction(Interpreter.functions[tokens[0]]);
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