using ConsoleApp.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ConsoleApp.Classes
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public void Write(string value)
        {
            Console.Write(value);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public string? ReadLine()
        {
            return Console.ReadLine();
        }

        [ExcludeFromCodeCoverage]
        public void Clear()
        {
            Console.Clear();
        }

        public Encoding OutputEncoding
        {
            get => Console.OutputEncoding;
            set => Console.OutputEncoding = value;
        }
    }
}