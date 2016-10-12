using System.IO;
using Inspector.Components;
using System.Linq;
using Inspector.CustomAnalyzers;
using Inspector.Reports;
using System;

namespace Inspector
{
    class Program
    {
        private const string CodeInspectionOutputDirectory = "CodeInspection";
        private const string ClassStatisticsFileName = "classStatistics.txt";
        private const string IfsqLevel2FileName = "ifsqlevel2.csv";

        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} solution.sln");
                Console.ReadLine();
                return;
            }

            string path = Path.GetFullPath(args.First());
            Solution solution = new Solution(path);

            string outputDir = GetOutputDirectory(path);

            PrintIfSQLevel2Scores(solution, outputDir);
            PrintClassMemberScores(solution, outputDir);
        }

        private static string GetOutputDirectory(string path)
        {
            var outputDir = Path.Combine(Path.GetDirectoryName(path), CodeInspectionOutputDirectory);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            return outputDir;
        }

        private static void PrintClassMemberScores(Solution solution, string outputDir)
        {
            var classStatisticsAnalyzer = new ClassStatisticsAnalyzer();

            classStatisticsAnalyzer.Analyze(solution.SourceFiles);


            var filename = Path.Combine(outputDir, ClassStatisticsFileName);
            if (File.Exists(filename))
                File.Delete(filename);

            var report = new ClassMembersReport();
            using (var writer = new StreamWriter(filename))
            {
                report.PrintMetrics(classStatisticsAnalyzer.Scores, writer);
            }
        }

        private static void PrintIfSQLevel2Scores(Solution solution, string outputDir)
        {
            var ifsqAnalyzer = new IfSQLevel2CSharpAnalyzer();

            ifsqAnalyzer.Analyze(solution.SourceFiles);

            var filename = Path.Combine(outputDir, IfsqLevel2FileName);
            if (File.Exists(filename))
                File.Delete(filename);

            var report = new IfSQLevel2Report();
            using (var writer = new StreamWriter(filename))
            {
                report.PrintMetrics(ifsqAnalyzer.Scores, writer);
            }
        }
    }
}
