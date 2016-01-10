using System;
using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Inspector.CodeMetrics.VisualBasic
{
    public class DisabledCode : VisualBasicAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).ToList().Select(m => {
                int score = CalculateScore(m);
                return CreateScore<DisabledScore>(m, score);
            });          
        }

        private int CalculateScore(MethodBlockSyntax m)
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
                    var stripped = comment.Trim(' ', '\'');
                    var options = new VisualBasicParseOptions(kind: SourceCodeKind.Script);
                    var code = VisualBasicSyntaxTree.ParseText(stripped, options: options);
                    var root = code.GetRoot();
                    var l = root.GetLeadingTrivia();
                    return l.Count()==0;
                };
            }
        }
    }
}
