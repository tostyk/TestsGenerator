namespace TestsGenerator.Example
{
    public interface ITestInterface { }
    public class ExampleClass 
    {
        public ExampleClass(ITestInterface testInterface, int number) { }

        public void VoidMethod() { }

        public int IntParameterlessMethod()
        {
            return 0;
        }

        public string StringMethodWithParameters(int number, string s, ExampleClass exampleClass)
        {
            return "";
        }

        public int DuplicateMethod(int number)
        {
            return 0;
        }

        public int DuplicateMethod(double number)
        {
            return 0;
        }
        public static int StaticMethod()
        {
            return 0;
        }
    }

    public static class StaticClass
    {
        public static int Method(int number)
        {
            return 0;
        }
    }
}