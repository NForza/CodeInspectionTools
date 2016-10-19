using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;

namespace Inspector.CodeMetrics.CSharp
{
    public class NestingLevel : CSharpAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node, string project)
        {
            return GetMethods(node).ToList().Select(m =>
            {
                var visitor = new Walker();
                visitor.Visit(m);

                int maxLevel = visitor.MaxLevel;
                var score= CreateScore<NestingLevelScore>(m, maxLevel, project);
                score.LineCountPerLevel = new Dictionary<int, int>(visitor.LineCountPerLevel);
                return score;
            }
            );

        }

        private class Walker : CSharpSyntaxWalker
        {
            private readonly Dictionary<int, int> lineCountPerLevel = new Dictionary<int, int>();
            private int currentLevel = 0;
            public int MaxLevel { get; private set; } = 0;

            public Dictionary<int,int> LineCountPerLevel
            {
                get
                {
                    return lineCountPerLevel;
                }
            }

            public override void VisitIfStatement(IfStatementSyntax node)
            {
                IncreaseLevel(node.Statement.ToString());
                base.VisitIfStatement(node);

                DecreaseLevel();
            }

            private void DecreaseLevel()
            {
                currentLevel--;
            }

            private void IncreaseLevel(string code)
            {
                currentLevel++;

                if (currentLevel > MaxLevel)
                {
                    MaxLevel = currentLevel;
                    StoreLineCountForThisLevel(code);
                }
            }

            private void StoreLineCountForThisLevel(string code)
            {
                int lineCount = code.Split('\n').Count();

                if (lineCountPerLevel.ContainsKey(currentLevel))
                    lineCountPerLevel[currentLevel] += lineCount;
                else
                    lineCountPerLevel.Add(currentLevel, lineCount);
            }

            public override void VisitSwitchStatement(SwitchStatementSyntax node)
            {
                IncreaseLevel(node.ToString());
                base.VisitSwitchStatement(node);
                DecreaseLevel();
            }

            public override void VisitWhileStatement(WhileStatementSyntax node)
            {
                IncreaseLevel(node.ToString());
                base.VisitWhileStatement(node);
                DecreaseLevel();
            }

            public override void VisitDoStatement(DoStatementSyntax node)
            {
                IncreaseLevel(node.ToString());
                base.VisitDoStatement(node);
                DecreaseLevel();
            }

            public override void VisitForStatement(ForStatementSyntax node)
            {
                IncreaseLevel(node.Statement.ToString());
                base.VisitForStatement(node);
                DecreaseLevel();
            }
        }
    }
}
