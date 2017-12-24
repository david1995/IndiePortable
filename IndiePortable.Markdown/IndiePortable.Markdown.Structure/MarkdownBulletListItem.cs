﻿// <copyright file="MarkdownBulletListItem.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class MarkdownBulletListItem
        : MarkdownBlock
    {
        public MarkdownBulletListItem(params MarkdownInlineMarkup[] inlineMarkup)
        {
            this.InlineMarkup = inlineMarkup?.ToArray() ?? throw new ArgumentNullException(nameof(inlineMarkup));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownBulletListItem"/> class.
        /// </summary>
        protected MarkdownBulletListItem()
        {
        }

        public IEnumerable<MarkdownInlineMarkup> InlineMarkup { get; }

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
