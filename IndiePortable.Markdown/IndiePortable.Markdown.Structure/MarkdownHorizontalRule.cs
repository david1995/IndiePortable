// <copyright file="MarkdownHorizontalRule.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownHorizontalRule
        : MarkdownBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownHorizontalRule"/> class.
        /// </summary>
        public MarkdownHorizontalRule()
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
