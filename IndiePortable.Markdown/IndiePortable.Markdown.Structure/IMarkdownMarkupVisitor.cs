// <copyright file="IMarkdownMarkupVisitor.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Structure
{
    public interface IMarkdownMarkupVisitor
    {

        void Visit(MarkdownDocument markup);

        void Visit(MarkdownAtxTitle markup);

        void Visit(MarkdownBlockquoteLine markup);

        void Visit(MarkdownCodeLine markup);

        void Visit(MarkdownBulletListItem markup);

        void Visit(MarkdownInlineCode markup);

        void Visit(MarkdownHorizontalRule markup);

        void Visit(MarkdownImage markup);

        void Visit(MarkdownTextLine markup);

        void Visit(MarkdownInlineTextMarkup markup);

        void Visit(MarkdownBold markup);

        void Visit(MarkdownItalic markup);

        void Visit(MarkdownLink markup);
    }
}
