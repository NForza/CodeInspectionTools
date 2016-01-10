namespace Inspector.CodeMetrics.Scores
{
    public class CodeMetricScore
    {
        public string ClassName { get; internal set; }
        public int Score { get; internal set; }

        public override string ToString()
        {
            return $"\t{ClassName}: {Score}";
        }
    }
}