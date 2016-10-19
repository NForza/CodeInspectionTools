using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.CSharp;
using FluentAssertions;
using System.Linq;
using InspectionTests.Builders;
using Inspector.CodeMetrics.Scores;

namespace InspectionTests.CodeMetricsTests.CSharp
{
    [TestClass]
    public class MagicStringTests
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

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void StringDeclaration_ShouldHave_Score0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        string test = ""hello"";
                        return false;
                    }
                }
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void IfWithStringLiteral_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        string test = ""hello"";
                        if (test == ""hello"")
                            return true;

                        return false;
                    }
                }
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void ConditionWithStringLiteral_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        string test = ""hello"";
                        return test == ""hello"";
                    }
                }
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SwitchWithStringLiteral_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        string test = ""hello"";
                        switch(test) {
                            case ""hello"":
                                return false;
                                break;
                        }
            
                        return true;
                    }
                }
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void WhileWithStringLiteral_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        string test = ""hello"";

                        while (test == ""hello"") {
                        }
            
                        return true;
                    }
                }
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }
    }
}
