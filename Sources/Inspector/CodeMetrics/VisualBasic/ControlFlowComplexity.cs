using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;

namespace Inspector.CodeMetrics.VisualBasic
{
    /// <summary>
    /// Calculates the control-flow complexity for all methods in a given class.
    /// Score can be used in the IfSQ Level 2 Sp-3 indicator (Count > 10)
    /// </summary>
    public class ControlFlowComplexity : VisualBasicAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).Select(m => CreateScore<ControlFlowComplexityScore>(m, GetScore(m)));
        }

        private int GetScore(MethodBlockSyntax method)
        {
            var nodes = method.DescendantNodes();
            var ifs = nodes.OfType<BinaryExpressionSyntax>();
            var cases = nodes.OfType<CaseClauseSyntax>();
            var boolExpr = nodes.OfType<SingleLineIfStatementSyntax>().Where(c=>c.Condition is LiteralExpressionSyntax);
            var boolExprMulti = nodes.OfType<IfStatementSyntax>().Where(c => c.Condition is LiteralExpressionSyntax);
            var boolExprElse = nodes.OfType<ElseIfStatementSyntax>().Where(c => c.Condition is LiteralExpressionSyntax);

            return ifs.Count() + cases.Count() + boolExpr.Count() + boolExprMulti.Count() + boolExprElse.Count();
        }
    }
}
