namespace IndiePortable.Markdown.Parser

open System
open System.Collections.Generic
open System.IO
open System.Linq

[<AbstractClassAttribute>]
type public MarkdownNode() =
    abstract member GetChildren : unit -> MarkdownNode seq
    abstract member AcceptVisitor : unit -> unit

type public MarkdownTree(treeNodes : MarkdownNode seq) =
    inherit MarkdownNode()
    member public this.Nodes = treeNodes
    override this.GetChildren() = this.Nodes
    override this.Accept

[<AbstractClassAttribute>]
type public MarkdownLine() =
    inherit MarkdownNode()

type public MarkdownTextLine(text : string) =
    inherit MarkdownLine()

    member val public Text = text
    override this.GetChildren() = seq []

[<AbstractClassAttribute>]
type public MarkdownTitle(title : string) =
    inherit MarkdownLine()
    
    member val public Title : string = title
    override this.GetChildren() = seq []

type public MarkdownAtxTitle(title : string, level : int) =
    inherit MarkdownTitle(title)
    
    member val public Level : int = if level <= 0 || level >= 7
                                    then raise (ArgumentOutOfRangeException("level"))
                                    else level

type public MarkdownBlockquote(content : MarkdownLine) =
    inherit MarkdownLine()

    override this.GetChildren() = seq [ content ]

type public MarkdownBulletListItem(content : MarkdownLine) =
    inherit MarkdownLine()

    member val public Content : MarkdownLine = content
    override this.GetChildren() = seq [ this.Content ]

type public MarkdownBulletList(listItems : MarkdownBulletListItem seq) =
    inherit MarkdownNode()
    
    member public __.ListItems : MarkdownBulletListItem seq = listItems

    override this.GetChildren() = this.ListItems.Cast<MarkdownNode>()


type MarkdownParser(newLine : string) = 
    member val public NewLine : string = if ((seq [ "\r\n", "\r", "\n" ]).Contains(newLine))
                                         then newLine
                                         else raise(ArgumentException("newLine"))

    member private this.Serialize(node:MarkdownTextLine) : string = node.Text
    member private this.Serialize(node:MarkdownBulletListItem) : string = seq [ "> "; this.Serialize(node.Content) ] |> String.concat String.Empty
    member private this.Serialize(node:MarkdownBulletList) : string =
        node.ListItems
        |> Seq.map (fun n -> this.Serialize(n))
        |> String.concat this.NewLine

    member public this.Serialize(tree:MarkdownTree) : string = raise(NotImplementedException())

    // TODO: implement Visitor Pattern