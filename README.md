# IndiePortable

The IndiePortable library provides some extensions for the .NET libraries. It currently contains two class libraries that are described further in the document.


## IndiePortable Formatter

The *IndiePortable Formatter* class library provides logic for serializing and deserializing managed objects. You can use it like the original [`BinaryFormatter`](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter(v=vs.110).aspx "BinaryFormatter (MSDN)") in the [`System.Runtime.Serialization.Formatters.Binary`](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary(v=vs.110).aspx "System.Runtime.Serialization.Formatters.Binary (MSDN)") namespace, but the Microsoft attributes and interfaces like [`SerializableAttribute`](https://msdn.microsoft.com/en-us/library/system.serializableattribute(v=vs.110).aspx "SerializableAttribute (MSDN)"), [`NonSerializedAttribute`](https://msdn.microsoft.com/en-us/library/system.nonserializedattribute(v=vs.110).aspx, "NonSerializedAttribute (MSDN)"), [`ISerializable`](https://msdn.microsoft.com/en-us/library/System.Runtime.Serialization.ISerializable.aspx "ISerializable (MSDN)") and [`ISerializationSurrogate`](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.iserializationsurrogate(v=vs.110).aspx "ISerializationSurrogate (MSDN)") are not supported currently. Instead, the library introduces its own serialization structure that is similar to the Microsoft implementation.

The huge disadvantage of Mircosofts `BinaryFormatter` is that its support includes only the .NET Framework (WinForms, WPF, ASP.NET, Console, Mono) but not .NET Core (UWP, WinRT, ASP.NET Core 5, future .NET branches). The *IndiePortable Formatter* solves this problem by unifying its serialization structure over all .NET platforms. Thus, the code for the formatter is identical on each platform. This allows, for example, simplified TCP communication between a Console app and a UWP app via one .NET assembly.

### Why should I use IndiePortable Formatter?

There are many alternatives for this library, like various JSON or XML formatter / serializer implementations for .NET, but most of them are built to serve a specific purpose.

For example, JSON serializers are very common in web communication via HTTP. And JSON is built for a certain kind of communication and language (dynamic typing). Thus, it represents the perfect fit for sending JavaScript objects via HTTP. The disadvantages of pure JSON: it is neither built for serializing circular object graphs nor static typing (like in C#/.NET).

The *IndiePortable Formatter* is built to provide a general purpose serialization library that can be used to serialize objects to any [`Stream`](https://msdn.microsoft.com/en-us/library/system.io.stream(v=vs.110) "Stream (MSDN)") output.

Although the library currentlyprovides only the `BinaryFormatter` class for (de-)serialization, you can implement your own formatter that uses the (de-)serialization infrastructure included in the library.

*But wouldn't it be nice to support static types as well as circular graphs in JSON?* Yes, it is. I plan to create a format based on JSON as description language and the *IndiePortable `BinaryFormatter`*'s output language as structure. It will be able to stores type information as attributes, e.g. type or field names and thus allow basic type checking during deserialization.


## IndiePortable Collections

The *IndiePortable Collections* class library contains additional collection types. They can be considered faster than a default list because each contained collection is based on an array. And arrays are optimized on most platforms.

In addition to that, the library contains [`IDictionary<TKey, TValue>`](https://msdn.microsoft.com/en-us/library/s4ys34ea(v=vs.110).aspx "IDictionary<TKey, TValue> (MSDN)") implementations based on the included collections.

The *IndiePortable Collections* classes use the interfaces and attributes provided by the *IndiePortable Formatter* library. Thus, they can easily be used by formatters to serialize and de-serialize them to and from streams.

### Why should I use IndiePortable Collections?

The Microsoft .NET framework already contains an array-based collection called [`ArrayList`](https://msdn.microsoft.com/en-us/library/system.collections.arraylist(v=VS.100).aspx "ArrayList (MSDN)") which is a non-generic type. That means no type check is performed when an element is added to an `ArrayList`. The *IndiePortable Collections* `DynamicArray<T>` class provides identical functionality and integrates type checking via generics.

Most .NET [`ICollection<T>`](https://msdn.microsoft.com/en-us/library/92t2ye13(v=vs.110).aspx "ICollection<T> (MSDN)") or [`ICollection`](https://msdn.microsoft.com/en-us/library/system.collections.icollection(v=vs.110).aspx "ICollection (MSDN)") implementations do not guarantee thread-safety. A thread could try to add an object to the collection while another one might enumerate it. By integrating a semaphore into the collections, issues with concurrency could be solved. Each collection of the *IndiePortable Collections* library integrates a semaphore to prevent concurrency issues.

The *IndiePortable Collections* library contains interfaces for implementing your own read-only and observable list classes. Additionally, it provides a default implementation for an observable list (`ObservableDynamicArray<T>`) and for a read-only observable list (`ReadOnlyObservableDynamicArray<T>`).

**Your contribution and ideas for these projects are welcome!**
