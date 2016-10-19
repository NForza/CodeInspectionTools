using System;
using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Inspector.CodeMetrics.CSharp
{
    public class DisabledCode : CSharpAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node, string project)
        {
            return GetMethods(node).ToList().Select(m => {
                int score = CalculateScore(m);
                return CreateScore<DisabledScore>(m, score, project);
            });          
        }

        private int CalculateScore(BaseMethodDeclarationSyntax m)
        {
            var locator = new CommentLocator(m);
            var commentedCodeLines = locator.GetComments(DisabledCodeFilter).Count();

            return commentedCodeLines;
        }

        public static Predicate<string> DisabledCodeFilter
        {
            get
            {
                return comment => {
                    var stripped = comment.Trim(' ', '/');
                    var code = CSharpSyntaxTree.ParseText(stripped);
                    var root = code.GetRoot();

                    return root.HasLeadingTrivia;
                };
            }
        }
    }
}
