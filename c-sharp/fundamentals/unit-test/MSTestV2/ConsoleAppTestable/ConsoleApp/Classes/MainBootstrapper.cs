using ConsoleApp.Interfaces;

namespace ConsoleApp.Classes
{
    public class MainBootstrapper : IMainBootstrapper
    {
        private readonly IConsoleWrapper _console;
        private const string Tab = "   ";

        public MainBootstrapper(IConsoleWrapper console)
        {
            _console = console;
        }
        public void Run(string[] args)
        {
            _console.OutputEncoding = System.Text.Encoding.UTF8;
            _console.WriteLine("ConsoleApp program started");
            _console.WriteLine("");

            string? input;

            do
            {
                // Do display section

                _console.WriteLine($"{Tab}\"\u21b2\" to clear the screen");
                _console.WriteLine($"{Tab}\"q\" to quit the program");
                _console.WriteLine("");

                _console.Write("Please enter a choice: ");

                input = _console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    // Do switch section
                }
                else
                {
                    _console.Clear();
                }
            } while (input != "q");
        }
    }
}