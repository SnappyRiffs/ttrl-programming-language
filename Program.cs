namespace TTRL
{
    class App
    {
        public App()
        {
            // Start the interpreter when the App is created
            Interpreter.Start();

            // Read line from console to keep the app running
            Console.ReadLine();
        }
    }
}
