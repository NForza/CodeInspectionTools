using System;
using System.Collections.Generic;
using System.Linq;
using Inspector.CustomAnalyzers;
using Inspector.Infrastructure;
using System.IO;

namespace Inspector.Reports
{
    class ClassMembersReport
    {
        public void PrintMetrics(IEnumerable<ClassScore> classMemberScores, StreamWriter writer)
        {
            var scores = classMemberScores.ToList();
            writer.WriteLine($"Number of classes: {scores.Count}");

            writer.WriteLine($"  Highest number of members: {scores.Max(cms => cms.MemberCount)}");
            writer.WriteLine($"  Average number of members: {scores.Average(cms => cms.MemberCount):N2}");
            writer.WriteLine($"  Standard deviation of members: {scores.Select(cms => cms.MemberCount).StdDev():N2}");
            writer.WriteLine();

            PrintTop10LargestMethods(writer, scores);
            PrintTop10LargestMethodsPerProject(writer, scores);
        }

        private void PrintTop10LargestMethodsPerProject(StreamWriter writer, List<ClassScore> scores)
        {
            IEnumerable<IGrouping<string, ClassScore>> classScoresPerProject = scores.GroupBy(cls => cls.Project);

            foreach (var projectClassScores in classScoresPerProject)
            {
                writer.WriteLine($"Top 10 Largest classes for: {projectClassScores.Key}");
                var largestClassesForCurrentProject =
                    projectClassScores.OrderByDescending(cls => cls.TotalLineCount).Take(10);
                foreach(var largeMethodPerProject in largestClassesForCurrentProject)
                    writer.WriteLine($"  {largeMethodPerProject.Classname} - {largeMethodPerProject.TotalLineCount}");
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        private static void PrintTop10LargestMethods(StreamWriter writer, List<ClassScore> scores)
        {
            var top10LargestClasses = scores.OrderByDescending(cls => cls.TotalLineCount).Take(10);

            writer.WriteLine("Top 10 Largest classes:");
            foreach (var largeClass in top10LargestClasses)
            {
                writer.WriteLine($"  {largeClass.Project}.{largeClass.Classname} - {largeClass.TotalLineCount}");
            }
            writer.WriteLine();
        }
    }
}
