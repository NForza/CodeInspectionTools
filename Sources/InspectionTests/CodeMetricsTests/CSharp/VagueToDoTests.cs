﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.CSharp;
using System.Linq;
using FluentAssertions;

namespace InspectionTests.CodeMetricsTests.CSharp
{
    [TestClass]
    public class VagueToDoTests : CsharpMetricTest
    {
        [TestMethod]
        public void WithoutComments_ShouldReturn_Score0()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public TestClass() { }
                    
                    public bool TestMe(int i) {
                        return false;
                    }
                }
                ");

            var sut = new VagueToDo();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void WithOtherComments_ShouldReturn_Score0()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public TestClass() { }
                    
                    public bool TestMe(int i) {
                        // Test this
                        return false;
                    }
                }
                ");

            var sut = new VagueToDo();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void WitSimpleTodoComment_ShouldReturn_Score1()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public TestClass() { }
                    
                    public bool TestMe(int i) {
                        //TODO: test this            
                        return false;
                    }
                }
                ");

            
            var sut = new VagueToDo();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void With2TodoComments_ShouldReturn_Score2()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public TestClass() { }
                    
                    public bool TestMe(int i) {
                        //TODO: test this            
                        return false;
                        // todo something else
                    }
                }
                ");


            var sut = new VagueToDo();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void With5DifferentTodoComments_ShouldReturn_Score5()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public TestClass() { }
                    
                    public bool TestMe(int i) {
                        //TODO: test this            
                        // todo something else
                        //to do
                        //Find something else todo   
                        /* test TODO
                        */
    
                        // some other comment
                        return false;
                    }
                }
                ");
            
            var sut = new VagueToDo();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(5);
        }
    }
}
