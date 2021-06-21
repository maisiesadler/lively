using System;
using Lively.Resolvers;
using Lively.TypeDescriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lively.Tests
{
    public class DependencyTreeTests
    {
        [Fact]
        public void NullConfigThrowsException()
        {
            DependencyTreeConfig config = null;

            Assert.Throws<ArgumentNullException>(() => new DependencyTree(config));
        }

        [Fact]
        public void NullInterfaceResolverDefaultsToNone()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            config.CreateInterfaceResolver = null;

            var tree = new DependencyTree(config);

            Assert.Equal(typeof(NoInterfaceResolver), tree.InterfaceResolver.GetType());
        }

        [Fact]
        public void GivenAssemblyAndClassNameDependencyTreeCreated()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Tests.DependencyTreeTests+ExampleType";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleType", depTree.Type.FullName);
            Assert.Empty(depTree.Children);
        }

        [Fact]
        public void GivenAssemblyAndClassNameWithDependenciesDependencyTreeCreated()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Tests.DependencyTreeTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleTypeWithDeps", depTree.Type.FullName);
            var childdep = Assert.Single(depTree.Children);

            Assert.NotNull(childdep);
            Assert.Equal("example", childdep.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleType", childdep.Type.FullName);
            Assert.Empty(childdep.Children);
        }

        [Fact]
        public void CanResolveInterfaceDependencies()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps", depTree.Type.FullName);
            var childdep = Assert.Single(depTree.Children);

            Assert.NotNull(childdep);
            Assert.Null(childdep.Error);
            Assert.Equal("example", childdep.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleInterface", childdep.Type.FullName);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleImplementation", childdep.Implementation.FullName);
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
            var fullTypeName = "Lively.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleTypeWithInterfaceDeps", depTree.Type.FullName);
            var childdep = Assert.Single(depTree.Children);

            Assert.NotNull(childdep);
            Assert.Null(childdep.Error);
            Assert.Equal("example", childdep.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleInterface", childdep.Type.FullName);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleImplementation", childdep.Implementation.FullName);
            Assert.Empty(childdep.Children);
        }

        [Fact]
        public void GenericDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Tests.DependencyTreeTests+ExampleTypeWithGenericDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Null(depTree.Error);
            Assert.Equal("root", depTree.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleTypeWithGenericDeps", depTree.Type.FullName);
            var childdep = Assert.Single(depTree.Children);

            Assert.NotNull(childdep);
            Assert.Equal("example", childdep.Name);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleType`1", childdep.Type.FullName);
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
            var fullTypeName = "Lively.Tests.DependencyTreeTests+ExampleTypeWithRecursiveDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            Assert.NotNull(depTree);
            Assert.Equal("root", depTree.Name);

            var node = depTree;
            for (var i = 0; i <= 100; i++)
            {
                Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleTypeWithRecursiveDeps", node.Type.FullName);
                Assert.Equal("ExampleTypeWithRecursiveDeps", node.Type.Name);
                Assert.Null(node.Error);
                node = Assert.Single(node.Children);
            }

            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleTypeWithRecursiveDeps", node.Type.FullName);
            Assert.Equal("ExampleTypeWithRecursiveDeps", node.Type.Name);
            Assert.Equal(DependencyTreeError.TooManyLayers, node.Error);
            Assert.Null(node.Children);
        }

        [Fact]
        public void FindTypeInAnotherAssemblyNotReferenced_DoesNotWork()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var testTypeName = "Lively.Tests.DependencyTreeTests+ExampleType";
            var livelyTypeName = "Lively.DependencyTreeConfig";

            var tree = new DependencyTree(config);
            var testTypeDepTree = tree.GetDependencies(testTypeName);
            var livelyTypeDepTree = tree.GetDependencies(livelyTypeName);

            Assert.NotNull(testTypeDepTree);
            var testTypeDescription = Assert.IsType<ConcreteTypeDescription>(testTypeDepTree.Type);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleType", testTypeDescription.FullName);
            Assert.NotNull(livelyTypeDepTree);
            var livelyTypeDescription = Assert.IsType<UnknownTypeDescription>(livelyTypeDepTree.Type);
            Assert.Equal("Lively.DependencyTreeConfig", livelyTypeDescription.FullName);
        }

        [Fact]
        public void LoadMultipleAssembliesAndFindTypes()
        {
            var testAssembly = this.GetType().Assembly;
            var livelyAssembly = typeof(DependencyTreeConfig).Assembly;
            var config = new DependencyTreeConfig(new[] { testAssembly, livelyAssembly });
            var testTypeName = "Lively.Tests.DependencyTreeTests+ExampleType";
            var livelyTypeName = "Lively.DependencyTreeConfig";

            var tree = new DependencyTree(config);
            var testTypeDepTree = tree.GetDependencies(testTypeName);
            var livelyTypeDepTree = tree.GetDependencies(livelyTypeName);

            Assert.NotNull(testTypeDepTree);
            var testTypeDescription = Assert.IsType<ConcreteTypeDescription>(testTypeDepTree.Type);
            Assert.Equal("Lively.Tests.DependencyTreeTests+ExampleType", testTypeDescription.FullName);
            Assert.NotNull(livelyTypeDepTree);
            var livelyTypeDescription = Assert.IsType<ConcreteTypeDescription>(livelyTypeDepTree.Type);
            Assert.Equal("Lively.DependencyTreeConfig", livelyTypeDescription.FullName);
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
