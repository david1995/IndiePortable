// <copyright file="MarkdownBlock.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public abstract class MarkdownBlock
        : MarkdownMarkup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownBlock"/> class.
        /// </summary>
        protected MarkdownBlock()
        {
        }
    }
}
