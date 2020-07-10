using System;

namespace GprTest
{
    public sealed class Dummy
    {
        public string SayHello()
        {
            return $"Hello on ${DateTime.Now}!";
        }
    }
}
