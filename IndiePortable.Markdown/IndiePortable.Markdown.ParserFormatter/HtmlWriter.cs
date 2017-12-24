// <copyright file="HtmlWriter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.ParserFormatter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.Encodings.Web;
    using IndiePortable.Markdown.Structure;

    public class HtmlWriter
        : IMarkdownMarkupVisitor, IDisposable
    {
        private bool isDisposed;
        private TextWriter target;
        private HtmlEncoder encoder;

        public HtmlWriter(TextWriter target, bool writeFullHtmlDocument = true, HtmlEncoder encoder = null)
        {
            this.target = target ?? throw new ArgumentNullException(nameof(target));
            this.WriteFullHtmlDocument = writeFullHtmlDocument;
            this.encoder = encoder ?? HtmlEncoder.Default;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HtmlWriter"/> class.
        /// </summary>
        ~HtmlWriter()
        {
            this.Dispose(false);
        }

        public bool WriteFullHtmlDocument { get; }

        public void Visit(MarkdownDocument markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            if (this.WriteFullHtmlDocument)
            {
                this.target.WriteLine("<html>");
                this.target.WriteLine("<body>");
            }

            foreach (var block in markup.Blocks)
            {
                block.AcceptVisitor(this);
            }

            if (this.WriteFullHtmlDocument)
            {
                this.target.WriteLine("</body>");
                this.target.WriteLine("</html>");
            }
        }

        public void Visit(MarkdownAtxTitle markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine($"<h{markup.Level}>{this.encoder.Encode(markup.Text)}</h{markup.Level}>");
        }

        public void Visit(MarkdownBlockquoteLine markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            markup.Content.AcceptVisitor(this);
        }

        public void Visit(MarkdownCodeLine markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine(markup.Code);
        }

        public void Visit(MarkdownBulletListItem markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine("<li>");

            foreach (var i in markup.InlineMarkup)
            {
                i.AcceptVisitor(this);
            }

            this.target.WriteLine("</li>");
        }

        public void Visit(MarkdownInlineCode markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write($"<code>{this.encoder.Encode(markup.Text)}</code>");
        }

        public void Visit(MarkdownHorizontalRule markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine("<hr/>");
        }

        public void Visit(MarkdownImage markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write($@"<img alt=""{this.encoder.Encode(markup.AltText)}"" src=""{this.encoder.Encode(markup.Path)}""/>");
        }

        public void Visit(MarkdownTextLine markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.WriteLine("<p>");

            foreach (var i in markup.InlineMarkup)
            {
                i.AcceptVisitor(this);
            }

            this.target.WriteLine("</p>");
        }

        public void Visit(MarkdownInlineTextMarkup markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write(this.encoder.Encode(markup.Text));
        }

        public void Visit(MarkdownBold markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write($"<strong>{this.encoder.Encode(markup.Text)}</strong>");
        }

        public void Visit(MarkdownItalic markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write($"<em>{this.encoder.Encode(markup.Text)}</em>");
        }

        public void Visit(MarkdownLink markup)
        {
            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            this.target.Write(
                markup.Title is null
                ? $@"<a href=""{this.encoder.Encode(markup.Reference)}"">{this.encoder.Encode(markup.Text)}</a>"
                : $@"<a href=""{this.encoder.Encode(markup.Reference)}"" title=""{this.encoder.Encode(markup.Title)}"" >{this.encoder.Encode(markup.Text)}</a>");
        }

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
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
    }
}
