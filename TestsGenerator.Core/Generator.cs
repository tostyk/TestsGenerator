using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestsGenerator.Core
{
    public class Generator
    {
        public Task Generate(string source)
        {
            return new Task(() => GenerateTests(source));
        }
        private TestClassInfo[] GenerateTests(string source)
        { 
            List<TestClassInfo> testClassesInfo = new List<TestClassInfo>();
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            SyntaxList<UsingDirectiveSyntax> usings = new SyntaxList<UsingDirectiveSyntax>
            {
                UsingDirective(ParseName("System")),
                UsingDirective(ParseName("System.Collections.Generic")),
                UsingDirective(ParseName("System.Linq")),
                UsingDirective(ParseName("NUnit.Framework")),
                UsingDirective(ParseName("System.Text"))
            };
            usings.AddRange(root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().Select(n => UsingDirective(n.Name)));
            usings.AddRange(root.DescendantNodes().OfType<UsingDirectiveSyntax>());

            var sourceClasses = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)));
            foreach (var sourceClass in sourceClasses)
            {
                var testClass = CreateTestClass(sourceClass);
                var testCode = CompilationUnit().WithUsings(usings).AddMembers(testClass);
                testClassesInfo.Add(
                    new TestClassInfo(sourceClass.Identifier.Text + "Tests", 
                    testCode.NormalizeWhitespace().ToFullString()));
            }
            return testClassesInfo.ToArray();
        }

        private MemberDeclarationSyntax CreateTestClass(ClassDeclarationSyntax sourceClass)
        {
            var _namespace = NamespaceDeclaration(IdentifierName("Tests"));

            var testMethods = CreateTestMethods(sourceClass);

            var testClass = ClassDeclaration(sourceClass.Identifier.Text + "Tests")
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .AddMembers(testMethods);

            _namespace = _namespace.AddMembers(testClass);
            return _namespace;
        }

        private MemberDeclarationSyntax[] CreateTestMethods(ClassDeclarationSyntax sourceClass)
        {
            List<MemberDeclarationSyntax> methodList = new List<MemberDeclarationSyntax>();
            foreach (var member in sourceClass.Members)
            {
                if (member.IsKind(SyntaxKind.MethodDeclaration))
                {
                    var sourceMethod = (MethodDeclarationSyntax)member;

                    var attributes = AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Test"))));
                    var modifiers = TokenList(Token(SyntaxKind.PublicKeyword));
                    var returnType = PredefinedType(Token(SyntaxKind.VoidKeyword));

                    var methodBody = Block();

                    var testMethod = MethodDeclaration(returnType, sourceMethod.Identifier.Text + "Test")
                        .AddBodyStatements(methodBody)
                        .AddAttributeLists(attributes)
                        .WithModifiers(modifiers);
                    methodList.Add(testMethod);
                }
            }
            return methodList.ToArray();
        }
    }
}