using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DepTree.Tests
{
    public class DependencyTreeTests
    {
        [Fact]
        public void GivenAssemblyAndClassNameDependencyTreeCreated()
        {
            var assembly = this.GetType().Assembly;
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleType";

            var depTree = DependencyTree.Create(assembly, fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleType", depTree.Type.FullName);
            Assert.Empty(depTree.Children);
        }

        [Fact]
        public void GivenAssemblyAndClassNameWithDependenciesDependencyTreeCreated()
        {
            var assembly = this.GetType().Assembly;
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleTypeWithDeps";

            var depTree = DependencyTree.Create(assembly, fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleTypeWithDeps", depTree.Type.FullName);
            var childdep = Assert.Single(depTree.Children);

            Assert.NotNull(childdep);
            Assert.Equal("example", childdep.Name);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleType", childdep.Type.FullName);
            Assert.Empty(childdep.Children);
        }

        [Fact]
        public void CanResolveInterfaceDependencies()
        {
            var assembly = this.GetType().Assembly;
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps";

            var depTree = DependencyTree.Create(assembly, fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps", depTree.Type.FullName);
            var childdep = Assert.Single(depTree.Children);

            Assert.NotNull(childdep);
            Assert.Null(childdep.Error);
            Assert.Equal("example", childdep.Name);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleInterface", childdep.Type.FullName);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleImplementation", childdep.Implementation.FullName);
            Assert.Empty(childdep.Children);
        }

        public class ExampleTypeWithDeps
        {
            public ExampleTypeWithDeps(ExampleType example)
            {

            }
        }

        public class ExampleType
        {
        }

        public class ExampleTypeWithInterfaceDeps
        {
            public ExampleTypeWithInterfaceDeps(ExampleInterface example)
            {

            }
        }

        public interface ExampleInterface
        {
        }

        public class ExampleImplementation : ExampleInterface
        {
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTransient<ExampleInterface, ExampleImplementation>();
            }
        }
    }
}
