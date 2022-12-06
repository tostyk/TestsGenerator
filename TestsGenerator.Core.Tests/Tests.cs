using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace TestsGenerator.Core.Tests
{
    public class GeneratorTests
    {
        Generator _generator;
        TestClassInfo[] _testClasses;
        [OneTimeSetUp]
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
        public void FilesCountTest()
        {
            Assert.That(_testClasses, Is.Not.Null);
            Assert.That(_testClasses.Length, Is.EqualTo(2));
        }

        [Test]
        public void FilesNamesTest()
        {
            Assert.That(_testClasses[0].Name, Is.EqualTo("ExampleClassTests"));
            Assert.That(_testClasses[1].Name, Is.EqualTo("StaticClassTests"));
        }

        [Test]
        public void UsingsTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            Assert.That(root.Members.Count, Is.EqualTo(1));
            var usings = root.Usings.ToList();
            Assert.That(usings.Count, Is.EqualTo(7));
            Assert.That(usings.Find(u => u.ToFullString().Trim() == "using System;"), Is.Not.Null);
            Assert.That(usings.Find(u => u.ToFullString().Trim() == "using System.Collections.Generic;"), Is.Not.Null);
            Assert.That(usings.Find(u => u.ToFullString().Trim() == "using System.Linq;"), Is.Not.Null);
            Assert.That(usings.Find(u => u.ToFullString().Trim() == "using System.Text;"), Is.Not.Null);
            Assert.That(usings.Find(u => u.ToFullString().Trim() == "using NUnit.Framework;"), Is.Not.Null);
            Assert.That(usings.Find(u => u.ToFullString().Trim() == "using Moq;"), Is.Not.Null);
            Assert.That(usings.Find(u => u.ToFullString().Trim() == "using TestsGenerator.Example;"), Is.Not.Null);
        }

        [Test]
        public void NamespacesCountTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            Assert.That(root.Members.Count, Is.EqualTo(1));
            var ns = root.Members[0] as NamespaceDeclarationSyntax;
            Assert.That(ns, Is.Not.Null);
        }

        [Test]
        public void ClassesCountTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            var ns = root.Members[0] as NamespaceDeclarationSyntax;
            var cl = ns.Members[0] as ClassDeclarationSyntax;

            Assert.That(cl, Is.Not.Null);
        }

        [Test]
        public void ClassesNamesTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var ns = root.Members[0] as NamespaceDeclarationSyntax;
            var cl = ns.Members[0] as ClassDeclarationSyntax;

            SyntaxTree tree2 = CSharpSyntaxTree.ParseText(_testClasses[1].Content);
            CompilationUnitSyntax root2 = tree2.GetCompilationUnitRoot();
            var ns2 = root2.Members[0] as NamespaceDeclarationSyntax;
            var cl2 = ns2.Members[0] as ClassDeclarationSyntax;

            Assert.That(cl, Is.Not.Null);
            Assert.That(cl2, Is.Not.Null);
            Assert.That(cl.Identifier.Text, Is.EqualTo("ExampleClassTests"));
            Assert.That(cl2.Identifier.Text, Is.EqualTo("StaticClassTests"));
        }

        [Test]
        public void ClassesAttributesTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var ns = root.Members[0] as NamespaceDeclarationSyntax;
            var cl = ns.Members[0] as ClassDeclarationSyntax;

            SyntaxTree tree2 = CSharpSyntaxTree.ParseText(_testClasses[1].Content);
            CompilationUnitSyntax root2 = tree2.GetCompilationUnitRoot();
            var ns2 = root2.Members[0] as NamespaceDeclarationSyntax;
            var cl2 = ns2.Members[0] as ClassDeclarationSyntax;

            Assert.That(cl, Is.Not.Null);
            Assert.That(cl.AttributeLists.Count, Is.EqualTo(1));
            Assert.That(cl.AttributeLists[0].Attributes.Count, Is.EqualTo(1));
            Assert.That(cl.AttributeLists[0].Attributes[0].Name.ToFullString(), Is.EqualTo("TestFixture"));

            Assert.That(cl2, Is.Not.Null);
            Assert.That(cl2.AttributeLists.Count, Is.EqualTo(1));
            Assert.That(cl2.AttributeLists[0].Attributes.Count, Is.EqualTo(1));
            Assert.That(cl2.AttributeLists[0].Attributes[0].Name.ToFullString(), Is.EqualTo("TestFixture"));
        }

        [Test]
        public void ClassFieldsTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var cl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();

            var fields = cl.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            Assert.That(fields.Count(), Is.EqualTo(2));
            Assert.That(fields[0].Declaration.Variables[0].Identifier.Text, Is.EqualTo("_exampleClass"));
            Assert.That(fields[0].Declaration.Type.ToFullString().Replace(" ", ""), Is.EqualTo("ExampleClass"));
            Assert.That(fields[0].Modifiers[0].Kind(), Is.EqualTo(SyntaxKind.PrivateKeyword));
            Assert.That(fields[1].Declaration.Variables[0].Identifier.Text, Is.EqualTo("_testInterface"));
            Assert.That(fields[1].Declaration.Type.ToFullString().Replace(" ", ""), Is.EqualTo("Mock<ITestInterface>"));
            Assert.That(fields[1].Modifiers[0].Kind(), Is.EqualTo(SyntaxKind.PrivateKeyword));
        }

        [Test]
        public void StaticClassFieldsTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[1].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var staticClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();

            Assert.That(staticClass.DescendantNodes().OfType<FieldDeclarationSyntax>().Count, Is.EqualTo(0));
        }

        [Test]
        public void SetUpMethodTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var cl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();

            var setUpMethod = cl.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.Text == "SetUp").Single();
            Assert.That(setUpMethod.AttributeLists[0].Attributes[0].ToFullString(), Is.EqualTo("SetUp"));
            Assert.That(setUpMethod.Body.Statements.Count(), Is.EqualTo(3));
        }

        [Test]
        public void SetUpMethodForStaticClassTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[1].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var staticClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();

            var setUpMethod = staticClass.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.Text == "SetUp").Single();
            Assert.That(setUpMethod.AttributeLists[0].Attributes[0].ToFullString(), Is.EqualTo("SetUp"));
            Assert.That(setUpMethod.Body.Statements.Count(), Is.EqualTo(0));
        }

        [Test]
        public void MethodsTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var ns = root.Members[0] as NamespaceDeclarationSyntax;
            var cl = ns.Members[0] as ClassDeclarationSyntax;
            var methods = cl.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.Text != "SetUp");
            foreach(var method in methods)
            {
                Assert.That(method.Body.Statements.Where(s => s.ToFullString() == "Assert.Fail(\"autogenerated\")"), Is.Not.Null);
            }
        }

        [Test]
        public void StaticMethodsTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_testClasses[0].Content);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var cl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            var staticMethodTest = cl
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToList()
                .Find(t => t.Identifier.Text == "StaticMethodTest");
            var staticMethod = staticMethodTest.Body.Statements
                .ToList()
                .Where(e => e.ToFullString() == "int actual = ExampleClass.StaticMethod();");
            Assert.That(staticMethod, Is.Not.Null);
        }
    }
}
