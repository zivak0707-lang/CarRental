using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CarRental.Tests;

/// <summary>
/// Перевіряє правила стандарту, для яких немає готового аналізатора.
/// Працює з текстом вихідних файлів, тому падає в тому самому конвеєрі, що й решта тестів.
/// </summary>
public partial class StandardComplianceTests
{
    private const int MaxLineLength = 100;
    private const int MaxFileLines = 500;
    private const int MaxMethodLines = 50;
    private const int MaxParameters = 6;
    private const int MaxNestingDepth = 4;
    private const int MaxPublicMembers = 20;

    private static readonly string[] _productionProjects = { "CarRental.Core", "CarRental.App" };
    private static readonly string _root = FindSolutionRoot();

    [Fact]
    public void SourceFiles_LineLength_WithinLimit()
    {
        // Act
        List<string> violations = ProductionFiles()
            .SelectMany(file => File.ReadAllLines(file)
                .Select((text, index) => (text, index))
                .Where(line => line.text.Length > MaxLineLength)
                .Select(line => $"{Relative(file)}:{line.index + 1} F-03"))
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void SourceFiles_LineCount_WithinLimit()
    {
        // Act
        List<string> violations = ProductionFiles()
            .Where(file => File.ReadAllLines(file).Length > MaxFileLines)
            .Select(file => $"{Relative(file)} S-13")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void Methods_LineCount_WithinLimit()
    {
        // Act
        List<string> violations = ProductionMethods()
            .Where(method => LineCountOf(method) > MaxMethodLines)
            .Select(method => Describe(method, "S-11"))
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void Methods_ParameterCount_WithinLimit()
    {
        // Act
        List<string> violations = ProductionMethods()
            .Where(method => method.ParameterList.Parameters.Count > MaxParameters)
            .Select(method => Describe(method, "S-12"))
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void Methods_NestingDepth_WithinLimit()
    {
        // Act
        List<string> violations = ProductionMethods()
            .Where(method => method.Body is not null && DepthOf(method.Body, 0) > MaxNestingDepth)
            .Select(method => Describe(method, "S-03"))
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void Types_PublicMemberCount_WithinLimit()
    {
        // Act
        List<string> violations = ProductionRoots()
            .SelectMany(root => root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            .Where(type => type.Members.Count(IsPublic) > MaxPublicMembers)
            .Select(type => $"{type.Identifier.Text} S-13")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void SourceFiles_ForbiddenConstructs_Absent()
    {
        // Act
        List<string> violations = ProductionRoots()
            .SelectMany(root => root.DescendantTrivia(descendIntoTrivia: true)
                .Where(trivia => trivia.IsKind(SyntaxKind.RegionDirectiveTrivia)
                    || trivia.IsKind(SyntaxKind.PragmaWarningDirectiveTrivia))
                .Select(trivia => trivia.GetLocation())
                .Concat(root.DescendantNodes().OfType<GotoStatementSyntax>()
                    .Select(statement => statement.GetLocation())))
            .Select(location => $"{location.GetLineSpan().Path} F-12/S-06/A-02")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void SuppressMessageAttributes_Justification_NotEmpty()
    {
        // Act
        List<string> violations = ProductionRoots()
            .SelectMany(root => root.DescendantNodes().OfType<AttributeSyntax>())
            .Where(attribute => attribute.Name.ToString().Contains("SuppressMessage"))
            .Where(attribute => !HasJustification(attribute))
            .Select(attribute => $"{attribute.GetLocation().GetLineSpan().Path} A-02")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void Comments_Todo_HaveAuthorAndIssue()
    {
        // Act
        List<string> violations = ProductionRoots()
            .SelectMany(root => root.DescendantTrivia()
                .Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                .Where(trivia => trivia.ToString().Contains("TODO", StringComparison.Ordinal))
                .Where(trivia => !TodoFormat().IsMatch(trivia.ToString()))
                .Select(trivia => $"{trivia.GetLocation().GetLineSpan().Path} C-04"))
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void BooleanExpressions_ComparisonWithLiteral_Absent()
    {
        // Act
        List<string> violations = ProductionRoots()
            .SelectMany(root => root.DescendantNodes().OfType<BinaryExpressionSyntax>())
            .Where(expression => expression.IsKind(SyntaxKind.EqualsExpression)
                || expression.IsKind(SyntaxKind.NotEqualsExpression))
            .Where(expression => IsBoolLiteral(expression.Left) || IsBoolLiteral(expression.Right))
            .Select(expression => $"{expression.GetLocation().GetLineSpan().Path} S-08")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void MemberAccess_ThisQualifier_Absent()
    {
        // Act
        List<string> violations = ProductionRoots()
            .SelectMany(root => root.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
            .Where(access => access.Expression is ThisExpressionSyntax)
            .Select(access => $"{access.GetLocation().GetLineSpan().Path} N-04")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void Core_MoneyAndRates_NoFloatingPointTypes()
    {
        // Act
        List<string> violations = Roots(FilesOf("CarRental.Core"))
            .SelectMany(root => root.DescendantNodes().OfType<PredefinedTypeSyntax>())
            .Where(type => type.Keyword.IsKind(SyntaxKind.DoubleKeyword)
                || type.Keyword.IsKind(SyntaxKind.FloatKeyword))
            .Select(type => $"{type.GetLocation().GetLineSpan().Path} D-10")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void Identifiers_ForbiddenNames_Absent()
    {
        // Act
        List<string> violations = ProductionRoots()
            .SelectMany(root => root.DescendantNodes().SelectMany(DeclaredNames))
            .Where(name => ForbiddenName().IsMatch(name))
            .Select(name => $"{name} N-18")
            .Distinct()
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [Fact]
    public void TestMethods_Name_HasThreeParts()
    {
        // Act
        List<string> violations = Roots(FilesOf("CarRental.Tests"))
            .SelectMany(root => root.DescendantNodes().OfType<MethodDeclarationSyntax>())
            .Where(IsTestMethod)
            .Where(method => method.Identifier.Text.Split('_').Length != 3)
            .Select(method => $"{method.Identifier.Text} N-15")
            .ToList();

        // Assert
        Assert.Empty(violations);
    }

    [GeneratedRegex(@"TODO\(@[\w-]+, #\d+\):")]
    private static partial Regex TodoFormat();

    [GeneratedRegex(
        @"^(data|info|temp|tmp|obj|item\d)$|(Manager|Helper|Helpers|Utils|Util)$"
        + @"|^(str|int|dbl|bln)[A-Z]|\w+And[A-Z]\w*$|[A-Za-z]\d+$"
        + @"|^(Zamovlennya|Kilkist|Suma|Tsina|Bronyuvannya)")]
    private static partial Regex ForbiddenName();

    private static string FindSolutionRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && directory.GetFiles("CarRental.sln").Length == 0)
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Не знайдено корінь рішення.");
    }

    private static IEnumerable<string> FilesOf(string project) =>
        Directory.EnumerateFiles(Path.Combine(_root, project), "*.cs", SearchOption.AllDirectories)
            .Where(file => !IsBuildOutput(file));

    private static bool IsBuildOutput(string file)
    {
        string[] segments = file.Split(Path.DirectorySeparatorChar);
        return segments.Contains("bin") || segments.Contains("obj");
    }

    private static IEnumerable<string> ProductionFiles() =>
        _productionProjects.SelectMany(FilesOf);

    private static IEnumerable<SyntaxNode> Roots(IEnumerable<string> files) =>
        files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file).GetRoot());

    private static IEnumerable<SyntaxNode> ProductionRoots() => Roots(ProductionFiles());

    private static IEnumerable<BaseMethodDeclarationSyntax> ProductionMethods() =>
        ProductionRoots().SelectMany(root =>
            root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>());

    private static string Relative(string file) => Path.GetRelativePath(_root, file);

    private static int LineCountOf(SyntaxNode node)
    {
        FileLinePositionSpan span = node.GetLocation().GetLineSpan();
        return span.EndLinePosition.Line - span.StartLinePosition.Line + 1;
    }

    private static string Describe(BaseMethodDeclarationSyntax method, string rule)
    {
        FileLinePositionSpan span = method.GetLocation().GetLineSpan();
        return $"{Relative(span.Path)}:{span.StartLinePosition.Line + 1} {rule}";
    }

    private static int DepthOf(SyntaxNode node, int depth)
    {
        int result = depth;
        foreach (SyntaxNode child in node.ChildNodes())
        {
            result = Math.Max(result, DepthOf(child, depth + (AddsNesting(child) ? 1 : 0)));
        }

        return result;
    }

    private static bool AddsNesting(SyntaxNode node)
    {
        bool isElseIf = node is IfStatementSyntax && node.Parent is ElseClauseSyntax;
        bool isBlockStatement = node is IfStatementSyntax or ForStatementSyntax
            or ForEachStatementSyntax or WhileStatementSyntax or DoStatementSyntax
            or SwitchStatementSyntax or TryStatementSyntax or LockStatementSyntax;
        return isBlockStatement && !isElseIf;
    }

    private static bool IsPublic(MemberDeclarationSyntax member) =>
        member.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));

    private static bool HasJustification(AttributeSyntax attribute) =>
        attribute.ArgumentList?.Arguments.Any(argument =>
            argument.NameEquals?.Name.Identifier.Text == "Justification"
            && argument.Expression.ToString().Length > "\"\"".Length) ?? false;

    private static bool IsBoolLiteral(ExpressionSyntax expression) =>
        expression.IsKind(SyntaxKind.TrueLiteralExpression)
        || expression.IsKind(SyntaxKind.FalseLiteralExpression);

    private static bool IsTestMethod(MethodDeclarationSyntax method) =>
        method.AttributeLists.SelectMany(list => list.Attributes)
            .Any(attribute => attribute.Name.ToString() is "Fact" or "Theory");

    private static IEnumerable<string> DeclaredNames(SyntaxNode node) => node switch
    {
        BaseTypeDeclarationSyntax type => new[] { type.Identifier.Text },
        MethodDeclarationSyntax method => new[] { method.Identifier.Text },
        PropertyDeclarationSyntax property => new[] { property.Identifier.Text },
        ParameterSyntax parameter => new[] { parameter.Identifier.Text },
        VariableDeclaratorSyntax variable => new[] { variable.Identifier.Text },
        _ => Array.Empty<string>(),
    };
}
