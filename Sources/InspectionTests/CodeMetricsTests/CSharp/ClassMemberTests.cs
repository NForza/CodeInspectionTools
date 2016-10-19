using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.CSharp;
using FluentAssertions;
using System.Linq;
using InspectionTests.Builders;

namespace InspectionTests.CodeMetricsTests.CSharp
{
    [TestClass]
    public class ClassMemberTests
    {
        [TestMethod]
        public void EmptyClass_ShouldHave_Score0()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                public class TestClass {
                    
                }
                ");

            var sut = new ClassComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void ClassWithConstructor_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                public class TestClass {
                   public TestClass() { } 
                }
                ");

            var sut = new ClassComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.First().Score.Should().Be(1);
        }


        [TestMethod]
        public void ClassWithDestructor_ShouldHave_Score1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                public class TestClass {
                   public ~TestClass() { } 
                }
                ");

            var sut = new ClassComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.First().Score.Should().Be(1);
        }



        [TestMethod]
        public void ClassWith7FieldsMethodsAndProperties_ShouldHave_ScoreOf7()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                public class TestClass {
                   public TestClass() { } 
                   public ~TestClass() { } 

                   int Test { get; set; }
                   string Test2 { get; set; }
                   int Test3;
                   int Test4;
                   void Test5() { }
                }
                ");

            var sut = new ClassComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.First().Score.Should().Be(7);
            results.First().Project.Should().Be("TestProjectName");
        }
    }
}
