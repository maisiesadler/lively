using System;
using DepTree.TypeDescriptions;
using Xunit;

namespace DepTree.Tests
{
    public class ErrorTests
    {
        [Fact]
        public void TypeDoesNotExist()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Nonsense";

            var depTree = DependencyTree.Create(config, fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);
            var typeDescription = Assert.IsType<UnknownTypeDescription>(depTree.Type);
            Assert.Equal("Nonsense", typeDescription.FullName);
            Assert.Equal(DependencyTreeError.UnknownType, depTree.Error);
            Assert.Null(depTree.Children);
        }

        [Theory]
        [InlineData("DepTree.Tests.ErrorTests+ExampleTypeTooMany")]
        [InlineData("DepTree.Tests.ErrorTests+ExampleTypePrivateOnly")]
        public void IncorrectConstructors(string fullTypeName)
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);

            var depTree = DependencyTree.Create(config, fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);
            Assert.Equal(fullTypeName, depTree.Type.FullName);
            Assert.Equal(DependencyTreeError.IncorrectConstructors, depTree.Error);
            Assert.Null(depTree.Children);
        }

        [Fact]
        public void ExampleWithErrorDepHasChildWithError()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Tests.ErrorTests+ExampleWithErrorDep";

            var depTree = DependencyTree.Create(config, fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("DepTree.Tests.ErrorTests+ExampleWithErrorDep", depTree.Type.FullName);
            Assert.Null(depTree.Error);
            var childdep = Assert.Single(depTree.Children);

            Assert.Equal("tooMany", childdep.Name);
            Assert.Equal("DepTree.Tests.ErrorTests+ExampleTypeTooMany", childdep.Type.FullName);
            Assert.Equal(DependencyTreeError.IncorrectConstructors, childdep.Error);
        }

        public class ExampleWithErrorDep
        {
            public ExampleWithErrorDep(ExampleTypeTooMany tooMany) { }
        }

        public class ExampleTypeTooMany
        {
            public ExampleTypeTooMany(ExampleType example) { }
            public ExampleTypeTooMany(ExampleType example1, ExampleType example2) { }
        }

        public class ExampleTypePrivateOnly
        {
            private ExampleTypePrivateOnly(ExampleType example) { }
        }

        public class ExampleType
        {
        }
    }
}
