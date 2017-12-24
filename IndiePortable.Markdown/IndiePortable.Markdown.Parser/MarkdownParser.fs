namespace IndiePortable.Markdown.Parser

open System
open System.Collections.Generic
open System.IO
open System.Linq


type MarkdownParser(newLine : string) = 
    member val public NewLine : string = if ((seq [ "\r\n", "\r", "\n" ]).Contains newLine) then newLine else raise(ArgumentException("newLine"))

    member private this.Serialize(node:MarkdownTextLine) : string = node.Text
    member private this.Serialize(node:MarkdownBulletListItem) : string = seq [ "> "; this.Serialize(node.Content) ] |> String.concat String.Empty
    member private this.Serialize(node:MarkdownBulletList) : string =
        node.ListItems
        |> Seq.map (fun n -> this.Serialize(n))
        |> String.concat this.NewLine

    member public this.Serialize(tree:MarkdownTree) : string = raise(NotImplementedException())
