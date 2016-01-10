using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inspector.CodeMetrics.Scores;

namespace Inspector.CodeMetrics.CSharp
{
    public class MethodLength : CSharpAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).Select(item =>
            {
                var body = item.Body;

                string fullMethod = string.Empty;
                using (var writer = new StringWriter())
                {
                    body.WriteTo(writer);
                    fullMethod = writer.ToString();
                }
                var lines = fullMethod.Split('\n');

                var totalLength = lines.Length - 1;
                return CreateScore<MethodLengthScore>(item, totalLength);
            });
        }
    }
}
