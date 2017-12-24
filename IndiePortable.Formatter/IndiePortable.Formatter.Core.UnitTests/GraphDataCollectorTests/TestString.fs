[<Xunit.Trait("Target", "GraphDataCollector")>]
[<Xunit.Trait("Input", "string")>]
module ``GraphDataCollector string tests``

open IndiePortable.Formatter.Core
open IndiePortable.Formatter.Core.Graph
open Xunit

let collector = GraphDataCollector()

[<Fact>]
[<Trait("Expected", "StringData")>]
let ``Test string -> StringData``() =
    "Hello World" |> collector.GetGraphData |> Assert.IsType<StringData> |> ignore

[<Fact>]
[<Trait("Expected", "StringData")>]
let ``Test string == StringData``() =
    let expected = "Hello World"
    let collected = expected |> collector.GetGraphData :?> StringData
    let actual = collected.Value
    Assert.Equal(expected, actual)