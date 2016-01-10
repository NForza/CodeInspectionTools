using Inspector.Components;
using Inspector.IfSQ;
using System;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using System.Collections.Generic;

namespace Inspector
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution solution = new Solution(args.First());

            SourceFileAnalyzer sourceAnalyzer = GetCodeAnalyzers();

            IEnumerable<CodeMetricScore> codeMetrics = sourceAnalyzer.CalculateCodeMetrics(solution.SourceFiles);
            IEnumerable<Level2Score> ifsqScores = solution.SourceFiles.Select(sf => new Level2Score(sf, codeMetrics));

            PrintDetails(ifsqScores);
            PrintTotal(solution.SourceFiles, ifsqScores);
        }

        private static void PrintDetails(IEnumerable<Level2Score> ifsqScores)
        {
            Console.WriteLine(Level2Score.HeaderString());
            ifsqScores.OrderByDescending(s => s.Rating).ToList().ForEach(s => Console.WriteLine(s));
        }

        private static void PrintTotal(IEnumerable<SourceFile> sourceFilesWithCodeMetrics, IEnumerable<Level2Score> ifsqScores)
        {
            int totalLines = sourceFilesWithCodeMetrics.Sum(sf => sf.LinesOfCode);
            var total = new Level2Score(totalLines, ifsqScores);
            Console.WriteLine(total);
        }

        private static SourceFileAnalyzer GetCodeAnalyzers()
        {
            var sourceAnalyzer = new SourceFileAnalyzer();
            //SP-1
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.MethodLength());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.MethodLength());

            //SP-2
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.NestingLevel());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.NestingLevel());

            //SP-3
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.ControlFlowComplexity());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.ControlFlowComplexity());

            //WIP-1
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.VagueToDo());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.VagueToDo());

            //WIP-2
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.DisabledCode());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.DisabledCode());

            //WIP-3
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.EmptyStatementBlock());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.EmptyStatementBlock());

            //SPM-1
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.MagicNumber());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.MagicNumber());

            //SPM-2
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.CSharp.MagicString());
            sourceAnalyzer.AddAnalyzer(new CodeMetrics.VisualBasic.MagicString());

            return sourceAnalyzer;
        }
    }
}
