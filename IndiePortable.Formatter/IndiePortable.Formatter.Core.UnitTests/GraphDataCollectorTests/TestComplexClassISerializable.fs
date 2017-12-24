[<Xunit.Trait("Target", "GraphDataCollector")>]
[<Xunit.Trait("Input", "Complex type / Class")>]
[<Xunit.Trait("Input implements", "ISerializable")>]
module ``GraphDataCollector Complex type Class implementing ISerializable``

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.Serialization
open IndiePortable.Formatter.Core
open IndiePortable.Formatter.Core.Graph
open Xunit

[<Serializable>]
type Complex2() =
    [<DefaultValue>]val mutable private text : string
    [<DefaultValue>]val mutable private value : int

    new (text : string, value : int) as this =
        Complex2()
        then
            this.text <- text
            this.value <- value

    new (data : SerializationInfo, _ : StreamingContext) =
        Complex2(data.GetString("Text"), data.GetInt32("Value"))

    member public this.Text with get() = this.text
    member public this.Value with get() = this.value

    interface ISerializable with
        member this.GetObjectData(data : SerializationInfo, ctx : StreamingContext) =
            data.AddValue("Text", this.Text)
            data.AddValue("Value", this.Value)

// GraphDataCollector is stateless and can therefore be used by all unit tests in parallel
let collector = GraphDataCollector();

[<Fact>]
[<Trait("Input", "Complex type / Class")>]
[<Trait("Input marked with", "SerializableAttribute")>]
[<Trait("Input implements", "ISerializable")>]
[<Trait("Expected", "All serializable fields")>]
let ``Collect data of Complex2 implementing ISerializable is ObjectData``() =
    let collected = Complex2("Test", 1) |> collector.GetGraphData :?> ObjectData
    Assert.Equal<struct(string * INodeData)>(// TODO: refactor to Assert.True
        collected.Fields |> Seq.map (fun f -> struct(f.FieldName, f.Value)),
        [struct("Text", StringData "Test" :> INodeData); struct("Value", ValueData 1 :> INodeData)])
