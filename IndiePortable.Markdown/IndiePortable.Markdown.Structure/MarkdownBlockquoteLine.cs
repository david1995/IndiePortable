// <copyright file="MarkdownBlockquoteLine.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;

    [Serializable]
    public class MarkdownBlockquoteLine
        : MarkdownBlock
    {
        public MarkdownBlockquoteLine(MarkdownBlock content)
        {
            this.Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public MarkdownBlock Content { get; }

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
