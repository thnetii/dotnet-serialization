using System;
using System.Collections.Generic;
using System.Text.Json;

using Xunit;

namespace THNETII.Serialization.JsonConverters.Test
{
    public static class BindingJsonConverterTest
    {
        private static readonly JsonSerializerOptions options =
            new JsonSerializerOptions
            { Converters = { BindingJsonConverterFactory.Instance } };

        public class TestClass1
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; } = null!;
            public bool Prop3 { get; set; }
        }

        [Fact]
        public static void CanDeserializePrimitivesOnlyJson()
        {
            const string json = @"{
    ""Prop1"": 42,
    ""Prop2"": ""Hello World"",
    ""Prop3"": true
}";

            var instance = JsonSerializer.Deserialize<TestClass1>(json, options);

            Assert.NotNull(instance);
            Assert.Equal(42, instance.Prop1);
            Assert.Equal("Hello World", instance.Prop2, StringComparer.Ordinal);
            Assert.True(instance.Prop3);
        }

        public class TestClass2
        {
            public ICollection<string> Values { get; } = new List<string>();
        }

        [Fact]
        public static void CanDeserializeCollectionIntoPreInitializedReadOnlyProperty()
        {
            const string json = @"{
    ""Values"": [
        ""Hello"",
        ""World""
    ]
}";

            var instance = JsonSerializer.Deserialize<TestClass2>(json, options);

            Assert.NotNull(instance);
            Assert.NotNull(instance.Values);
            Assert.Equal(new[] { "Hello", "World" }, instance.Values,
                StringComparer.Ordinal);
        }

        public class TestClass3
        {
            public TestClass1 Inner { get; } = new TestClass1();
        }

        [Fact]
        public static void CanDeserializeObjectIntoPreInitializedReadOnlyProperty()
        {
            const string json = @"{
    ""Inner"": {
        ""Prop1"": 42,
        ""Prop2"": ""Hello World"",
        ""Prop3"": true
    }
}";

            var instance = JsonSerializer.Deserialize<TestClass3>(json, options);

            Assert.NotNull(instance);
            Assert.NotNull(instance.Inner);
            Assert.Equal(42, instance.Inner.Prop1);
            Assert.Equal("Hello World", instance.Inner.Prop2, StringComparer.Ordinal);
            Assert.True(instance.Inner.Prop3);
        }
    }
}
