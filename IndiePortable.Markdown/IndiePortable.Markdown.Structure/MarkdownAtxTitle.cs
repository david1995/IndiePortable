// <copyright file="MarkdownAtxTitle.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownAtxTitle
        : MarkdownTitle
    {

        public MarkdownAtxTitle(string text, int level)
            : base(text)
        {
            this.Level = level < 1 || level > 6
                       ? throw new ArgumentOutOfRangeException(nameof(level))
                       : level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownAtxTitle"/> class.
        /// </summary>
        protected MarkdownAtxTitle()
        {
        }

        public int Level { get; }

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
