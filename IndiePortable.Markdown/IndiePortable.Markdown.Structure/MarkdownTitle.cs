// <copyright file="MarkdownTitle.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public abstract class MarkdownTitle
        : MarkdownBlock
    {
        protected MarkdownTitle()
        {
        }

        protected MarkdownTitle(string text)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; }
    }
}
