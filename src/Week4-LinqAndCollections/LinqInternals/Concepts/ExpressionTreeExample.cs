using System.Linq.Expressions;

namespace LinqInternals.Concepts;

/// <summary>
/// Demonstrates the difference between Func<T,R> delegates (executed immediately)
/// and Expression<Func<T,R>> (captured as an AST, translatable to SQL or other backends).
/// </summary>
public static class ExpressionTreeExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Expression Trees ---");

        // Func<T,R> — compiled to IL, executed immediately
        Func<int, bool> funcDelegate = x => x > 5;
        Console.WriteLine($"Func delegate: {funcDelegate(7)} (executes immediately)");

        // Expression<Func<T,R>> — captured as an AST, can be inspected/translated
        Expression<Func<int, bool>> expressionTree = x => x > 5;
        Console.WriteLine($"Expression tree: {expressionTree}");

        // Compile the expression tree to a delegate
        var compiledFunc = expressionTree.Compile();
        Console.WriteLine($"Compiled and executed: {compiledFunc(7)}");

        // Demonstrate expression tree inspection
        Console.WriteLine("\nInspecting expression tree structure:");
        InspectExpression(expressionTree.Body, 1);
    }

    private static void InspectExpression(Expression expr, int depth)
    {
        var indent = new string(' ', depth * 2);

        switch (expr.NodeType)
        {
            case ExpressionType.GreaterThan:
                var binExpr = (BinaryExpression)expr;
                Console.WriteLine($"{indent}Binary: >");
                InspectExpression(binExpr.Left, depth + 1);
                InspectExpression(binExpr.Right, depth + 1);
                break;

            case ExpressionType.Parameter:
                var paramExpr = (ParameterExpression)expr;
                Console.WriteLine($"{indent}Parameter: {paramExpr.Name}");
                break;

            case ExpressionType.Constant:
                var constExpr = (ConstantExpression)expr;
                Console.WriteLine($"{indent}Constant: {constExpr.Value}");
                break;

            default:
                Console.WriteLine($"{indent}{expr.NodeType}: {expr}");
                break;
        }
    }
}
