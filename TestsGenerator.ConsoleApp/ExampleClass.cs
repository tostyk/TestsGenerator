using System.Threading.Tasks.Dataflow;
using TestsGenerator.Core;

namespace TestsGenerator.ConsoleApp
{
    public class MyClass
    {
        public void FirstMethod()
        {
            Console.WriteLine("First method");
            TransformBlock<string, string> _readFile;
            TransformBlock<string, TestClassInfo[]> _processFile;
            ActionBlock<TestClassInfo[]> _writeFile;
        }

        public void SecondMethod()
        {
            Console.WriteLine("Second method");
        }

        public void ThirdMethod(int a)
        {
            Console.WriteLine("Third method (int)");
        }

        public void ThirdMethod(double a)
        {
            Console.WriteLine("Third method (double)");
        }
    }
}
