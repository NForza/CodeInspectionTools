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

        protected T CreateScore<T>(BaseMethodDeclarationSyntax m, int score) where T : MethodScore, new()
        {
            T result = new T();
            result.ClassName = GetClassName(m);
            result.Method = GetMethodName(m);
            result.Score = score;

            return result;
        }
                
        protected string GetClassName(BaseMethodDeclarationSyntax m)
        {
            var classBlock = m.Parent as ClassDeclarationSyntax;
            if (classBlock != null)
                return classBlock.Identifier.ValueText;

            return "n/a";
        }

        private string GetMethodName(BaseMethodDeclarationSyntax m)
        {
            if (m is DestructorDeclarationSyntax)
            {
                var destructor = m as DestructorDeclarationSyntax;
                return $"~{ destructor.Identifier } {destructor.ParameterList}";
            }
            if (m is ConstructorDeclarationSyntax)
            {
                var constructor = m as ConstructorDeclarationSyntax;
                return $"{constructor.Identifier} {constructor.ParameterList}";
            }
            if (m is ConversionOperatorDeclarationSyntax)
            {
                var conversion = m as ConversionOperatorDeclarationSyntax;
                return $"{conversion.OperatorKeyword}{conversion.ParameterList}";
            }
            if ( m is OperatorDeclarationSyntax)
            {
                var operatodecl = m as OperatorDeclarationSyntax;
                return $"{operatodecl.OperatorKeyword}{operatodecl.ParameterList}";
            }
            var method = m as MethodDeclarationSyntax;
            return $"{ method.ReturnType } { method.Identifier } {method.ParameterList}";
        }

        protected IEnumerable<BaseMethodDeclarationSyntax> GetMethods(SyntaxNode node)
        {
            return node.DescendantNodes().OfType<BaseMethodDeclarationSyntax>();
        }

        protected IEnumerable<ClassDeclarationSyntax> GetClasses(SyntaxNode node)
        {
            return node.DescendantNodes().OfType<ClassDeclarationSyntax>();
        }
    }
}