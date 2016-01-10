using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Inspector.CodeMetrics.CSharp
{
    public class MagicString : CSharpAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).ToList().Select(ms =>
            {
                int score = CalculateScore(ms);
                return CreateScore<MagicStringScore>(ms, score);
            });
        }

        private int CalculateScore(MethodDeclarationSyntax method)
        {
            var nodes = method.DescendantNodes();

            var conditionWithStringLiteral = nodes.OfType<BinaryExpressionSyntax>().Where(bes => 
                CheckForStringInCondition(bes)
            );
            var caseWithStringLiteral = nodes.OfType<CaseSwitchLabelSyntax>().Where(cs => 
                cs.Value.IsKind(SyntaxKind.StringLiteralExpression)
            );

            return conditionWithStringLiteral.Count() + caseWithStringLiteral.Count();
        }

        private bool CheckForStringInCondition(BinaryExpressionSyntax expr)
        {
            if (expr==null)
                return false;

            if (expr.Left.IsKind(SyntaxKind.StringLiteralExpression) ||
                expr.Right.IsKind(SyntaxKind.StringLiteralExpression))
                return true;

            return false;
        }
    }
}
