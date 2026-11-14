using System.Text.RegularExpressions;
using CarRental.Tests.Doubles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CarRental.Tests.Architecture;

/// <summary>Цільові показники СР3 як тест: перевищення будь-якого зупиняє конвеєр.</summary>
public partial class ComplexityLimitsTests
{
    private const int MaxLogicalLines = 30;
    private const int MaxComplexity = 8;
    private const int MaxNesting = 3;
    private const int MaxParameters = 4;

    // Сигнатуру з п'ятьма параметрами задає інтерфейс ILogger бібліотеки .NET
    private static readonly string[] _frameworkSignatures = ["FileLogger.Log"];

    [Fact]
    public void Methods_AllSourceFiles_WithinLimits()
    {
        List<string> violations = Methods()
            .Select(m => (Name: Describe(m), Lines: LogicalLines(m), Cc: Complexity(m),
                Depth: 1 + Nesting(m, 0), Params: m.ParameterList.Parameters.Count))
            .Where(m => !_frameworkSignatures.Contains(m.Name))
            .Where(m => m.Lines > MaxLogicalLines || m.Cc > MaxComplexity
                || m.Depth > MaxNesting || m.Params > MaxParameters)
            .Select(m => $"{m.Name}: {m.Lines} р., CC {m.Cc}, рівнів {m.Depth}, {m.Params} пар.")
            .ToList();

        Assert.Empty(violations);
    }

    private static IEnumerable<BaseMethodDeclarationSyntax> Methods() =>
        Directory.GetFiles(SourceRoot(), "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Split(Path.DirectorySeparatorChar).Contains("obj"))
            .SelectMany(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f)).GetRoot()
                .DescendantNodes().OfType<BaseMethodDeclarationSyntax>())
            .Where(m => m.Body is not null || m.ExpressionBody is not null);

    private static string SourceRoot() => Path.Combine(RepoPaths.Root(), "src");

    private static string Describe(BaseMethodDeclarationSyntax m)
    {
        string type = m.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>()?.Identifier.Text ?? "?";
        return m is MethodDeclarationSyntax method
            ? $"{type}.{method.Identifier.Text}"
            : $"{type}.ctor";
    }

    private static int LogicalLines(BaseMethodDeclarationSyntax m) =>
        ((SyntaxNode?)m.Body ?? m.ExpressionBody!).ToString().Split('\n')
            .Select(l => l.Trim())
            .Count(l => l.Length > 0 && !l.StartsWith("//", StringComparison.Ordinal)
                && !BraceOnly().IsMatch(l));

    // М1: 1 + if, ?:, цикли, case, плече switch, catch, &&, ||, ??
    private static int Complexity(SyntaxNode node) => 1 + node.DescendantNodes().Count(n =>
        n is IfStatementSyntax or ConditionalExpressionSyntax or ForStatementSyntax
            or ForEachStatementSyntax or WhileStatementSyntax or DoStatementSyntax
            or CaseSwitchLabelSyntax or CatchClauseSyntax
        || (n is SwitchExpressionArmSyntax arm && arm.Pattern is not DiscardPatternSyntax)
        || n.IsKind(SyntaxKind.LogicalAndExpression) || n.IsKind(SyntaxKind.LogicalOrExpression)
        || n.IsKind(SyntaxKind.CoalesceExpression));

    private static int Nesting(SyntaxNode node, int level) => node.ChildNodes()
        .Select(c => Nesting(c, IsBlock(c) ? level + 1 : level))
        .DefaultIfEmpty(level)
        .Max();

    private static bool IsBlock(SyntaxNode n) =>
        (n is IfStatementSyntax && n.Parent is not ElseClauseSyntax)
        || n is ForStatementSyntax or ForEachStatementSyntax or WhileStatementSyntax
            or DoStatementSyntax or SwitchStatementSyntax or TryStatementSyntax
            or UsingStatementSyntax or LockStatementSyntax;

    [GeneratedRegex(@"^[{}();,]+$")]
    private static partial Regex BraceOnly();
}
