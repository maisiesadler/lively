using System;
using Lively.Resolvers;
using Lively.TypeDescriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lively.Tests
{
    public class ConcreteTypeDescriptionTests
    {
        [Fact]
        public void NullTypeThrowsException()
        {
            Type type = null;

            Assert.Throws<ArgumentNullException>(() => new ConcreteTypeDescription(type));
        }

        [Fact]
        public void ExampleTypeNameReturned()
        {
            Type type = typeof(ExampleType);

            var typeDescription = new ConcreteTypeDescription(type);

            Assert.Equal("ExampleType", typeDescription.Name);
            Assert.Equal("Lively.Tests.ConcreteTypeDescriptionTests+ExampleType", typeDescription.FullName);
        }

        [Fact]
        public void ExampleTypeMethodNamesReturned()
        {
            Type type = typeof(ExampleType);

            var typeDescription = new ConcreteTypeDescription(type);

            var singleMethod = Assert.Single(typeDescription.Methods);

            Assert.Equal("Method", singleMethod.Name);
        }

        public class ExampleType
        {
            public void Method() { }
        }
    }
}
