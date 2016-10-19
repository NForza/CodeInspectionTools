using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.VisualBasic;
using FluentAssertions;
using System.Linq;
using InspectionTests.Builders;
using Inspector.CodeMetrics.Scores;

namespace InspectionTests.CodeMetricsTests.VisualBasic
{
    [TestClass]
    public class MagicNumberTests 
    {
        [TestMethod]
        public void EmptyMethod_ShouldHave_Score0()
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

            var sut = new MagicNumber();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void TestWith0Or1_ShouldHave_Score0()
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

            var sut = new MagicNumber();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void TestNumber_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean                       
                        return i > 10
                    End Function
                End Class
                ");

            var sut = new MagicNumber();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void VariableDeclarationWithNumber_ShouldNot_RaiseScore()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean   
                        Dim j as Integer  = 200                    
                        return i > 10
                    End Function
                End Class
                ");

            var sut = new MagicNumber();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SelectCase_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean   
                        Dim j as Integer  = 200                    
                        Select Case j
                            Case 10
                                Return False
                        End Select
                        Return i > 0
                    End Function
                End Class
                ");

            var sut = new MagicNumber();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }
    }
}
