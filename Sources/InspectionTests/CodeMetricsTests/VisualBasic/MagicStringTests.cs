using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.VisualBasic;
using FluentAssertions;
using System.Linq;
using InspectionTests.Builders;
using Inspector.CodeMetrics.Scores;

namespace InspectionTests.CodeMetricsTests.VisualBasic
{
    [TestClass]
    public class MagicStringTests 
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

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void DeclarationWithStringLiteral_ShouldHave_Score0()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean                       
                        Dim x as String = ""Hello""
                        return i > 0
                    End Function
                End Class
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void IfWithStringLiteral_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean                       
                        Dim x as String = ""Hello""
                        If x = ""Hello"" Then
                        End If
                        return i > 0
                    End Function
                End Class
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SelectCaseWithStringLiteral_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean                       
                        Dim x as String = ""Hello""
                        Select Case x
                            Case ""hello""
                                return True
                        End Select
                        Return False
                    End Function
                End Class
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void ReturnWithStringLiteralConditional_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean                       
                        Dim x as String = ""Hello""
                        Return x = ""Hello""
                    End Function
                End Class
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void SingleLineIfWithStringLiteral_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean                       
                        Dim x as String = ""Hello""
                        If x = ""Hello"" Then Return True End if
                        Return False
                    End Function
                End Class
                ");

            var sut = new MagicString();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

    }
}
