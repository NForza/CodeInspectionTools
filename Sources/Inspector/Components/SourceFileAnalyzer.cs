using Inspector.CodeMetrics;
using Inspector.CodeMetrics.Scores;
using System.Collections.Generic;
using System.Linq;

namespace Inspector.Components
{
    public class SourceFileAnalyzer
    {
        private readonly ICollection<ICodeMetricAnalyzer> analyzers = new List<ICodeMetricAnalyzer>();

        public void AddAnalyzer(ICodeMetricAnalyzer analyzer)
        {
            analyzers.Add(analyzer);
        }

        public IEnumerable<CodeMetricScore> CalculateCodeMetrics(IEnumerable<SourceFile> sourceFiles)
        {
            return sourceFiles.SelectMany(sf => sf.CalculateMetricsWith(analyzers));
        }                                                 
    }
}
