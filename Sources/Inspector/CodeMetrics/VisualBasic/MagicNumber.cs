﻿using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspector.CodeMetrics.VisualBasic
{
    public class MagicNumber : VisualBasicAnalyzer
    {
        public override IEnumerable<MethodScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).ToList().Select(ms =>
            {
                int score = CalculateScore(ms);
                return CreateScore<MagicNumberScore>(ms, score);
            });
        }

        private int CalculateScore(MethodBlockSyntax method)
        {
            var nodes = method.DescendantNodes();
            var literals = nodes.OfType<LiteralExpressionSyntax>().Where(bes =>
                CheckForNumber(bes)
            );

            return literals.Count();
        }

        private bool CheckForNumber(LiteralExpressionSyntax expr)
        {
            if (expr == null)
                return false;

            if (expr.Parent is EqualsValueSyntax)
                return false;

            if (expr.Parent is UnaryExpressionSyntax)
                return true;

            if (expr != null && expr.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                double value = Double.Parse(expr.Token.ValueText);
                return (value != 0.0 && value != 1.0);
            }
            else
                return false;
        }
    }
}