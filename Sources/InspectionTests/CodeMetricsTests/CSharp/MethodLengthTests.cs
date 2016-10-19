using System.Linq;
using FluentAssertions;
using InspectionTests.Builders;
using Inspector.CodeMetrics.CSharp;
using Inspector.CodeMetrics.Scores;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InspectionTests.CodeMetricsTests.CSharp
{
    [TestClass]
    public class MethodLengthTests
    {
        [TestMethod]
        public void EmptyMethod_ShouldReturn_MethodWithScoreOf_0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) 
                    {                        
                    }
                }
                ");

            var sut = new MethodLength();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void EmptyConstructor_ShouldReturn_MethodWithScoreOf_0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public TestClass(int i) 
                    {                        
                    }
                }
                ");

            var sut = new MethodLength();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("TestClass (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void EmptyDestructor_ShouldReturn_MethodWithScoreOf_0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public ~TestClass() 
                    {                        
                    }
                }
                ");

            var sut = new MethodLength();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("~TestClass ()");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void MethodWith23Lines_ShouldReturn_MethodWithScoreOf_23()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) 
                    {                        
                        if (i == 10) 
                            return true;
                        if (i == 10) 
                            return true;
                        
                        if (i == 10) 
                            return true;


                        if (i == 10) 
                            return true;
                        if (i == 10) 
                            return true;

                        if (i == 10) 
                            return true;
                        if (i == 10) 
                            return true;
                        if (i == 10) 
                            return true;
                        if (i == 10) 
                            return true;                       
                        return true;
                     }
                }
                ");

            var sut = new MethodLength();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(23);
        }     
    }
}
