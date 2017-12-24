module IndiePortable.Communication.Binary.UnitTests.InMemoryStreamConnectionTests

open System
open System.Threading
open IndiePortable.Communication.Binary
open Xunit

[<Fact>]
let ``Connect a client to a listener at port 17``() =
    // generate event
    use event = new AutoResetEvent(false)

    // generate listenenr
    use listener = new InMemoryStreamConnectionListener(17)

    // declare client 2 & connection variables
    let mutable client2 : InMemoryStreamConnectionClient = null
    let mutable client2conn : InMemoryStreamConnection = null;
    listener.add_ConnectionRequested((fun _ question ->
        client2 <- question.Accept()
        client2conn <- client2.Connection.Activate()
        event.Set() |> ignore))
    listener.StartListening()
    use client1 = (new InMemoryStreamConnectionClient()).Initialize()
    use client1conn = client1.Connect(17).Initialize().Activate()

    // wait until both connection end points are ready
    event.WaitOne() |> ignore

    Assert.True(client1.IsConnected)
    Assert.True(client2.IsConnected)

    client1.Disconnect()
    client2.Disconnect()
