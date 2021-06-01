using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lively.Tests
{
    public class TypeTests
    {
        [Fact]
        public void SkipTypesNotIncludedInDependencyTree()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                SkipTypes = new HashSet<string>
                {
                    "Lively.Tests.TypeTests+ExampleGenericInterface`1"
                },
            };
            var fullTypeName = "Lively.Tests.TypeTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("Lively.Tests.TypeTests+ExampleTypeWithDeps", depTree.Type.FullName);
            var childnode = Assert.Single(depTree.Children);
            Assert.NotNull(childnode);
            Assert.Equal("Lively.Tests.TypeTests+ExampleType", childnode.Type.FullName);
        }

        public class ExampleTypeWithDeps
        {
            public ExampleTypeWithDeps(ExampleType example, ExampleGenericInterface<int> @interface)
            {

            }
        }

        public class ExampleType
        {
        }

        public interface ExampleGenericInterface<T>
        {
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
            }
        }
    }
}
