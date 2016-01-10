using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;

namespace Inspector.CodeMetrics.CSharp
{
    public abstract class CSharpAnalyzer : ICodeMetricAnalyzer
    {
        public abstract IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node);

        protected T CreateScore<T>(MethodDeclarationSyntax m, int score) where T : MethodScore, new()
        {
            T result = new T();
            result.ClassName = GetClassName(m);
            result.Method = GetMethodName(m);
            result.Score = score;

            return result;
        }
                
        private string GetClassName(MethodDeclarationSyntax m)
        {
            var classBlock = m.Parent as ClassDeclarationSyntax;
            if (classBlock != null)
                return classBlock.Identifier.ValueText;

            return "n/a";
        }

        private string GetMethodName(MethodDeclarationSyntax m)
        {
            return $"{ m.ReturnType } { m.Identifier } {m.ParameterList.ToString()}";
        }

        protected IEnumerable<MethodDeclarationSyntax> GetMethods(SyntaxNode node)
        {
            return node.DescendantNodes().OfType<MethodDeclarationSyntax>();
        }
    }
}