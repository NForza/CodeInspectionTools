using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using Inspector.Infrastructure;

namespace Inspector.CodeMetrics.CSharp
{
    public class MethodLength : CSharpAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node, string project)
        {
            return GetMethods(node).Select(item =>
            {
                var totalLength = item.GetLineCount();
                return CreateScore<MethodLengthScore>(item, totalLength, project);
            });
        }
    }
}
