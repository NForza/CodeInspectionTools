﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Inspector.CodeMetrics.Generic
{
    /// <summary>
    /// Basic (abstract) behavior. Use Inspector.Analyzers.CSharp.CommentLocator or
    /// Inspector.Analyzers.VisualBasic.CommentLocator instead.
    /// </summary>
    public abstract class CommentLocator
    {
        private readonly SyntaxNode syntaxNode;

        public CommentLocator(SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
                throw new ArgumentException("syntaxNode");

            this.syntaxNode = syntaxNode;
        }
        protected abstract bool IsComment(SyntaxTrivia trivia);

        public IEnumerable<Comment> GetComments(Predicate<string> filter=null)
        {
            if (filter == null)
                filter = s => true; // retrieve all comments when no filter is given

            var comments = new List<Comment>();

            var commentLocatingVisitor = new CommentLocatingVisitor(IsComment, comment =>
            {
                if (filter(comment.Content))
                    comments.Add(comment);
            });

            commentLocatingVisitor.Visit(
                syntaxNode
            );

            return comments;
        }

        private class CommentLocatingVisitor : SyntaxWalker
        {
            private readonly Action<Comment> _commentLocated;
            private readonly Func<SyntaxTrivia, bool> _commentRecognizer;

            public CommentLocatingVisitor(Func<SyntaxTrivia, bool> commentRecognizer, Action<Comment> commentLocated) : base(SyntaxWalkerDepth.StructuredTrivia)
            {
                if (commentRecognizer == null)
                    throw new ArgumentNullException("commentRecognizer");

                _commentRecognizer = commentRecognizer;

                if (commentLocated == null)
                    throw new ArgumentNullException("commentLocated");

                _commentLocated = commentLocated;
            }


            protected override void VisitTrivia(SyntaxTrivia trivia)
            {
                if (_commentRecognizer(trivia))
                {
                    string triviaContent;
                    using (var writer = new StringWriter())
                    {
                        trivia.WriteTo(writer);
                        triviaContent = writer.ToString();
                    }

                    // Note: When looking for the containingMethodOrPropertyIfAny, we want MemberDeclarationSyntax types such as ConstructorDeclarationSyntax, MethodDeclarationSyntax,
                    // IndexerDeclarationSyntax, PropertyDeclarationSyntax but NamespaceDeclarationSyntax and TypeDeclarationSyntax also inherit from MemberDeclarationSyntax and we
                    // don't want those
                    var containingNode = trivia.Token.Parent;
                    var containingMethodOrPropertyIfAny = TryToGetContainingNode<MemberDeclarationSyntax>(
                        containingNode,
                        n => !(n is NamespaceDeclarationSyntax) && !(n is TypeDeclarationSyntax)
                    );
                    var containingTypeIfAny = TryToGetContainingNode<TypeDeclarationSyntax>(containingNode);
                    var containingNameSpaceIfAny = TryToGetContainingNode<NamespaceDeclarationSyntax>(containingNode);
                    _commentLocated(new Comment(
                        triviaContent,
                        trivia.SyntaxTree.GetLineSpan(trivia.Span).StartLinePosition.Line,
                        containingMethodOrPropertyIfAny,
                        containingTypeIfAny,
                        containingNameSpaceIfAny
                    ));
                }
                base.VisitTrivia(trivia);
            }

            private T TryToGetContainingNode<T>(SyntaxNode node, Predicate<T> optionalFilter = null) where T : SyntaxNode
            {
                if (node == null)
                    throw new ArgumentNullException("node");

                var currentNode = node;
                while (true)
                {
                    var nodeOfType = currentNode as T;
                    if (nodeOfType != null)
                    {
                        if ((optionalFilter == null) || optionalFilter(nodeOfType))
                            return nodeOfType;
                    }
                    if (currentNode.Parent == null)
                        break;
                    currentNode = currentNode.Parent;
                }
                return null;
            }
        }
    }
}
