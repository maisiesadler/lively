using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lively.Diagrams.Tests
{
    public class MermaidMdTests
    {
        private static Regex _whitespace = new Regex("[\r\n]+");

        [Fact]
        public void CanCreateDiagramForSimpleDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Diagrams.Tests.MermaidTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = MermaidMd.Create(new[] { depTree });

            var expected = @"```mermaid
classDiagram
  ExampleTypeWithDeps --> ExampleType
```";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementation()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.MermaidTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.MermaidTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = MermaidMd.Create(new[] { depTree });

            var expected = @"```mermaid
classDiagram
  class ExampleInterface {
    ExampleImplementation
  }
  ExampleTypeWithInterfaceDeps --> ExampleInterface
```";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void MultipleRegistrationsDedupedAndNoted()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.MermaidTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.MermaidTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = MermaidMd.Create(new[] { depTree, depTree });

            var expected = @"```mermaid
classDiagram
  class ExampleInterface {
    ExampleImplementation
  }
  ExampleTypeWithInterfaceDeps --> ""2"" ExampleInterface
```";

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

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTransient<ExampleInterface, ExampleImplementation>();
            }
        }
    }
}
