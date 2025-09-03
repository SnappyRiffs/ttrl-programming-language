using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTL
{
    static class Interpreter
    {
        public static void Start()
        {
            Dictionary<string, string> string_variables = new Dictionary<string, string>();
            Dictionary<string, int> int_variables = new Dictionary<string, int>();
            Dictionary<string, float> float_variables = new Dictionary<string, float>();
            string[] Files;
            Console.WriteLine("started interpreter");
            try
            {
                string FilePath = File.ReadAllText("path.cfg");

                try
                {
                    Files = Directory.GetFiles(FilePath, "*.ttrl");

                    foreach (string file in Files)
                    {
                        string[] lines = File.ReadAllLines(file);
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
                                    string joined = String.Join (" ", ToJoin);
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

                                    Stack<float> stack = new Stack<float>();
                                    foreach (string token in tokens)
                                    {
                                        if (token.All(Char.IsDigit))
                                        {
                                            stack.Push(float.Parse(token));
                                        }
                                    }
                                    float sum = 0;
                                    foreach (float number in stack)
                                    {
                                        sum += number;
                                    }
                                    Console.WriteLine("The sum is: " + sum);
                                }/*
                                var startsWithWhiteSpace = char.IsWhiteSpace(tokens[0], 0); // 0 = first character
                                if (startsWithWhiteSpace)
                                {
                                    Console.WriteLine("error : white space");
                                }*/
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
