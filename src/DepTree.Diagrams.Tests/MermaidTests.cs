using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DepTree.Diagrams.Tests
{
    public class MermaidTests
    {
        private static Regex _whitespace = new Regex("[\r\n]+");

        [Fact]
        public void CanCreateDiagramForSimpleDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Diagrams.Tests.MermaidTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = Mermaid.Create(new[] { depTree });

            var expected = @"classDiagram
  ExampleTypeWithDeps --> ExampleType";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementation()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "DepTree.Diagrams.Tests.MermaidTests+Startup",
            };
            var fullTypeName = "DepTree.Diagrams.Tests.MermaidTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = Mermaid.Create(new[] { depTree });

            var expected = @"classDiagram
  ExampleTypeWithInterfaceDeps --> ExampleInterface
  class ExampleInterface {
    ExampleImplementation
  }";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void MultipleRegistrationsDedupedAndNoted()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "DepTree.Diagrams.Tests.MermaidTests+Startup",
            };
            var fullTypeName = "DepTree.Diagrams.Tests.MermaidTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = Mermaid.Create(new[] { depTree, depTree });

            var expected = @"classDiagram
  ExampleTypeWithInterfaceDeps --> ""2"" ExampleInterface
  class ExampleInterface {
    ExampleImplementation
  }";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void GenericTypeBackTicksAreConverted()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Diagrams.Tests.MermaidTests+ExampleTypeWithGenericDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = Mermaid.Create(new[] { depTree });

            var expected = @"classDiagram
  ExampleTypeWithGenericDeps --> ExampleGenericType2";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        private static string Normalise(string s)
        {
            return _whitespace.Replace(s, "\n").Trim();
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

        public class ExampleTypeWithGenericDeps
        {
            public ExampleTypeWithGenericDeps(ExampleGenericType<string, int> example)
            {

            }
        }

        public class ExampleGenericType<T1, T2>
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
