// <copyright file="MarkdownWriter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.ParserFormatter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using IndiePortable.Markdown.Structure;

    public class MarkdownWriter
        : IMarkdownMarkupVisitor, IDisposable
    {
        private TextWriter target;
        private bool isDisposed;
        private Stack<MarkdownBlock> currentBlocks = new Stack<MarkdownBlock>();

        public MarkdownWriter(TextWriter target, char boldChar = '*', char italicChar = '*', int hrCharCount = 3, char hrChar = '-', char bulletChar = '*')
        {
            this.target = target ?? throw new ArgumentNullException(nameof(target));
            this.HorizontalRuleCharCount = hrCharCount < 3
                                         ? throw new ArgumentOutOfRangeException(nameof(hrCharCount))
                                         : hrCharCount;

            this.HorizontalRuleChar = !IsHorizontalRuleChar(hrChar)
                                    ? throw new ArgumentException()
                                    : hrChar;

            this.BulletChar = !IsBulletChar(bulletChar)
                            ? throw new ArgumentException()
                            : bulletChar;

            this.BoldChar = !IsBoldChar(boldChar)
                          ? throw new ArgumentException()
                          : boldChar;

            this.ItalicChar = !IsItalicChar(italicChar)
                            ? throw new ArgumentException()
                            : italicChar;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MarkdownWriter"/> class.
        /// </summary>
        ~MarkdownWriter()
        {
            this.Dispose(false);
        }

        public int HorizontalRuleCharCount { get; }

        public char HorizontalRuleChar { get; }

        public char BulletChar { get; }

        public char BoldChar { get; }

        public char ItalicChar { get; }

        public static bool IsHorizontalRuleChar(char value) => value is '_' || value is '-' || value is '*';

        public static bool IsBulletChar(char value) => value is '*' || value is '-' || value is '+';

        public static bool IsBoldChar(char value) => value is '*' || value is '_';

        public static bool IsItalicChar(char value) => value is '*' || value is '_';

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                this.target.Dispose();
                this.isDisposed = true;
            }
        }

        public void Visit(MarkdownDocument markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            foreach (var b in markup.Blocks)
            {
                b.AcceptVisitor(this);
            }
        }

        public void Visit(MarkdownAtxTitle markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine($"{string.Empty.PadLeft(markup.Level, '#')} {markup.Text}");
        }

        public void Visit(MarkdownBlockquoteLine markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write("> ");
            markup.Content.AcceptVisitor(this);
        }

        public void Visit(MarkdownCodeLine markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine($"    {markup.Code}");
        }

        public void Visit(MarkdownBulletListItem markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            foreach (var m in markup.InlineMarkup)
            {
                this.target.Write($"{this.BulletChar}  ");
                m.AcceptVisitor(this);
            }

            this.target.WriteLine();
        }

        public void Visit(MarkdownInlineCode markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write(
                markup.Text.Contains("`")
                ? $"`{markup.Text}`"
                : $"``{markup.Text.Replace("`", "(`)")}``");
        }

        public void Visit(MarkdownHorizontalRule markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine(string.Empty.PadLeft(this.HorizontalRuleCharCount, this.HorizontalRuleChar));
        }

        public void Visit(MarkdownImage markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine(
                markup.Title is null
                ? $"![{markup.AltText}]({markup.Path})"
                : $"![{markup.AltText}]({markup.Path} \"{markup.Title}\")");
        }

        public void Visit(MarkdownTextLine markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            foreach (var m in markup.InlineMarkup)
            {
                m.AcceptVisitor(this);
            }

            this.target.WriteLine();
        }

        public void Visit(MarkdownInlineTextMarkup markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write(markup.Text);
        }

        public void Visit(MarkdownBold markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            var indicator = string.Empty.PadLeft(2, this.BoldChar);
            this.target.Write($"{indicator}{markup.Text}{indicator}");
        }

        public void Visit(MarkdownItalic markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write($"{this.ItalicChar}{markup.Text}{this.ItalicChar}");
        }

        public void Visit(MarkdownLink markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write(
                markup.Title is null
                ? $"[{markup.Text}]({markup.Reference})"
                : $"[{markup.Text}]({markup.Reference} \"{markup.Title}\")");
        }
    }
}
