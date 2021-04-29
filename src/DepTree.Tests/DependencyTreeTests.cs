using System;
using Xunit;

namespace DepTree.Tests
{
    public class DependencyTreeTests
    {
        [Fact]
        public void GivenAssemblyAndClassNameDependencyTreeCreated()
        {
            var assembly = this.GetType().Assembly;
            var fullTypeName = "DepTree.Tests.ExampleType";

            var depTree = DependencyTree.Create(assembly, fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("DepTree.Tests.ExampleType", depTree.Type.FullName);
            Assert.Empty(depTree.Children);
        }
    }

    public class ExampleType
    {
    }
}
