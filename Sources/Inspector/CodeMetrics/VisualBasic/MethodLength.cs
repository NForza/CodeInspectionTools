using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Inspector.CodeMetrics.VisualBasic
{
    /// <summary>
    /// Calculates the method length for all methods in a given class.
    /// Method length includes all lines, including blanc and comment lines
    /// </summary>
    public class MethodLength : VisualBasicAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return node.DescendantNodes().OfType<MethodBlockSyntax>().Select(item =>
            {
                string fullMethod = string.Empty;
                fullMethod = item.ToFullString();

                var lines = fullMethod.Split('\n');
                
                var totalLength = lines.Length;
                var emptyLines = lines.Where(l => string.IsNullOrWhiteSpace(l)).Count();
                var linesStartingWithComment = lines.Where(l => l.Trim().StartsWith("'")).Count();
                return CreateScore<MethodLengthScore>(item, totalLength-emptyLines-linesStartingWithComment);
            });
        }
    }
}
