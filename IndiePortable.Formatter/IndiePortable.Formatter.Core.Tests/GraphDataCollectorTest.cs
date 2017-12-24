using System;
using System.Linq;
using System.Runtime.Serialization;
using IndiePortable.Formatter.Core.Graph;
using Xunit;
using System.Collections.Generic;

namespace IndiePortable.Formatter.Core.Tests
{
    public class GraphDataCollectorTest
    {
        private readonly GraphDataCollector collector = new GraphDataCollector();

        [Fact]
        public void TestStringCollect()
        {
            const string testString = "Hello World";
            var toCompare = this.collector.GetGraphData(testString);
            Assert.True(toCompare is StringData);
        }

        [Fact]
        public void TestSerializableAttributeComplexCollect()
        {
            var testInstance = new SerializationTest("Test", 1);
            var result = this.collector.GetGraphData(testInstance);
            Assert.True(result is ObjectData o && o.Fields.Count() == 2);
        }

        [Fact]
        public void TestISerializableComplexCollect()
        {
            var testInstance = new SerializableTest("Test", 1);
            var result = this.collector.GetGraphData(testInstance);
            Assert.True(result is ObjectData o && o.Fields.Count() == 2);
        }

        [Fact]
        public void TestNonSerializableComplexCollect()
        {
            var testInstance = new NonSerializableTest("Test", 1);
            Assert.Throws<SerializationException>(() => this.collector.GetGraphData(testInstance));
        }

        [Fact]
        public void TestListCollect()
        {
            var testInstance = new List<int> { 1, 2, 3 };
            var actual = this.collector.GetGraphData(testInstance);
            Assert.True(actual is ArrayData a && a.Length == 3 && a.Values.Count == 3);
        }

        [Serializable]
        public class SerializationTest
        {
            public SerializationTest(string text, int value)
            {
                this.Text = text;
                this.Value = value;
            }

            protected SerializationTest()
            {
            }

            public string Text { get; private set; }

            public int Value { get; private set; }
        }

        [Serializable]
        public class SerializableTest
            : ISerializable
        {
            public SerializableTest(string text, int value)
            {
                this.Text = text;
                this.Value = value;
            }

            protected SerializableTest()
            {
            }

            protected SerializableTest(SerializationInfo info, StreamingContext context)
            {
                this.Text = info.GetString(nameof(this.Text));
                this.Value = info.GetInt32(nameof(this.Value));
            }

            public string Text { get; }

            public int Value { get; }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(this.Text), this.Text);
                info.AddValue(nameof(this.Value), this.Value);
            }
        }

        public class NonSerializableTest
        {
            public NonSerializableTest(string text, int value)
            {
                this.Text = text;
                this.Value = value;
            }

            protected NonSerializableTest()
            {
            }

            public string Text { get; private set; }

            public int Value { get; private set; }
        }
    }
}
