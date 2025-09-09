using System;
using TTRL; // Make sure this is your namespace containing Interpreter

namespace TTRL
{
    class Program
    {
        // Entry point of the application
        static void Main(string[] args)
        {
            // Start the interpreter when the App is created
            Interpreter.Start();

            // Keep the console open so the app doesn't immediately exit
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
