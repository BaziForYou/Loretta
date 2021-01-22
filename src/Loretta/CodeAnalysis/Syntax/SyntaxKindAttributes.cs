﻿// This file contains the attributes used in the SyntaxKind enum members for tokens and keywords.
// They're here instead of being added by the source generator so that I can get a reference to
// their symbols instead of comparing strings which is more annoying and more error-prone.
using System;

namespace Loretta.CodeAnalysis.Syntax
{
    /// <summary>
    /// The trivia indicator attribute.
    /// Indicates to the SyntaxFacts Source Generator that this <see cref="SyntaxKind"/> is a trivia's.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal sealed class TriviaAttribute : Attribute
    {
        public TriviaAttribute ( )
        {
        }
    }

    /// <summary>
    /// The token indicator attribute.
    /// Indicates to the SyntaxFacts Source Generator that this <see cref="SyntaxKind"/> is a token's.
    /// May optionally indicate a fixed text for the token.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal sealed class TokenAttribute : Attribute
    {
        public TokenAttribute ( )
        {
        }

        /// <summary>
        /// The <see cref="SyntaxToken"/>'s fixed text.
        /// </summary>
        public String? Text { get; set; }
    }

    /// <summary>
    /// The keyword indicator attribute.
    /// Indicates to the SyntaxFacts Source Generator that this <see cref="SyntaxKind"/> is a keywords's
    /// and the keyword fixed text.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal sealed class KeywordAttribute : Attribute
    {
        public KeywordAttribute ( String text )
        {
            this.Text = text;
        }

        /// <summary>
        /// The keyword's text.
        /// </summary>
        public String Text { get; }
    }

    /// <summary>
    /// The unary operator indicator attribute.
    /// Indicates to the SyntaxFacts Source Generator that this
    /// <see cref="SyntaxKind"/> is an unary operator's with the
    /// provided precedence.
    /// THIS DOES NOT IMPLY THE <see cref="TokenAttribute"/>
    /// ATTRIBUTE.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal sealed class UnaryOperatorAttribute : Attribute
    {
        public UnaryOperatorAttribute ( Int32 precedence )
        {
            this.Precedence = precedence;
        }

        /// <summary>
        /// The unary operator's precedence.
        /// </summary>
        public Int32 Precedence { get; }
    }

    /// <summary>
    /// The binary operator indicator attribute.
    /// Indicates to the SyntaxFacts Source Generator that
    /// this <see cref="SyntaxKind"/> is a binary operator's
    /// with the provided precedence.
    /// THIS DOES NOT IMPLY THE <see cref="TokenAttribute"/>
    /// ATTRIBUTE.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal sealed class BinaryOperatorAttribute : Attribute
    {
        public BinaryOperatorAttribute ( Int32 precedence )
        {
            this.Precedence = precedence;
        }

        /// <summary>
        /// The binary operator's precedence.
        /// </summary>
        public Int32 Precedence { get; }
    }

    /// <summary>
    /// The syntax node indicator attribute.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    sealed class NodeAttribute : Attribute
    {
        public NodeAttribute ( Type type )
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}