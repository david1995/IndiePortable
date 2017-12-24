// <copyright file="Program.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Markdown.Test
{
    using System;
    using System.IO;
    using IndiePortable.Markdown.ParserFormatter;
    using IndiePortable.Markdown.Structure;

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.CurrentDirectory);

            var doc = new MarkdownDocument(
                new MarkdownAtxTitle("Hello", 1),
                new MarkdownTextLine(
                    new MarkdownBold("This"), new MarkdownInlineTextMarkup(" is a text.")),
                new MarkdownBulletListItem(new MarkdownInlineTextMarkup("A bullet item.")),
                new MarkdownTextLine(
                    new MarkdownImage("The Wikipedia logo", "https://upload.wikimedia.org/wikipedia/commons/8/80/Wikipedia-logo-v2.svg", "Wikipedia logo")),
                new MarkdownCodeLine("private void DoSomething()"),
                new MarkdownCodeLine("{"),
                new MarkdownCodeLine("    Console.WriteLine(\"Hello World!\");"),
                new MarkdownCodeLine("}"),
                new MarkdownBlockquoteLine(new MarkdownTextLine(new MarkdownInlineTextMarkup("Some quoted text. "), new MarkdownInlineCode("DoSomething();"))),
                new MarkdownBlockquoteLine(new MarkdownTextLine()),
                new MarkdownBlockquoteLine(
                    new MarkdownBulletListItem(new MarkdownInlineTextMarkup("Some bullet item."))),
                new MarkdownBlockquoteLine(
                    new MarkdownBulletListItem(
                        new MarkdownInlineCode("DoSomething()"),
                        new MarkdownInlineTextMarkup(" calls the method defined in the last paragraph."))));

            using (var sw = new StringWriter())
            {
                using (var wr = new MarkdownWriter(sw))
                {
                    doc.AcceptVisitor(wr);
                    var str = sw.GetStringBuilder().ToString();
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "test.md"), str);
                    Console.WriteLine(str);
                }
            }

            using (var sw = new StringWriter())
            {
                using (var wr = new HtmlWriter(sw))
                {
                    doc.AcceptVisitor(wr);
                    var str = sw.GetStringBuilder().ToString();
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "test.html"), str);
                }
            }

            Console.ReadLine();
        }
    }
}