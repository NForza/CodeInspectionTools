using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Inspector.CodeMetrics.CSharp
{
    public class EmptyStatementBlock : CSharpAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node, string project)
        {
            return GetMethods(node).Select(m =>
            {
                int score = CalculateScore(m);
                return CreateScore<EmptyStatementBlockScore>(m, score, project);
            });
        }

        private int CalculateScore(BaseMethodDeclarationSyntax m)
        {
            var nodes = m.DescendantNodes();

            var emptyBlocks = nodes.OfType<BlockSyntax>().Where(s =>
            {
                var parent = s.Parent;
                if (!parent.IsKind(SyntaxKind.ConstructorDeclaration)) // constructor could be empty
                {
                    if (s.Statements.Count == 0)
                        return !HasCommentWhyEmpty(s);
                }

                return false;
            });


            return emptyBlocks.Count();
        }

        private static bool HasCommentWhyEmpty(SyntaxNode s)
        {
            var locator = new CommentLocator(s.Parent);
            int commentCount = locator.GetComments().Count();
            return commentCount > 0; //empty catch without comments why.
        }
    }
}
