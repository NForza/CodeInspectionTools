using FluentAssertions;
using Inspector.CodeMetrics.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using InspectionTests.Builders;
using Inspector.CodeMetrics.Scores;

namespace InspectionTests.CodeMetricsTests.VisualBasic
{
    [TestClass]
    public class ControlFlowComplexityTests
    {
        [TestMethod]
        public void EmptyMethod_ShouldReturn_MethodWithScoreOf_0()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean                       
                        return false
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void SimpleIf_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        if (i = 10) then
                            return true
                        end if

                        return false
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }


        [TestMethod]
        public void IfElseIf_ShouldReturn_MethodWithScoreOf_2()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        if (i = 10) then
                            return true
                        elseif i>10 then
                            return true
                        else
                            return false
                        end if

                        return false
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(2);
        }


        [TestMethod]
        public void IfElseIfAndReturnWithExpression_ShouldReturn_MethodWithScoreOf_3()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        if (i = 10) then
                            return true
                        elseif i>10 then
                            return true
                        end if

                        return i=0
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void IfWithAnd_ShouldReturn_MethodWithScoreOf_3()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        Dim check = False
                        if i = 10 And check = True then
                            return true
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void SingleLineIf_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        if i = 10 then return true
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SelectCaseWith1Option_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        Select Case i
                            Case 0
                                return true                            
                        End Select

                        return false
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SelectCaseWith2Options_ShouldReturn_MethodWithScoreOf_2()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        Select Case i
                            Case 0
                                return true       
                            Case 1
                                return true                     
                        End Select

                        return false
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void SelectCaseWith2OptionsAndEmbeddedIf_ShouldReturn_MethodWithScoreOf_3()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        Dim z = true
                        Select Case i
                            Case 0
                                if z = True then 
                                    return true       
                                end if
                            Case 1
                                return true                     
                        End Select

                        return false
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void ReturnWithExpression_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        return i > 0
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void IfWithBoolean_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        If True then return false else return true
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void IfWithBooleanMultiLine_ShouldReturn_MethodWithScoreOf_1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        If True then 
                            return false 
                        End If
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void ElseIfWithBoolean_ShouldReturn_MethodWithScoreOf_2()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        If True then 
                            return false 
                        ElseIf False then
                            return true
                        End If
                    End Function
                End Class
                ");

            var sut = new ControlFlowComplexity();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(2);
        }

    }
}
