﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;

namespace Inspector.CodeMetrics.CSharp
{
    public class DisabledCode : CSharpAnalyzer
    {
        public override IEnumerable<MethodScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).ToList().Select(m => {
                int score = CalculateScore(m);
                return CreateScore<DisabledCodeScore>(m, score);
            });          
        }

        private int CalculateScore(MethodDeclarationSyntax m)
        {
            var locator = new CommentLocator(m);
            var commentedCodeLines = locator.GetComments(DisabledCodeFilter).Count();

            return commentedCodeLines;
        }

        public static Predicate<string> DisabledCodeFilter
        {
            get
            {
                return comment => {
                    var stripped = comment.Trim(' ', '/');
                    var code = CSharpSyntaxTree.ParseText(stripped);
                    var root = code.GetRoot();

                    return root.HasLeadingTrivia;
                };
            }
        }
    }
}
