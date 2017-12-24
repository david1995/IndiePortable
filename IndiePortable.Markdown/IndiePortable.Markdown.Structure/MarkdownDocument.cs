// <copyright file="MarkdownDocument.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class MarkdownDocument
        : MarkdownMarkup
    {

        public MarkdownDocument(params MarkdownBlock[] blocks)
        {
            this.Blocks = blocks?.ToArray() ?? throw new ArgumentNullException(nameof(blocks));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownDocument"/> class.
        /// </summary>
        protected MarkdownDocument()
        {
        }

        public IEnumerable<MarkdownBlock> Blocks { get; }

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
