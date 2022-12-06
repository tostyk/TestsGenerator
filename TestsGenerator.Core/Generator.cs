using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestsGenerator.Core
{
    public class Generator
    {
        public TestClassInfo[] GenerateTests(string source)
        {
            List<TestClassInfo> testClassesInfo = new();
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            var usings = new SyntaxList<UsingDirectiveSyntax>(
                root.DescendantNodes().OfType<UsingDirectiveSyntax>())
                .Add(UsingDirective(ParseName("System")))
                .Add(UsingDirective(ParseName("System.Collections.Generic")))
                .Add(UsingDirective(ParseName("System.Linq")))
                .Add(UsingDirective(ParseName("System.Text")))
                .Add(UsingDirective(ParseName("NUnit.Framework")))
                .Add(UsingDirective(ParseName("Moq")))
                .AddRange(root.DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                    .Select(u => UsingDirective(u.Name)));

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
            var attributes = SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestFixture")))));

            var fields = CreateTestFields(sourceClass);
            var testClass = ClassDeclaration(sourceClass.Identifier.Text + "Tests")
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithAttributeLists(attributes)
                .AddMembers(fields.ToArray())
                .AddMembers(testMethods.ToArray());

            _namespace = _namespace.AddMembers(testClass);
            return _namespace;
        }

        private List<MemberDeclarationSyntax> CreateTestMethods(ClassDeclarationSyntax sourceClass)
        {
            List<MemberDeclarationSyntax> methodList = new()
            {
                SetUpMethod(sourceClass)
            };
            Dictionary<string, int> overrideMethods = new();
            foreach (var member in sourceClass.Members)
            {
                if (member.IsKind(SyntaxKind.MethodDeclaration) && member.Modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    var sourceMethod = (MethodDeclarationSyntax)member;

                    var attributes = SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Test")))));
                    var modifiers = TokenList(Token(SyntaxKind.PublicKeyword));
                    var returnType = PredefinedType(Token(SyntaxKind.VoidKeyword));

                    BlockSyntax methodBody = Block(CreateTestMethodBody(sourceMethod, sourceClass.Identifier.Text));

                    var name = sourceMethod.Identifier.Text;
                    if (overrideMethods.ContainsKey(name))
                    {
                        overrideMethods[name]++;
                    }
                    else
                    {
                        overrideMethods.Add(name, 1);
                    }
                    var testMethod = MethodDeclaration(returnType, name + (overrideMethods.ContainsKey(name) && overrideMethods[name] > 1 ? overrideMethods[name] : "") + "Test")
                        .WithBody(methodBody)
                        .WithAttributeLists(attributes)
                        .WithModifiers(modifiers);
                    methodList.Add(testMethod);
                }
            }
            return methodList;
        }

        private List<MemberDeclarationSyntax> CreateTestFields(ClassDeclarationSyntax classDeclaration)
        {
            var className = classDeclaration.Identifier.Text;
            var camelCaseClasName = $"_{className.ToLower()[0]}{className.Remove(0, 1)}";
            var fields = new List<MemberDeclarationSyntax>();
            if (!classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                fields.Add(FieldDeclaration(
                    VariableDeclaration(
                        IdentifierName(className))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                Identifier(camelCaseClasName)))))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PrivateKeyword))));
                var constructor = classDeclaration
                    .DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .ToList()
                    .FirstOrDefault();

                if (constructor != null)
                    foreach (var parameter in constructor.ParameterList.Parameters)
                    {
                        if (parameter.Type.ToFullString().StartsWith('I'))
                        {
                            var interfaceName = parameter.Type.ToFullString();
                            var mockVar = $"_{interfaceName.ToLower()[1]}{interfaceName.Remove(0, 2)}";
                            fields.Add(
                                FieldDeclaration(
                                    VariableDeclaration(
                                        GenericName(
                                            Identifier("Mock"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList<TypeSyntax>(
                                                    IdentifierName(interfaceName)))))
                                    .WithVariables(
                                        SingletonSeparatedList(
                                            VariableDeclarator(
                                                Identifier(mockVar)))))
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PrivateKeyword))));
                        }
                    }
            }
            return fields;
        }

        private List<StatementSyntax> CreateTestMethodBody(MethodDeclarationSyntax method, string className)
        {
            var parameters = new SeparatedSyntaxList<ArgumentSyntax>();
            var methodBody = new List<StatementSyntax>();
            foreach (var parameter in method.ParameterList.Parameters)
            {
                parameters = parameters.Add(Argument(IdentifierName(parameter.Identifier.Text)));

                methodBody.Add(LocalDeclarationStatement(
                    VariableDeclaration(parameter.Type)
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                Identifier(parameter.Identifier.Text))
                            .WithInitializer(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.DefaultLiteralExpression,
                                        Token(SyntaxKind.DefaultKeyword))))))));
            }

            var camelCaseClassName = $"_{className.ToLower()[0]}{className.Remove(0, 1)}";
            var callClassName = method.Modifiers.Any(SyntaxKind.StaticKeyword) ? className : camelCaseClassName;

            if (method.ReturnType is PredefinedTypeSyntax typeSyntax
                && typeSyntax.Keyword.ValueText == Token(SyntaxKind.VoidKeyword).ValueText)
            {
                methodBody.Add(ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(callClassName),
                            IdentifierName(method.Identifier.Text)))
                    .WithArgumentList(ArgumentList(parameters))));
            }
            else
            {
                methodBody.Add(LocalDeclarationStatement(
                    VariableDeclaration(method.ReturnType)
                    .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(
                            Identifier("actual"))
                        .WithInitializer(
                            EqualsValueClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(callClassName),
                                        IdentifierName(method.Identifier.Text)))
                                .WithArgumentList(ArgumentList(parameters))))))));
                methodBody.Add(LocalDeclarationStatement(
                    VariableDeclaration(method.ReturnType)
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                Identifier("expected"))
                            .WithInitializer(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.DefaultLiteralExpression,
                                        Token(SyntaxKind.DefaultKeyword))))))));

                methodBody.Add(ExpressionStatement(
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Assert"),
                        IdentifierName("That")))
                .WithArgumentList(
                    ArgumentList(
                        SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]{
                                Argument(
                                    IdentifierName("actual")),
                                Token(SyntaxKind.CommaToken),
                                Argument(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("Is"),
                                            IdentifierName("EqualTo")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList(
                                                Argument(
                                                    IdentifierName("expected"))))))})))));
            }
            methodBody.Add(ExpressionStatement(
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Assert"),
                        IdentifierName("Fail")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal("autogenerated"))))))));
            return methodBody;
        }

        private MemberDeclarationSyntax SetUpMethod(ClassDeclarationSyntax classDeclaration)
        {
            var setup = new List<StatementSyntax>();
            var className = classDeclaration.Identifier.Text;
            var camelCaseClassName = $"_{className.ToLower()[0]}{className.Remove(0, 1)}";

            var interfaces = new List<string>();
            var constructor = classDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();

            var args = SeparatedList<ArgumentSyntax>();
            if (constructor != null)
                foreach (var parameter in constructor.ParameterList.Parameters)
                {
                    if (parameter.Type.ToFullString().StartsWith('I'))
                    {
                        var interfaceName = parameter.Type.ToFullString();
                        var mockVar = $"_{interfaceName.ToLower()[1]}{interfaceName.Remove(0, 2)}";
                        args = args.Add(Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(mockVar),
                                IdentifierName("Object"))));

                        setup.Add(ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(mockVar),
                                ObjectCreationExpression(
                                    GenericName(
                                        Identifier("Mock"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName(interfaceName)))))
                                .WithArgumentList(
                                    ArgumentList()))));
                    }
                    else
                    {
                        args = args.Add(Argument(IdentifierName(parameter.Identifier.Text)));

                        setup.Add(LocalDeclarationStatement(
                            VariableDeclaration(parameter.Type)
                            .WithVariables(
                                SingletonSeparatedList(
                                    VariableDeclarator(
                                        Identifier(parameter.Identifier.Text))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            LiteralExpression(
                                                SyntaxKind.DefaultLiteralExpression,
                                                Token(SyntaxKind.DefaultKeyword))))))));
                    }
                }
            if (!classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                setup.Add(
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(camelCaseClassName),
                            ObjectCreationExpression(
                                IdentifierName(className))
                            .WithArgumentList(
                                ArgumentList(args)))));

            MemberDeclarationSyntax setUpMethod = MethodDeclaration(
                PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("SetUp"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("SetUp"))))))
                .WithBody(Block(setup));
            return setUpMethod;
        }    
    }
}