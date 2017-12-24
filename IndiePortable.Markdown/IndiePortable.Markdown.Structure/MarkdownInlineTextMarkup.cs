// <copyright file="MarkdownInlineTextMarkup.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownInlineTextMarkup
        : MarkdownInlineMarkup
    {
        public MarkdownInlineTextMarkup(string text)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownInlineTextMarkup"/> class.
        /// </summary>
        protected MarkdownInlineTextMarkup()
        {
        }

        public string Text { get; }

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
