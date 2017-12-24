[<Xunit.Trait("Target", "GraphDataCollector")>]
[<Xunit.Trait("Input", "string")>]
module ``GraphDataCollector int32 tests``

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.Serialization
open IndiePortable.Formatter.Core
open IndiePortable.Formatter.Core.Graph
open Xunit

let collector = GraphDataCollector()

[<Fact>]
[<Trait("Expected", "ValueData")>]
let ``Test int32 -> ValueData``() =
     2 |> collector.GetGraphData |> Assert.IsType<ValueData>

[<Fact>]
[<Trait("Expected", "ValueData.Value == correct")>]
let ``Test int32 == ValueData value``() =
    let collected = 2 |> collector.GetGraphData :?> ValueData
    let expected = 2 :> ValueType
    Assert.Equal(expected, collected.Value)