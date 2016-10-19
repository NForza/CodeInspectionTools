using System.Linq;
using FluentAssertions;
using InspectionTests.Builders;
using Inspector.CodeMetrics.CSharp;
using Inspector.CodeMetrics.Scores;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InspectionTests.CodeMetricsTests.CSharp
{
    [TestClass]
    public class DisabledCodeTests
    {
        [TestMethod]
        public void EmptyMethod_ShouldHave_Score0()
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

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void NormalComment_ShouldHave_Score0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        // no code in here 
                        return false;
                    }
                }
                ");

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void IfInComment_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                     public bool TestMe(int i) {
                        // no code in here 
                        // if (i==0) return true;
                        return false;
                    }
                }
                ");

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void IfMultiLineInComment_ShouldHave_Score2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        // no code in here 
                        // if (i==0) 
                        //     return true;
                        return false;
                    }
                }
                ");

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void IfWithStatementBlockInComment_ShouldHave_Score4()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        // no code in here 
                        // if (i==0) 
                        // {
                        //     return true;
                        // }
                        return false;
                    }
                }
                ");

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(4);
        }
    }
}
