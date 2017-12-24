namespace IndiePortable.Markdown.Parser

type public IMarkdownNodeVisitor =
    interface
    abstract member Visit : MarkdownTree -> unit
    abstract member Visit : MarkdownTextLine -> unit
    abstract member Visit : MarkdownAtxTitle -> unit
    abstract member Visit : MarkdownBlockquote -> unit
    end