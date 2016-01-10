using System.Collections.Generic;
using System.Linq;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Inspector.CodeMetrics.VisualBasic
{
    public class NestingLevel : VisualBasicAnalyzer
    {
        public override IEnumerable<CodeMetricScore> GetMetrics(SyntaxNode node)
        {
            return GetMethods(node).ToList().Select(m =>
            {
                int baseLine = m.Ancestors().Count();
                var visitor = new Walker(baseLine);
                visitor.Visit(m);

                int maxLevel = visitor.MaxLevel;
                var score = CreateScore<NestingLevelScore>(m, maxLevel);
                score.LineCountPerLevel = new Dictionary<int, int>(visitor.LineCountPerLevel);
                return score;
            }
            );

        }
        private class Walker : VisualBasicSyntaxWalker
        {
            private readonly Dictionary<int, int> lineCountPerLevel = new Dictionary<int, int>();

            public Dictionary<int, int> LineCountPerLevel
            {
                get
                {
                    return lineCountPerLevel;
                }
            }

            public override void VisitWhileBlock(WhileBlockSyntax node)
            {
                IncreaseLevel(node);
                base.VisitWhileBlock(node);
                DecreaseLevel();
            }

            public override void VisitDoLoopBlock(DoLoopBlockSyntax node)
            {
                IncreaseLevel(node);
                base.VisitDoLoopBlock(node);
                DecreaseLevel();
            }

            public override void VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
            {
                IncreaseLevel(node);
                base.VisitSingleLineIfStatement(node);
                DecreaseLevel();
            }

            public override void VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
            {
                IncreaseLevel(node);
                base.VisitMultiLineIfBlock(node);
                DecreaseLevel();
            }

            private void DecreaseLevel()
            {
                currentLevel--;
            }

            private void IncreaseLevel(SyntaxNode node)
            {
                currentLevel++;

                if (currentLevel > MaxLevel)
                {
                    MaxLevel = currentLevel;
                    StoreLineCountForThisLevel(node);
                }
            }
            private void StoreLineCountForThisLevel(SyntaxNode node)
            {
                var code = node.ToFullString();
                int lineCount = code.Split('\n').Count();

                if (lineCountPerLevel.ContainsKey(currentLevel))
                    lineCountPerLevel[currentLevel] += lineCount;
                else
                    lineCountPerLevel.Add(currentLevel, lineCount);
            }

            public override void VisitCaseBlock(CaseBlockSyntax node)
            {
                IncreaseLevel(node);
                base.VisitCaseBlock(node);
                DecreaseLevel();
            }

            private int currentLevel = 0;
            private int baseLine;

            public Walker(int baseLine)
            {
                this.baseLine = baseLine;
            }

            public int MaxLevel { get; private set; } = 0;
        }
    }
}
