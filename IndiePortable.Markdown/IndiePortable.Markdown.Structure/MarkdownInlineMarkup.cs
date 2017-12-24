// <copyright file="MarkdownInlineMarkup.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public abstract class MarkdownInlineMarkup
        : MarkdownMarkup
    {
        protected MarkdownInlineMarkup()
        {
        }
    }
}
