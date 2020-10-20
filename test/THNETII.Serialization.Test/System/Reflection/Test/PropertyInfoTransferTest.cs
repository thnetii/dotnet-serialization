using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace THNETII.Serialization.System.Reflection.Test
{
    public static class PropertyInfoTransferTest
    {
        internal class TestReadWriteClass
        {
            public string? ReadWriteProperty { get; set; }
        }

        [Fact]
        public static void CanCreateUnboundInstancePropertyGetterDelegate()
        {
            var propertyInfo = typeof(TestReadWriteClass)
                .GetProperty(nameof(TestReadWriteClass.ReadWriteProperty))!;

            var getter = (Func<TestReadWriteClass, string?>)propertyInfo
                .GetMethod!.CreateDelegate(
                    typeof(Func<TestReadWriteClass, string?>));
            var instance = new TestReadWriteClass { ReadWriteProperty = "Test" };

            Assert.NotNull(getter);
            var propValue = getter(instance);
            Assert.Same(instance.ReadWriteProperty, propValue);
        }

        [Fact]
        public static void CanCreateUnboundInstancePropertySetterDelegate()
        {
            var propertyInfo = typeof(TestReadWriteClass)
                .GetProperty(nameof(TestReadWriteClass.ReadWriteProperty))!;

            var setter = (Action<TestReadWriteClass, string?>)propertyInfo
                .SetMethod!.CreateDelegate(
                    typeof(Action<TestReadWriteClass, string?>));
            var instance = new TestReadWriteClass();

            Assert.NotNull(setter);
            var propValue = "Test";
            setter(instance, propValue);
            Assert.Same(propValue, instance.ReadWriteProperty);
        }
    }
}
