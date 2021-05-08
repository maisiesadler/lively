using System.Text.RegularExpressions;
using DepTree.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DepTree.Diagrams.Tests
{
    public class YumlMdTests
    {
        private static Regex _whitespace = new Regex("[\r\n]+");

        [Fact]
        public void CanCreateDiagramForSimpleDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Diagrams.Tests.YumlMdTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = yUMLmd.Create(new[] { depTree });

            var expected = @"<img src=""http://yuml.me/diagram/scruffy/class/[ExampleTypeWithDeps]-&gt;[ExampleType]"" />";

            Assert.Equal(expected.Trim(), diagram.Trim());
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementation()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "DepTree.Diagrams.Tests.YumlMdTests+Startup",
            };
            var fullTypeName = "DepTree.Diagrams.Tests.YumlMdTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = yUMLmd.Create(new[] { depTree });

            var expected = @"<img src=""http://yuml.me/diagram/scruffy/class/[ExampleTypeWithInterfaceDeps]-&gt;[ExampleInterface|ExampleImplementation]"" />";

            Assert.Equal(expected.Trim(), diagram.Trim());
        }

        [Fact]
        public void MultipleRegistrationsDedupedAndNoted()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "DepTree.Diagrams.Tests.YumlMdTests+Startup",
            };
            var fullTypeName = "DepTree.Diagrams.Tests.YumlMdTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = yUMLmd.Create(new[] { depTree, depTree });

            var expected = @"<img src=""http://yuml.me/diagram/scruffy/class/[ExampleTypeWithInterfaceDeps]-2&gt;[ExampleInterface|ExampleImplementation]"" />";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        private static string Normalise(string s)
        {
            return _whitespace.Replace(s, "\n");
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
