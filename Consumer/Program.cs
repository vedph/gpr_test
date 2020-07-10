using System;
using GprTest;

namespace Consumer
{
    internal static class Program
    {
        private static void Main()
        {
            Dummy dummy = new Dummy();
            Console.WriteLine(dummy.SayHello());
        }
    }
}
