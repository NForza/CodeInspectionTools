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
    public class ControlFlowComplexityTests
    {
        [TestMethod]
        public void EmptyMethod_ShouldReturn_MethodWithScoreOf_0()
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

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void SimpleIf_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                   
                    public bool TestMe(int i) {
                        if (i == 10) 
                            return true;

                        return false;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }


        [TestMethod]
        public void IfElseIf_ShouldReturn_MethodWithScoreOf_2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        if (i == 10) 
                            return true;
                        else if (i>10) 
                            return true;
                        else
                            return false;
                        
                        return false;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }


        [TestMethod]
        public void IfElseIfAndReturnWithExpression_ShouldReturn_MethodWithScoreOf_3()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        if (i == 10) 
                            return true;
                        else if (i>10) 
                            return true;                        

                        return i==0;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void IfWithAnd_ShouldReturn_MethodWithScoreOf_3()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        var check = false;
                        if ((i == 10) && (check==true))
                            return true;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void SingleLineIf_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                   
                    public bool TestMe(int i) {
                        if (i == 10) return true;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SelectCaseWith1Option_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        switch (i)
                        {
                            case 10:
                                break;
                        }
                        return false;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SelectCaseWith2Options_ShouldReturn_MethodWithScoreOf_2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        switch (i) {
                            case 0:
                                return true;
                                break;
                            case 1:
                                return true;
                                break;
                        }

                        return false;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void SelectCaseWith2OptionsAndEmbeddedIf_ShouldReturn_MethodWithScoreOf_3()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        var z = true;
                        switch (i) {
                            case 0:
                                if (z == true)  
                                    return true;       
                                break;
                            case 1:
                                return true;
                                break;
                        }

                        return false;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void ReturnWithExpression_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        return i > 0;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void IfWithBoolean_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        if (true) return false; else return true;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void IfWithBooleanMultiLine_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                     public bool TestMe(int i) {
                        if (true)
                            return false;
                        
                        return true;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void ElseIfWithBoolean_ShouldReturn_MethodWithScoreOf_2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        if (true)
                            return false;
                        else if (false)
                            return true;
                    }
                }
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }
    }
}
