// <copyright file="MarkdownItalic.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;

    [Serializable]
    public class MarkdownItalic
        : MarkdownInlineTextMarkup
    {
        public MarkdownItalic(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownItalic"/> class.
        /// </summary>
        protected MarkdownItalic()
        {
        }

        public override void AcceptVisitor(IMarkdownMarkupVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(this);
        }
    }
}
