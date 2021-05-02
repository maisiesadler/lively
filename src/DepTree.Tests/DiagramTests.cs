using DepTree.Diagrams;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DepTree.Tests
{
    public class DiagramTests
    {
        [Fact]
        public void CanCreateDiagramForSimpleDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "DepTree.Tests.DiagramTests+ExampleTypeWithDeps";

            var depTree = DependencyTree.Create(config, fullTypeName);
            var diagram = yUML.Create(depTree);

            var expected = @"// {type:class}
// {direction:topDown}
// {generate:true}

[ExampleTypeWithDeps]->[ExampleType]";

            Assert.Equal(expected.Trim(), diagram.Trim());
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
