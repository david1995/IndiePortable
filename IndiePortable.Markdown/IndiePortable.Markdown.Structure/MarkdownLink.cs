// <copyright file="MarkdownLink.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownLink
        : MarkdownInlineTextMarkup
    {
        public MarkdownLink(string text, string reference, string title = null)
            : base(text)
        {
            this.Reference = reference ?? throw new ArgumentNullException(nameof(reference));
            this.Title = title;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownLink"/> class.
        /// </summary>
        protected MarkdownLink()
        {
        }

        public string Reference { get; }

        public string Title { get; }

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
