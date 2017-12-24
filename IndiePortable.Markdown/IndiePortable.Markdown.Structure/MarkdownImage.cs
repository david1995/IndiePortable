// <copyright file="MarkdownImage.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class MarkdownImage
        : MarkdownInlineMarkup
    {

        public MarkdownImage(string altText, string path, string title = null)
        {
            this.AltText = altText ?? throw new ArgumentNullException(nameof(altText));
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Title = title;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownImage"/> class.
        /// </summary>
        protected MarkdownImage()
        {
        }

        public string AltText { get; }

        public string Path { get; }

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
