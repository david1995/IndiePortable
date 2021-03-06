// <copyright file="DynamicArrayTTest.cs" company="David Eiwen">Copyright © 2016 by David Eiwen</copyright>
using System;
using System.Linq;
using IndiePortable.Collections;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndiePortable.Collections.Tests
{
    /// <summary>This class contains parameterized unit tests for DynamicArray`1</summary>
    [PexClass(typeof(DynamicArray<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class DynamicArrayTTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public DynamicArray<T> ConstructorTest<T>()
        {
            var target = new DynamicArray<T>();
            target.Add(default(T));
            target.Add(default(T));
            target.Add(default(T));

            foreach (var item in target)
            {
                Console.WriteLine(item);
            }

            return target;
            // TODO: add assertions to method DynamicArrayTTest.ConstructorTest()
        }
    }
}
