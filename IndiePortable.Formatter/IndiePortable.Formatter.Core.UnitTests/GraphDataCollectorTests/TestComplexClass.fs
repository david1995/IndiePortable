[<Xunit.Trait("Target", "GraphDataCollector")>]
[<Xunit.Trait("Input", "Complex type / Class")>]
module ``GraphDataCollector Complex type Class``

open System
open System.Collections.Generic
open System.Linq
open IndiePortable.Formatter.Core
open IndiePortable.Formatter.Core.Graph
open Xunit
open System.Runtime.Serialization

[<Serializable>]
type Complex1(t : string, v : int) =
    member public this.Text with get() = t
    member public this.Value with get() = v

[<Serializable>]
type Complex2(text, value) =
    [<field:NonSerialized>]
    member val public Text : string = text
    member val public Value : int = value

let collector = GraphDataCollector()
let equalityComp = 
    { new IEqualityComparer<struct(string * INodeData)> with
        member __.Equals(struct(xn, xv) : struct(string * INodeData), struct(yn, yv) : struct(string * INodeData)) =
            xn = yn && xv.Equals(yv)
        member __.GetHashCode(x : struct(string * INodeData)) = x.GetHashCode()
    }

[<Fact>]
[<Trait("Input marked with", "SerializableAttribute")>]
let ``Test Complex1 -> ObjectData``() =
    Complex1("Test", 1) |> collector.GetGraphData |> Assert.IsType<ObjectData> |> ignore

[<Fact>]
[<Trait("Input marked with", "SerializableAttribute")>]
[<Trait("Expected", "ObjectData with correct fields")>]
let ``Test Complex1 == ObjectData Fields``() =
    let collected = Complex1("Test", 1) |> collector.GetGraphData :?> ObjectData
    let actual = collected.Fields |> Seq.map (fun f -> struct(f.FieldName, f.Value)) |> seq
    let expected = seq [struct("v", ValueData 1 :> INodeData); struct("t", StringData "Test" :> INodeData)]
    Assert.True(
        expected.SequenceEqual(
            actual,
            { new IEqualityComparer<struct(string * INodeData)> with
                member __.Equals(struct(xn, xv) : struct(string * INodeData), struct(yn, yv) : struct(string * INodeData)) =
                    xn = yn && xv.Equals(yv)
                member __.GetHashCode(x : struct(string * INodeData)) = x.GetHashCode()
            }));

[<Fact>]
[<Trait("Input marked with", "SerializableAttribute")>]
[<Trait("Input field text marked with", "NonSerializedAttribute")>]
[<Trait("Expected", "ObjectData { Value@ = 1 }")>]
let ``Test Complex2 == ObjectData Fields``() =
    let collected = Complex2("Test", 1) |> collector.GetGraphData :?> ObjectData
    let actual = collected.Fields |> Seq.map (fun f -> struct(f.FieldName, f.Value)) |> seq
    let expected = seq [struct("Value@", ValueData 1 :> INodeData)]
    expected.SequenceEqual(actual, equalityComp) |> Assert.True

[<Fact>]
[<Trait("Expected", "SerializationException")>]
let ``Test Complex type wo SerializableAttribute -> SerializationException``() =
    Assert.Throws<SerializationException>(
        fun () ->
            IndiePortable.Formatter.Core.Tests.ComplexType1("Test", 1)
            |> collector.GetGraphData
            |> ignore)

    