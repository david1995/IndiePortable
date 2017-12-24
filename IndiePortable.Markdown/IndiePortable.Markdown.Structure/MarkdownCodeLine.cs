// <copyright file="MarkdownCodeLine.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownCodeLine
        : MarkdownBlock
    {
        public MarkdownCodeLine(string code)
        {
            this.Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownCodeLine"/> class.
        /// </summary>
        protected MarkdownCodeLine()
        {
        }

        public string Code { get; }

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
