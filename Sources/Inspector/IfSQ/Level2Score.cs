using Inspector.CodeMetrics.Scores;
using System.Collections.Generic;
using System.Linq;
using Inspector.Components;

namespace Inspector.IfSQ
{
    public class Level2Score
    {
        private const int Sp1ScoreTreshold = 200;
        private const int Sp2ScoreTreshold = 4;
        private const int Sp3ScoreTreshold = 10;

        public Level2Score(SourceFile sourceFile, IEnumerable<CodeMetricScore> metrics)
        {
            Project = sourceFile.Project.Name;
            File = sourceFile.FileName;
            Loc = sourceFile.LinesOfCode;

            Spm1 = metrics.OfType<MagicNumberScore>().Sum(ms => ms.Score);
            Spm2 = metrics.OfType<MagicStringScore>().Sum(ms => ms.Score);
            Wip1 = metrics.OfType<VagueToDoScore>().Sum(ms => ms.Score);
            Wip2 = metrics.OfType<DisabledScore>().Sum(ms => ms.Score);
            Wip3 = metrics.OfType<EmptyStatementBlockScore>().Sum(ms => ms.Score);
            Sp1 = metrics.OfType<MethodLengthScore>().Where(ms => ms.Score > Sp1ScoreTreshold).Sum(ms => ms.Score - Sp1ScoreTreshold);
            Sp2 = metrics.OfType<NestingLevelScore>().Where(ms => ms.Score > Sp2ScoreTreshold).Sum(ms => ms.LineCountPerLevel[Sp2ScoreTreshold + 1]);
            Sp3 = metrics.OfType<ControlFlowComplexityScore>().Where(ms => ms.Score > Sp3ScoreTreshold).Sum(ms => ms.Score - Sp3ScoreTreshold);
        }

        public Level2Score(int totalLines, IEnumerable<Level2Score> scores)
        {
            Project = "Total";
            File = "All";
            Loc = totalLines;

            Wip1 = scores.Sum(s => s.Wip1);
            Wip2 = scores.Sum(s => s.Wip2);
            Wip3 = scores.Sum(s => s.Wip3);
            Sp1 = scores.Sum(s => s.Sp1);
            Sp2 = scores.Sum(s => s.Sp2);
            Sp3 = scores.Sum(s => s.Sp3);
            Spm1 = scores.Sum(s => s.Spm1);
            Spm2 = scores.Sum(s => s.Spm2);
            Spm3 = scores.Sum(s => s.Spm3);
        }

        public string Solution { get; private set; }
        public string Project { get; private set; }
        public string File { get; private set; }
        public string Language { get; private set; }
        public int Wip1 { get; private set; }
        public int Wip2 { get; private set; }
        public int Wip3 { get; private set; }
        public int Sp1 { get; private set; }
        public int Sp2 { get; private set; }
        public int Sp3 { get; private set; }
        public int Spm1 { get; private set; }
        public int Spm2 { get; private set; }
        public int Spm3 { get; private set; }
        public int Total { get { return Wip1 + Wip2 + Wip3 + Sp1 + Sp2 + Sp3 + Spm1 + Spm2 + Spm3; } }
        public int Loc { get; private set; }

        public int DefectsPerKloc
        {
            get
            {
                var kloc = Loc / 1000.0;
                return (int)(Total / kloc);
            }
        }

        public string Rating
        {
            get
            {
                return RatingFromDefectsPerKloc(DefectsPerKloc);
            }
        }

        /// <summary>
        /// Get IfSQ level 2 rating based on defects per 1000 lines of code (kloc)
        /// </summary>
        /// <param name="defectsKloc">defects per 1000 lines of code</param>
        /// <returns></returns>
        // Static method to allow calculations outside this class to use the rating 
        public static string RatingFromDefectsPerKloc(int defectsKloc)
        {
            if (defectsKloc <= 1)
                return "AAA";
            if (defectsKloc <= 2)
                return "AA";
            if (defectsKloc <= 5)
                return "A";
            if (defectsKloc <= 10)
                return "B";
            if (defectsKloc <= 20)
                return "C";
            if (defectsKloc <= 50)
                return "D";
            if (defectsKloc <= 100)
                return "E";
            if (defectsKloc <= 200)
                return "F";
            if (defectsKloc <= 500)
                return "FF";
            return "FFF";
        }

        public static string HeaderString()
        {
            return $"\"Project\",\"File\",\"Loc\",\"DefectsFound\",\"DefectsPerKloc\",\"Rating\",\"Wip1\",\"Wip2\",\"Wip3\",\"Sp1\",\"Sp2\",\"Sp3\",\"Spm1\",\"Spm2\",\"Spm3\"";
        }
        public override string ToString()
        {
            return $"\"{Project}\",\"{File}\",{Loc},{Total},{DefectsPerKloc},\"{Rating}\",{Wip1},{Wip2},{Wip3},{Sp1},{Sp2},{Sp3},{Spm1},{Spm2},{Spm3}";
        }
    }
}
