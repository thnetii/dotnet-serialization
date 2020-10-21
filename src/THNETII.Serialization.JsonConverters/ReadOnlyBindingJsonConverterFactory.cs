using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace THNETII.Serialization.JsonConverters
{
    public class ReadOnlyBindingJsonConverterFactory : JsonConverterFactory
    {
        private static readonly Dictionary<Type, Type> typedConverterTypes =
            new Dictionary<Type, Type>();

        public static ReadOnlyBindingJsonConverterFactory Instance { get; } =
            new ReadOnlyBindingJsonConverterFactory();

        private ReadOnlyBindingJsonConverterFactory() : base() { }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert is null || !typeToConvert.IsClass)
                return false;

            lock (typedConverterTypes)
            {
                if (typedConverterTypes.ContainsKey(typeToConvert))
                    return true;
            }

            return true;
        }

        public override JsonConverter CreateConverter(Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (options?.IgnoreReadOnlyProperties ?? false)
                return default!;

            var converterType = GetOrCreateTypedConverter(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType,
                BindingFlags.Public | BindingFlags.Instance,
                Type.DefaultBinder, Array.Empty<object>(),
                CultureInfo.InvariantCulture)!;
        }

        private static Type GetOrCreateTypedConverter(Type typeToConvert)
        {
            Type? converterType;
            lock (typedConverterTypes)
            {
                if (typedConverterTypes.TryGetValue(typeToConvert, out converterType))
                    return converterType;
            }

            converterType = typeof(ReadOnlyBindingJsonConverter<>)
                .MakeGenericType(typeToConvert);
            lock (typedConverterTypes)
            {
                typedConverterTypes[typeToConvert] = converterType;
            }

            return converterType;
        }
    }
}
