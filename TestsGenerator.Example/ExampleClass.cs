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
    }

    public class ExampleClass2
    {
        public ExampleClass2(int number) { }
        public int VoidMethodWithParameters(int number)
        {
            return 0;
        }
    }
}