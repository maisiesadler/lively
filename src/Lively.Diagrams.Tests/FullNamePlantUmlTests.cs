using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lively.Diagrams.Tests
{
    public class FullNamePlantUmlTests
    {
        private static Regex _whitespace = new Regex("[\r\n]+");

        [Fact]
        public void CanCreateDiagramForSimpleDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree });

            var expected = @"@startuml

class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithDeps {
}
Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithDeps ---> Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleType

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementation()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.PlantUmlTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.PlantUmlTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree });

            var expected = @"@startuml
class Lively.Diagrams.Tests.PlantUmlTests.ExampleTypeWithInterfaceDeps {
}
Lively.Diagrams.Tests.PlantUmlTests.ExampleTypeWithInterfaceDeps ---> Lively.Diagrams.Tests.PlantUmlTests.ExampleInterface

interface Lively.Diagrams.Tests.PlantUmlTests.ExampleInterface {
}
class Lively.Diagrams.Tests.PlantUmlTests.ExampleImplementation {
}
Lively.Diagrams.Tests.PlantUmlTests.ExampleInterface <--- Lively.Diagrams.Tests.PlantUmlTests.ExampleImplementation

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void MultipleRegistrationsDedupedAndNoted()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.PlantUmlTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.PlantUmlTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree, depTree });

            var expected = @"@startuml

class Lively.Diagrams.Tests.PlantUmlTests.ExampleTypeWithInterfaceDeps {
}
Lively.Diagrams.Tests.PlantUmlTests.ExampleTypeWithInterfaceDeps ---> ""2"" Lively.Diagrams.Tests.PlantUmlTests.ExampleInterface

interface Lively.Diagrams.Tests.PlantUmlTests.ExampleInterface {
}
class Lively.Diagrams.Tests.PlantUmlTests.ExampleImplementation {
}
Lively.Diagrams.Tests.PlantUmlTests.ExampleInterface <--- Lively.Diagrams.Tests.PlantUmlTests.ExampleImplementation

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForGenericDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Diagrams.Tests.PlantUmlTests+ExampleTypeWithGenericDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree });

            var expected = @"@startuml

class Lively.Diagrams.Tests.PlantUmlTests.ExampleTypeWithGenericDeps {
}
Lively.Diagrams.Tests.PlantUmlTests.ExampleTypeWithGenericDeps ---> Lively.Diagrams.Tests.PlantUmlTests.ExampleGenericType2

@enduml";

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
