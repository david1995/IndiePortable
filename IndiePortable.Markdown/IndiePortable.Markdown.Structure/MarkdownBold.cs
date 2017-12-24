// <copyright file="MarkdownBold.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownBold
        : MarkdownInlineTextMarkup
    {

        public MarkdownBold(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownBold"/> class.
        /// </summary>
        protected MarkdownBold()
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
