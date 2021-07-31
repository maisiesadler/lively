using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lively.Diagrams.Tests
{
    public class PlantUmlTests
    {
        private static Regex _whitespace = new Regex("[\r\n]+");

        [Fact]
        public void CanCreateDiagramForSimpleDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Diagrams.Tests.PlantUmlTests+ExampleTypeWithDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = PlantUml.Create(new[] { depTree });

            var expected = @"@startuml

class ExampleTypeWithDeps {
}
class ExampleType {
}
ExampleTypeWithDeps ---> ExampleType

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
            var diagram = PlantUml.Create(new[] { depTree });

            var expected = @"@startuml

class ExampleTypeWithInterfaceDeps {
}
interface ExampleInterface {
}
class ExampleImplementation {
}
ExampleInterface <--- ExampleImplementation

ExampleTypeWithInterfaceDeps ---> ExampleInterface

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
            var diagram = PlantUml.Create(new[] { depTree, depTree });

            var expected = @"@startuml

class ExampleTypeWithInterfaceDeps {
}
interface ExampleInterface {
}
class ExampleImplementation {
}
ExampleInterface <--- ExampleImplementation

ExampleTypeWithInterfaceDeps ---> ""2"" ExampleInterface

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
            var diagram = PlantUml.Create(new[] { depTree });

            var expected = @"@startuml

class ExampleTypeWithGenericDeps {
}
class ExampleGenericType2 {
}
ExampleTypeWithGenericDeps ---> ExampleGenericType2

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementationAndMethods()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.PlantUmlTests+Startup",
            };
            var fullTypeName = typeof(ExampleInterfaceWithMethods).FullName;

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            var diagram = PlantUml.Create(new[] { depTree });

            var expected = @"@startuml
interface ExampleInterfaceWithMethods {
}
class ExampleImplementationWithMethods {
  +One()
  +Beans()
}

ExampleInterfaceWithMethods <--- ExampleImplementationWithMethods

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementationAndDependencies()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.PlantUmlTests+Startup",
            };
            var fullTypeName = typeof(ExampleInterfaceWithDependencies).FullName;

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            var diagram = PlantUml.Create(new[] { depTree });

            var expected = @"@startuml

interface ExampleInterfaceWithDependencies {
}
class ExampleImplementationWithDependencies {
}
ExampleInterfaceWithDependencies <--- ExampleImplementationWithDependencies
class ExampleType {
}

ExampleImplementationWithDependencies ---> ExampleType

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

        public interface ExampleInterfaceWithMethods
        {
            void One();
            string Beans();
        }

        public class ExampleImplementationWithMethods : ExampleInterfaceWithMethods
        {
            public void One() { }
            public string Beans() => "hello";
        }

        public interface ExampleInterfaceWithDependencies
        {
        }

        public class ExampleImplementationWithDependencies : ExampleInterfaceWithDependencies
        {
            public ExampleImplementationWithDependencies(ExampleType exampleType) { }
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTransient<ExampleInterface, ExampleImplementation>();
                services.AddTransient<ExampleInterfaceWithMethods, ExampleImplementationWithMethods>();
                services.AddTransient<ExampleInterfaceWithDependencies, ExampleImplementationWithDependencies>();
            }
        }
    }
}
