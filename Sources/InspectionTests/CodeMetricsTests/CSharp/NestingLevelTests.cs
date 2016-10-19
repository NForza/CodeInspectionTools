using System.Linq;
using FluentAssertions;
using InspectionTests.Builders;
using Inspector.CodeMetrics.CSharp;
using Inspector.CodeMetrics.Scores;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InspectionTests.CodeMetricsTests.CSharp
{
    [TestClass]
    public class NestingLevelTests 
    {
        [TestMethod]
        public void EmptyMethod_ShouldHave_NestingLevel0()
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

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void SingleIfStatement_ShouldHave_NestingLevel1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        if (i > 10) {
                            return true;
                        }
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void NestedIfStatement_ShouldHave_NestingLevel2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        if (i > 10) 
                            if (true) 
                                return true;                            
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void TwoIfStatements_ShouldHave_NestingLevel1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        if (i > 10) 
                            return true;
                        
                        if (i < 0) 
                            return true;                            
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }
        [TestMethod]
        public void NestedIfStatementWithBlocks_ShouldHave_NestingLevel2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        if (i > 10)
                        {
                            if (true)
                            {
                                return true;                            
                            }
                        }
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void NestedIfStatementsWithBlocks_ShouldHave_NestingLevel3()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        if (i > 10) 
                        {
                            if (true) 
                            {
                                if (i==0) 
                                {
                                    return true; 
                                }
                            }
                        }
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void NestedIfStatements_ShouldHave_NestingLevel3()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        if (i > 10) 
                            if (true) 
                                if (i==0)
                                    return true; 
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void SimpleCase_ShouldHave_NestingLevel1()
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
                                return true;
                                break;
                            default:
                                break;
                        };
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void CaseWithNestedIf_ShouldHave_NestingLevel2()
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
                                if (i>0)
                                    return true;
                                break;
                            default:
                                break;
                        };
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void CaseWithNestedCase_ShouldHave_NestingLevel2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        int j = i * 2;
                        switch (i)
                        {
                            case 10:
                                switch (j) {
                                    case 20:
                                        return false;
                                        break;
                                }
                                return true;
                                break;
                            default:
                                break;
                        };
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void TwoCaseBlocks_ShouldHave_NestingLevel1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                     public bool TestMe(int i) {
                        int j = i * 2;
                        switch (i)
                        {
                            case 10:                                
                                return true;
                                break;
                            default:
                                break;
                        };
                        switch (j) {
                                    case 20:
                                        return false;
                                        break;
                                };
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void CaseWithNestedCaseWithNestedIf_ShouldHave_NestingLevel3()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        int j = i * 2;
                        switch (i)
                        {
                            case 10:
                                switch (j) {
                                    case 20:
                                        if (i+j == 30)
                                        {
                                            return false;
                                        }
                                        break;
                                }
                                return true;
                                break;
                            default:
                                break;
                        };
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void ForLoop_ShouldHave_NestingLevel1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {                        
                        for (int j=0; j<i; j++) {
                            //nothing here
                        };
                        
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void IfWithNestedForLoop_ShouldHave_NestingLevel2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {    
                        if (i > 10) {                    
                            for (int j=0; j<i; j++) {
                                //nothing here
                            };
                        }
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void ForLoopWithNestedIf_ShouldHave_NestingLevel2()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {                                              
                        for (int j=0; j<i; j++) {
                            if (j > 10) {
                                //nothing here
                            }
                        };
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void While_ShouldHave_NestingLevel1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                   public bool TestMe(int i) {
                        while (i > 10) {
                            i--;
                        }
                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void TwoWhile_ShouldHave_NestingLevel1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {
                    public bool TestMe(int i) {
                        while (i > 10) {
                            i--;
                        }
                        while (i > 10) {
                            i--;
                        }

                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void DoWhile_ShouldHave_NestingLevel1()
        {
            var parsedNode = new CSharpSyntaxTreeBuilder().FromSource(@"
                using System;
                using System.Text;

                [Serializable]
                public class TestClass {                    
                    public bool TestMe(int i) {
                        do  {
                            i--;
                        } while (i > 10);

                        return false;
                    }
                }
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode, "TestProjectName");

            results.Should().HaveCount(1);
            results.OfType<MethodScore>().First().Method.Should().Be("bool TestMe (int i)");
            results.First().Score.Should().Be(1);
        }

    }
}
