using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Inspector.CodeMetrics.VisualBasic
{
    public class MagicString : VisualBasicAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node, string project)
        {
            return GetMethods(node).ToList().Select(ms =>
            {
                int score = CalculateScore(ms);
                return CreateScore<MagicStringScore>(ms, score, project);
            });
        }

        private int CalculateScore(MethodBlockSyntax method)
        {
            var nodes = method.DescendantNodes();
            var ifs = nodes.OfType<BinaryExpressionSyntax>().Where(bes => 
                bes.Left.IsKind(SyntaxKind.StringLiteralExpression) || 
                bes.Right.IsKind(SyntaxKind.StringLiteralExpression));
            var cases = nodes.OfType<SimpleCaseClauseSyntax>().Where(ccs => ccs.Value.IsKind(SyntaxKind.StringLiteralExpression)) ;

            return ifs.Count() + cases.Count();
        }
    }
}
