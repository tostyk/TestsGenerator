using TestsGenerator.Core;

namespace TestsGenerator.ConsoleApp
{
    public class Program
    {
        const string programText =
@"using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
        public int Method() { }
    }
}";
        public static async Task Main(string[] args)
        {

        }
    }
}
