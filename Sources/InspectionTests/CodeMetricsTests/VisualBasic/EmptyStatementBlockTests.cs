using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.VisualBasic;
using FluentAssertions;
using System.Linq;
using InspectionTests.Builders;
using Inspector.CodeMetrics.Scores;

namespace InspectionTests.CodeMetricsTests.VisualBasic
{
    [TestClass]
    public class EmptyStatementBlockTests 
    {
        [TestMethod]
        public void SimpleReturn_ShouldHave_Score0()
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

            var sut = new EmptyStatementBlock();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void EmptyCatch_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean   
                        Try
                            i=i*2
                        Catch ex as Exception
                        End Try
                                   
                        return false
                    End Function
                End Class
                ");

            var sut = new EmptyStatementBlock();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void EmptyCatchWithComment_ShouldHave_Score0()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean   
                        Try
                            i=i*2
                        Catch ex as Exception
                            ' Just ignore
                        End Try
                                   
                        return false
                    End Function
                End Class
                ");

            var sut = new EmptyStatementBlock();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }
    }
}
