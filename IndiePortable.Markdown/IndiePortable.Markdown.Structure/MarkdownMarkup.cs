// <copyright file="MarkdownMarkup.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public abstract class MarkdownMarkup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownMarkup"/> class.
        /// </summary>
        protected MarkdownMarkup()
        {
        }

        public abstract void AcceptVisitor(IMarkdownMarkupVisitor visitor);
    }
}
