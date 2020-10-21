using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace THNETII.Serialization.JsonConverters
{
    public class ReadOnlyBindingJsonConverter<T> : JsonConverter<T>
    {
        private static readonly Action<JsonSerializerOptions, JsonSerializerOptions> simpleShallowOptionsClone = GenerateShallowClone();
        private static List<PropertyInfo> readOnlySerializationProperties = GetReadOnlySerializationProperties();

        private static Action<JsonSerializerOptions, JsonSerializerOptions> GenerateShallowClone()
        {
            Type type = typeof(JsonSerializerOptions);
            var properties = type.GetProperties(
                BindingFlags.Public | BindingFlags.Instance)
                .Where<PropertyInfo>(pi => pi.CanRead && pi.CanWrite);
            var dst = Expression.Parameter(type, "clone");
            var src = Expression.Parameter(type, "origin");
            var block = Expression.Block(properties.Select(pi =>
            {
                var left = Expression.Property(dst, pi);
                var right = Expression.Property(src, pi);
                return Expression.Assign(left, right);
            }));
            return Expression.Lambda
                <Action<JsonSerializerOptions, JsonSerializerOptions>>(
                    block, dst, src
                ).Compile();
        }

        private static List<PropertyInfo> GetReadOnlySerializationProperties()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => !pi.CanWrite && pi.CanRead)
                .Where(pi => ReadOnlyBindingJsonConverterFactory.Instance.CanConvert(pi.PropertyType))
                .Where(pi => pi.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                .ToList();
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            var baseOptions = CloneOptions(options);
            var subReader = reader;
            var instance = (T)JsonSerializer.Deserialize(ref subReader, typeToConvert, baseOptions);
            if (readOnlySerializationProperties.Count > 0 &&
                JsonDocument.TryParseValue(ref reader, out var dom))
            {
                using (dom)
                {
                    foreach (var pi in readOnlySerializationProperties)
                    {

                    }
                }
            }

            return instance;
        }

        public override void Write(Utf8JsonWriter writer, T value,
            JsonSerializerOptions options)
        {
            var baseOptions = CloneOptions(options);
            JsonSerializer.Serialize(writer, value, baseOptions);
        }

        public JsonSerializerOptions? CloneOptions(
            JsonSerializerOptions? options)
        {
            if (options is null)
                return null;

            var clone = new JsonSerializerOptions();
            simpleShallowOptionsClone(clone, options);
            foreach (var conv in options.Converters)
            {
                if (conv is ReadOnlyBindingJsonConverter<T> ||
                    conv is ReadOnlyBindingJsonConverterFactory)
                    continue;
                clone.Converters.Add(conv);
            }

            return clone;
        }
    }
}
