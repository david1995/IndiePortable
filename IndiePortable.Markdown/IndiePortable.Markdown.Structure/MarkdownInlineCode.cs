// <copyright file="MarkdownInlineCode.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownInlineCode
        : MarkdownInlineTextMarkup
    {

        public MarkdownInlineCode(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownInlineCode"/> class.
        /// </summary>
        protected MarkdownInlineCode()
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
