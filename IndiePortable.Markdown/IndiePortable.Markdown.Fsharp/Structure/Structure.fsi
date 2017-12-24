namespace IndiePortable.Markdown.Fsharp
    type State =
    | BlockStartState
    | InlineState
    | ListItemState
    | QuotationState
    | CodeBlockState
    | FinalState

    type Token =
    | NewLineToken
    | TextToken of text : string
    | BoldToken
    | EmphasisToken
    | InlineCodeToken
    | BulletToken
    | ListNumberToken
    | CodeBlockToken
    | QuotationToken

    type StackSymbol =
    | Z0
    | Bold
    | Emphasis
    | InlineCode

    module Parser =
        type Transition = char list -> State -> StackSymbol list -> Token list -> (char list * State * StackSymbol list * Token list)
        type ParserResult =
        | Success of newInput:char list * newState:State * newStack:StackSymbol list * newOutput:Token list
        | Failure        

        val Parse : char list -> ParserResult
