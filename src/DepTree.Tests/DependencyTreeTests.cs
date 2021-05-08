using System;
using Microsoft.Extensions.Configuration;
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
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleType";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

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
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

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
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

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

        [Fact]
        public void CanResolveInterfaceDependenciesWithConfig()
        {
            var assembly = this.GetType().Assembly;
            var cfgBuilder = new ConfigurationBuilder();
            var iconfiguration = cfgBuilder.Build();
            var config = new DependencyTreeConfig(assembly, iconfiguration)
            {
                StartupName = "StartupWithConfig",
            };
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

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

        [Fact]
        public void GenericDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleTypeWithGenericDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleTypeWithGenericDeps", depTree.Type.FullName);
            var childdep = Assert.Single(depTree.Children);

            Assert.NotNull(childdep);
            Assert.Equal("example", childdep.Name);
            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleType`1", childdep.Type.FullName);
            Assert.Equal("ExampleType`1", childdep.Type.Name);
            Assert.Empty(childdep.Children);
        }

        [Fact]
        public void UnknownGenericDependencyIsResolved()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "UnloadedLibrary.GenericTypeName`1[[UnloadedLibrary.GenericParameter, UnloadedLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("UnloadedLibrary.GenericTypeName`1", depTree.Type.FullName);
            Assert.Equal("GenericTypeName`1", depTree.Type.Name);
            Assert.Null(depTree.Children);
        }

        [Fact]
        public void RecursiveGenericDependencyDoesNotStackOverflow()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Tests.DependencyTreeTests+ExampleTypeWithRecursiveDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);

            var node = depTree;
            for (var i = 0; i <= 100; i++)
            {
                Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleTypeWithRecursiveDeps", node.Type.FullName);
                Assert.Equal("ExampleTypeWithRecursiveDeps", node.Type.Name);
                Assert.Null(node.Error);
                node = Assert.Single(node.Children);
            }

            Assert.Equal("DepTree.Tests.DependencyTreeTests+ExampleTypeWithRecursiveDeps", node.Type.FullName);
            Assert.Equal("ExampleTypeWithRecursiveDeps", node.Type.Name);
            Assert.Equal(DependencyTreeError.TooManyLayers, node.Error);
            Assert.Null(node.Children);
        }

        public class ExampleTypeWithDeps
        {
            public ExampleTypeWithDeps(ExampleType example) { }
        }

        public class ExampleType
        {
        }

        public class ExampleTypeWithInterfaceDeps
        {
            public ExampleTypeWithInterfaceDeps(ExampleInterface example) { }
        }

        public interface ExampleInterface
        {
        }

        public class ExampleImplementation : ExampleInterface
        {
        }

        public class ExampleTypeWithGenericDeps
        {
            public ExampleTypeWithGenericDeps(ExampleType<string> example) { }
        }

        public class ExampleType<T>
        {
        }

        public class ExampleTypeWithRecursiveDeps
        {
            public ExampleTypeWithRecursiveDeps(ExampleTypeWithRecursiveDeps example) { }
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTransient<ExampleInterface, ExampleImplementation>();
            }
        }

        public class StartupWithConfig
        {
            private readonly IConfiguration _configuration;

            public StartupWithConfig(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTransient<ExampleInterface, ExampleImplementation>();
            }
        }
    }
}
