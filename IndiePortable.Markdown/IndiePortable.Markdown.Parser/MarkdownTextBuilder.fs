namespace IndiePortable.Markdown.Parser

open System
open System.Collections.Generic
open System.IO
open System.Linq

type MarkdownTextBuilder() =
    member public this.BuildString(tree : MarkdownTree) : string =
        raise(NotImplementedException())
    
    interface IMarkdownNodeVisitor with
        member this.Visit(node : MarkdownTree) = ()
        member this.Visit(node : MarkdownTextLine) = ()
        member this.Visit(node : MarkdownAtxTitle) = ()
        member this.Visit(node : MarkdownBlockquote) = ()
