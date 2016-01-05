﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspector.CodeMetrics.VisualBasic;
using FluentAssertions;
using System.Linq;

namespace InspectionTests.CodeMetricsTests.VisualBasic
{
    [TestClass]
    public class EmptyStatementBlockTests : VisualBasicMetricTest
    {
        [TestMethod]
        public void SimpleReturn_ShouldHave_Score0()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
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
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void EmptyCatch_ShouldHave_Score1()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
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
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void EmptyCatchWithComment_ShouldHave_Score0()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
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
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }
    }
}