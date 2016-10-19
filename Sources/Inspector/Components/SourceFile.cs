using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Inspector.Components
{
    public class SourceFile
    {
        private const string csharpSourceFileExtension = ".cs";
        private const string visualBasicSourceFileExtension = ".vb";

        public SourceFile(Project project, string srcFile, string code)
        {
            Project = project;
            project.AddSourceFile(this);

            FileName = srcFile;
            Code = code;
            LinesOfCode = Code.Split('\n').Count();
        }

        public int LinesOfCode
        {
            get; internal set;
        }

        public Project Project { get; private set; }
        public string FileName { get; private set; }
        public string Language { get; private set; }

        public string Code { get; private set; }

        private SyntaxNode GetSyntaxNodeFromCode()
        {
            SyntaxNode sourceFileRootNode = null;
            if (FileName.EndsWith(csharpSourceFileExtension))
            {
                sourceFileRootNode = CSharpSyntaxTree.ParseText(Code).GetRoot();
            }
            else if (FileName.EndsWith(visualBasicSourceFileExtension))
            {
                sourceFileRootNode = VisualBasicSyntaxTree.ParseText(Code).GetRoot();
            };
            return sourceFileRootNode;
        }

        public IEnumerable<CodeMetricScore> CalculateMetricsWith(ICollection<ICodeMetricAnalyzer> analyzers)
        {
            var syntax = GetSyntaxNodeFromCode();
            Language = syntax.Language;
            return analyzers.SelectMany(a => a.GetMetrics(syntax, Project.Name)).ToList();
        }
    }
}