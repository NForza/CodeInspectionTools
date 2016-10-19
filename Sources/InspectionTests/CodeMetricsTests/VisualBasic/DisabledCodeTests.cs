using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.VisualBasic;
using FluentAssertions;
using System.Linq;
using InspectionTests.Builders;
using Inspector.CodeMetrics.Scores;

namespace InspectionTests.CodeMetricsTests.VisualBasic
{
    [TestClass]
    public class DisabledCodeTests 
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

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void NormalComment_ShouldHave_Score0()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean   
                        ' normal comment without code                    
                        return false
                    End Function
                End Class
                ");

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void IfStatementInComment_ShouldHave_Score1()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean   
                        ' normal comment without code     
                        ' If i = 10 Then Return True      
                        return false
                    End Function
                End Class
                ");

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void IfStatementBlockInComment_ShouldHave_Score3()
        {
            var parsedNode = new VBSyntaxTreeBuilder().FromSource(@"
                Imports System
                Imports System.Text

                <Serializable()> _
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean   
                        ' normal comment without code     
                        ' If i = 10 Then 
                        '   Return True      
                        ' End If
                        return false
                    End Function
                End Class
                ");

            var sut = new DisabledCode();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(3);
        }
    }
}
