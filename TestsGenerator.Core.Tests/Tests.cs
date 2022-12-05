using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace TestsGenerator.Core.Tests
{
    public class Tests
    {
        Generator _generator;
        TestClassInfo[] _testClasses;
        [SetUp]
        public void SetUp()
        {
            _generator = new Generator();
            string fileText = "";
            using (StreamReader reader = new StreamReader("ExampleClass.cs"))
            {
                fileText = reader.ReadToEnd();
            }
            _testClasses = _generator.GenerateTests(fileText);
        }

        [Test]
        public void ClassesCountTest()
        {
            Assert.That(_testClasses, Is.Not.Null);
            Assert.That(_testClasses.Length, Is.EqualTo(2));
        }

        [Test]
        public void TestClassesNamesTest()
        {
            Assert.That(_testClasses[0].Name, Is.EqualTo("ExampleClassTests"));
            Assert.That(_testClasses[1].Name, Is.EqualTo("ExampleClass2Tests"));
        }

        [Test]
        public void TestNamespacesCountTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            Assert.That(root.Members.Count, Is.EqualTo(1));
            var ns = root.Members[0] as NamespaceDeclarationSyntax;
            Assert.That(ns, Is.Not.Null);
        }

        [Test]
        public void TestClassesCountTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            var ns = root.Members[0] as NamespaceDeclarationSyntax;
            var cl = ns.Members[0] as ClassDeclarationSyntax;

            Assert.That(cl, Is.Not.Null);
        }
    }
}
