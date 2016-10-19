using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Inspector.CodeMetrics.VisualBasic
{
    public class EmptyStatementBlock : VisualBasicAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node, string project)
        {
            return GetMethods(node).Select(m =>
            {
                int score = CalculateScore(m);
                return CreateScore<EmptyStatementBlockScore>(m, score, project);
            });
        }

        private int CalculateScore(MethodBlockSyntax m)
        {
            var nodes = m.DescendantNodes();

            var emptyCatchClauses = nodes
                .OfType<CatchBlockSyntax>()
                .Where(s => s.Statements.Count == 0 && !HasCommentWhyEmpty(s));

            var emptyFinallyClauses = nodes
                .OfType<FinallyBlockSyntax>()
                .Where(s => s.Statements.Count == 0 && !HasCommentWhyEmpty(s));

            var emptyIf = nodes
                .OfType<MultiLineIfBlockSyntax>()
                .Where(s => s.Statements.Count == 0 && !HasCommentWhyEmpty(s));

            var emptyElses = nodes
                .OfType<ElseBlockSyntax>()
                .Where(s => s.Statements.Count == 0 && !HasCommentWhyEmpty(s));

            var emptyElseIfs = nodes
                .OfType<ElseIfBlockSyntax>()
                .Where(s => s.Statements.Count == 0 && !HasCommentWhyEmpty(s));

            return emptyCatchClauses.Count() + emptyFinallyClauses.Count() + emptyIf.Count() + emptyElses.Count() + emptyElseIfs.Count();
        }

        private static bool HasCommentWhyEmpty(SyntaxNode s)
        {
            var locator = new CommentLocator(s.Parent);
            int commentCount = locator.GetComments().Count();
            return commentCount > 0; //empty catch without comments why.
        }
    }
}
