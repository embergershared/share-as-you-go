using System;

namespace ConsoleApp.Interfaces
{
    public interface IConsoleWrapper
    {
        // Methods
        void WriteLine(string value);
        void Write(string value);
        ConsoleKeyInfo ReadKey();
        string? ReadLine();
        void Clear();

        // Properties
        System.Text.Encoding OutputEncoding { get; set; }
    }
}
