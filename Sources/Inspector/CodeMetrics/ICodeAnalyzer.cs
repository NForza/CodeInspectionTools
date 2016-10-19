using System.Collections.Generic;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;

namespace Inspector.CodeMetrics
{
    public interface ICodeMetricAnalyzer
    {
        IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node, string project);
    }
}