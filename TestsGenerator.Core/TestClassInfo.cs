namespace TestsGenerator.Core
{
    public class TestClassInfo
    {
        public string Name { get; }
        public string Content { get; }
        public TestClassInfo(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
