﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Inspector.Analyzers.CSharp
{
    /// <summary>
    /// Calculates the control-flow complexity for all methods in a given class.
    /// Score can be used in the IfSQ Level 2 Sp-3 indicator (Count > 10)
    /// </summary>
    public class ControlFlowComplexity : CSharpAnalyzer
    {
        public override IEnumerable<MethodScore> GetMethodScores(SyntaxNode node)
        {
            return GetMethods(node).Select(m => CreateScore<ControlFlowComplexityScore>(m, GetScore(m)));
        }

        private int GetScore(MethodDeclarationSyntax method)
        {
            var nodes = method.DescendantNodes();
            var ifs = nodes.OfType<BinaryExpressionSyntax>();
            var cases = nodes.OfType<CaseSwitchLabelSyntax>();
            var boolExprMulti = nodes.OfType<IfStatementSyntax>().Where(c => c.Condition is LiteralExpressionSyntax);

            return ifs.Count() + cases.Count() + boolExprMulti.Count();
        }

    }
}
