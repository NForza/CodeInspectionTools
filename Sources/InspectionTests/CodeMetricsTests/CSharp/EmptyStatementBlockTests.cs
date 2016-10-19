using System.Linq;
using FluentAssertions;
using InspectionTests.Builders;
using Inspector.CodeMetrics.CSharp;
using Inspector.CodeMetrics.Scores;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InspectionTests.CodeMetricsTests.CSharp
{
    [TestClass]
    public class EmptyStatementBlockTests
    {
        [TestMethod]
        public void SimpleReturn_ShouldHave_Score0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        return false;
                    }
                }
                ");

            var sut = new EmptyStatementBlock();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void EmptyCatchBlock_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    
                    public bool TestMe(int i) {
                        try {
                            i=i*2;
                        }
                        catch {}
                        return false;
                    }
                }
                ");

            var sut = new EmptyStatementBlock();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void EmptyCatchBlockWithComment_ShouldHave_Score0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        try {
                            i=i*2;
                        }
                        catch {
                            //ignore error because...
                        }
                        return false;
                    }
                }
                ");

            var sut = new EmptyStatementBlock();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }
    }
}
