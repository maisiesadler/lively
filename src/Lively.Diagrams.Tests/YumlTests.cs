using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lively.Diagrams.Tests
{
    public class YumlTests
    {
        private static Regex _whitespace = new Regex("[\r\n]+");

        [Fact]
        public void CanCreateDiagramForSimpleDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Diagrams.Tests.YumlTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = yUML.Create(new[] { depTree });

            var expected = @"// {type:class}
// {direction:topDown}
// {generate:true}

[ExampleTypeWithDeps]->[ExampleType]";

            Assert.Equal(expected.Trim(), diagram.Trim());
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementation()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.YumlTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.YumlTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = yUML.Create(new[] { depTree });

            var expected = @"// {type:class}
// {direction:topDown}
// {generate:true}

[ExampleTypeWithInterfaceDeps]->[ExampleInterface|ExampleImplementation]";

            Assert.Equal(expected.Trim(), diagram.Trim());
        }

        [Fact]
        public void MultipleRegistrationsDedupedAndNoted()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.YumlTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.YumlTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = yUML.Create(new[] { depTree, depTree });

            var expected = @"// {type:class}
// {direction:topDown}
// {generate:true}

[ExampleTypeWithInterfaceDeps]-2>[ExampleInterface|ExampleImplementation]
";

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
