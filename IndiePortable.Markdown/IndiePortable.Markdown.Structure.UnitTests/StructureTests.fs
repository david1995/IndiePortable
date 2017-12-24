module StructureTests

open IndiePortable.Markdown.Fsharp;
open Xunit;

[<Fact>]
let ``Test string 1``() =
    let input = "* "
    let actual = Parser.Parse(input.ToCharArray() |> List.ofArray)
    let expected = [ BulletToken ]
    match actual with
    | Parser.Success(_, _ , _, tokens)-> Assert.Equal<Token>(expected, tokens)
    | _ -> Assert.True false