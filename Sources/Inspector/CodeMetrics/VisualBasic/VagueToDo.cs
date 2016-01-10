using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Text.RegularExpressions;
using Inspector.CodeMetrics.Scores;

namespace Inspector.CodeMetrics.VisualBasic
{
    public class VagueToDo : VisualBasicAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node)
                .Select(m => CreateScore<VagueToDoScore>(m, CalculateScore(m)));          
        }

        private int CalculateScore(MethodBlockSyntax m)
        {
            var locator = new CommentLocator(m);
            return locator.GetComments(ToDoCommentFilter).Count();            
        }

        public static Predicate<string> ToDoCommentFilter
        {
            get
            {
                //Check for default todo / hack syntax as picked up by VisualStudio
                return commentContent =>
                    Regex.IsMatch(commentContent, @"('\s)*TO\s*DO\s*[:\[\r\n]*", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(commentContent, @"('\s)*HACK\s*[:\[\r\n]*", RegexOptions.IgnoreCase); 
            }
        }
    }
}
