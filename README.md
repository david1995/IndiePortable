

# IndiePortable

The IndiePortable library provides some extensions for the .NET libraries. It currently contains two class libraries that are described further in the document.


## IndiePortable Formatter

The *IndiePortable Formatter* class library provides logic for serializing and deserializing managed objects. You can use it like the original [`BinaryFormatter`](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter(v=vs.110).aspx "BinaryFormatter (MSDN)") in the [`System.Runtime.Serialization.Formatters.Binary`](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary(v=vs.110).aspx "System.Runtime.Serialization.Formatters.Binary (MSDN)") namespace, but the Microsoft attributes and interfaces like [`SerializableAttribute`](https://msdn.microsoft.com/en-us/library/system.serializableattribute(v=vs.110).aspx "SerializableAttribute (MSDN)"), [`NonSerializedAttribute`](https://msdn.microsoft.com/en-us/library/system.nonserializedattribute(v=vs.110).aspx, "NonSerializedAttribute (MSDN)"), [`ISerializable`](https://msdn.microsoft.com/en-us/library/System.Runtime.Serialization.ISerializable.aspx "ISerializable (MSDN)") and [`ISerializationSurrogate`](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.iserializationsurrogate(v=vs.110).aspx "ISerializationSurrogate (MSDN)") are not supported currently. Instead, the library introduces its own serialization structure that is similar to the Microsoft implementation.

The huge disadvantage of Mircosofts `BinaryFormatter` is that it is only available for Windows Forms, WPF, ASP.NET and Console projects. The *IndiePortable Formatter* solves this problem by unifying the serialization structure for all .NET platforms. Thus, the code for the formatter is identical on each platform. This allows, for example, simplified and unified communication between a Console app and a UWP app.

### Why should I use IndiePortable Formatter?

There are many alternatives for this library, like various JSON or XML formatter  / serializer implementations for .NET, but most of them are built to serve a specific purpose.

For example, JSON formatters are widely used to communicate via HTTP. The advantage: simplicity and readability. The disadvantage: most of them cannot serialize circular object graphs correctly because JSON is not built to behave this way. They have to use recursion limits to prevent a stack overflow which creates duplicates of the same object.

The *IndiePortable Formatter* is built to provide a general purpose serialization library that can be used to serialize objects to any output.

Although the library currently only provides the `BinaryFormatter` class for (de-)serialization, you can implement your own formatter that uses the (de-)serialization infrastructure included in the library. More formatters, specifically an XML and a JSON formatter, are planned to be included in future releases.

**Your contribution and ideas are welcome!**


## IndiePortable Collections

The *IndiePortable Collections* class library contains additional collection types. They can be considered faster than a default list because each contained collection is based on an array.

In addition to that, the library contains [`IDictionary<TKey, TValue>`](https://msdn.microsoft.com/en-us/library/s4ys34ea(v=vs.110).aspx "IDictionary<TKey, TValue> (MSDN)") implementations based on the included collections.

The *IndiePortable Collections* classes use the interfaces and attributes provided by the *IndiePortable Formatter* library. Thus, they can easily be used by formatters to serialize and de-serialize them to and from streams.

### Why should I use IndiePortable Collections?

The Microsoft .NET framework already contains an array-based collection called [`ArrayList`](https://msdn.microsoft.com/en-us/library/system.collections.arraylist(v=VS.100).aspx "ArrayList (MSDN)") that is a non-generic type. That means, that no type check is performed when an element is added to an instance of an `ArrayList` . The *IndiePortable Collections* `DynamicArray<T>` class provides the same functionality and adds support for type checks via generics.

Most .NET [`ICollection<T>`](https://msdn.microsoft.com/en-us/library/92t2ye13(v=vs.110).aspx "ICollection<T> (MSDN)") or [`ICollection`](https://msdn.microsoft.com/en-us/library/system.collections.icollection(v=vs.110).aspx "ICollection (MSDN)") implementations do not guarantee thread-safety. A thread could try to add an object to the collection while another one might enumerate the collection. This is handled by using semaphores for every action taken on the collection. Every collection in the *IndiePortable Collections* library is concurrent by using semaphores.

The *IndiePortable Collections* library contains interfaces for implementing your own read-only and observable list classes. It also contains a default implementation for both; `ObservableDynamicArray<T>` for an observable list and `ReadOnlyObservableDynamicArray<T>` for a read-only and observable list.

**Your contribution and ideas for this project are welcome!**
