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
        type ParserResult =
        | Success of newInput:char list * newState:State * newStack:StackSymbol list * newOutput:Token list
        | Failure
        type Transition2 = char list -> State -> StackSymbol list -> Token list -> ParserResult
        type public Transition = char list -> State -> StackSymbol list -> Token list -> (char list * State * StackSymbol list * Token list)

        let ``Asterisk::Space to BulletToken`` input state stack tokens =
            match (input, state, stack) with
            | ('*'::' '::t, BlockStartState, [Z0]) -> Success(t, ListItemState, [Z0], (tokens, [BulletToken]) ||> List.append)
            | _ -> Failure
        
        let ``Asterisk::Asterisk to StrongToken Push Strong`` input state stack tokens =
            match (input, state, stack) with
            | ('*'::'*'::t, BlockStartState, [Z0]) -> Success(t, InlineState, Bold::stack, (tokens, [BulletToken]) ||> List.append)
            | _ -> Failure

        let ``Asterisk::Asterisk to StrongToken Pop Strong`` input state stack tokens =
            match (input, state, stack) with
            | ('*'::'*'::t, InlineState, [Bold; Z0]) -> Success(t, InlineState, [Z0], (tokens, [BulletToken]) ||> List.append)
            | _ -> Failure

        let ``AngularClosing::Space to ``

        let Transitions : Transition2 list =
            [
                ``Asterisk::Asterisk to StrongToken Push Strong``;
                ``Asterisk::Asterisk to StrongToken Pop Strong``;
                ``Asterisk::Space to BulletToken``;
            ];

        let ExecuteTransition input state stack tokens =
            match (input, state, stack) with
            | ('*'::' '::t, BlockStartState, [Z0]) -> Success(t, ListItemState, [Z0], (tokens, [BulletToken]) ||> List.append)
            | ('*'::'*'::t, BlockStartState, [Z0]) -> Success(t, InlineState, Bold::stack, (tokens, [BulletToken]) ||> List.append)
            | ('*'::'*'::t, InlineState, [Bold; Z0]) -> Success(t, InlineState, [Z0], (tokens, [BulletToken]) ||> List.append)
            | _ -> Failure

        let rec ParseRec (input:char list) state (stack:StackSymbol list) tokens =
            match ExecuteTransition input state stack tokens with
            | Success([], _, _, _) as succ -> succ 
            | Success(newInput, newState, newStack, newOutput) -> ParseRec newInput newState newStack newOutput
            | _ -> Failure

        let Parse (input:char list) =
            ParseRec input BlockStartState [Z0] []
