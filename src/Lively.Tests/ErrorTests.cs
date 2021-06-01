using System;
using Lively.TypeDescriptions;
using Xunit;

namespace Lively.Tests
{
    public class ErrorTests
    {
        [Fact]
        public void TypeDoesNotExist()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Nonsense";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);
            var typeDescription = Assert.IsType<UnknownTypeDescription>(depTree.Type);
            Assert.Equal("Nonsense", typeDescription.FullName);
            Assert.Equal(DependencyTreeError.UnknownType, depTree.Error);
            Assert.Null(depTree.Children);
        }

        [Theory]
        [InlineData("Lively.Tests.ErrorTests+ExampleTypeTooMany")]
        [InlineData("Lively.Tests.ErrorTests+ExampleTypePrivateOnly")]
        public void IncorrectConstructors(string fullTypeName)
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

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
            var fullTypeName = "Lively.Tests.ErrorTests+ExampleWithErrorDep";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("Lively.Tests.ErrorTests+ExampleWithErrorDep", depTree.Type.FullName);
            Assert.Null(depTree.Error);
            var childdep = Assert.Single(depTree.Children);

            Assert.Equal("tooMany", childdep.Name);
            Assert.Equal("Lively.Tests.ErrorTests+ExampleTypeTooMany", childdep.Type.FullName);
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
