using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Inspector.CodeMetrics.CSharp
{
    public class MagicNumber : CSharpAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).ToList().Select(ms =>
            {
                int score = CalculateScore(ms);
                return CreateScore<MagicNumberScore>(ms, score);
            });
        }

        private int CalculateScore(MethodDeclarationSyntax method)
        {
            var nodes = method.DescendantNodes();
            var literals = nodes.OfType<LiteralExpressionSyntax>().Where(bes => 
                CheckForNumber(bes)
            );

            return literals.Count();
        }

        private bool CheckForNumber(LiteralExpressionSyntax expr)
        {
            if (expr == null)
                return false;

            if (expr.Parent is EqualsValueClauseSyntax)
                return false;

            if (expr.Parent is PrefixUnaryExpressionSyntax)
                return true;

            if (expr!=null && expr.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                double value = Double.Parse(expr.Token.ValueText);
                return (value != 0.0 && value != 1.0);
            }
            else
                return false;
        }
    }
}
