namespace Inspector.CustomAnalyzers
{
    internal class ClassScore
    {
        private readonly string classname;
        private readonly string project;
        private readonly int memberCount;
        private readonly int totalLineCount;

        public ClassScore(string project, string classname, int memberCount, int totalLineCount)
        {
            this.project = project;
            this.memberCount = memberCount;
            this.totalLineCount = totalLineCount;
            this.classname = classname;
        }

        public string Project
        {
            get { return project; }
        }

        public string Classname
        {
            get { return classname; }
        }

        public int MemberCount
        {
            get { return memberCount; }
        }

        public int TotalLineCount
        {
            get { return totalLineCount; }
        }
    }
}