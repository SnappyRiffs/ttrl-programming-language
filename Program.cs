namespace TTRL
{
    static class App
    {
        public static void Main(string[] args)
        {
            Interpreter.Start(args[0]);
            Console.ReadLine();
        }
    }
}
