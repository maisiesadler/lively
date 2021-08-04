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
class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleType {
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
                StartupName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree });

            var expected = @"@startuml

class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithInterfaceDeps {
}
interface Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterface {
}
class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleImplementation {
}
Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterface <--- Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleImplementation

Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithInterfaceDeps ---> Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterface

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void MultipleRegistrationsDedupedAndNoted()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+Startup",
            };
            var fullTypeName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+ExampleTypeWithInterfaceDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree, depTree });

            var expected = @"@startuml

class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithInterfaceDeps {
}
interface Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterface {
}
class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleImplementation {
}
Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterface <--- Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleImplementation

Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithInterfaceDeps ---> ""2"" Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterface

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForGenericDependency()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+ExampleTypeWithGenericDeps";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree });

            var expected = @"@startuml

class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithGenericDeps {
}
class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleGenericType2 {
}
Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleTypeWithGenericDeps ---> Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleGenericType2

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanCreateDiagramForDependencyWithImplementationAndMethods()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly)
            {
                StartupName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+Startup",
            };
            var fullTypeName = typeof(ExampleInterfaceWithMethods).FullName;

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);

            var diagram = FullNamePlantUml.Create(new[] { depTree });

            var expected = @"@startuml
interface Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterfaceWithMethods {
  +One()
  +Beans()
}
class Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleImplementationWithMethods {
  +One()
  +Beans()
}

Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleInterfaceWithMethods <--- Lively.Diagrams.Tests.FullNamePlantUmlTests.ExampleImplementationWithMethods

@enduml";

            Assert.Equal(Normalise(expected), Normalise(diagram));
        }

        [Fact]
        public void CanApplyCustomNamespaceGrouping()
        {
            var assembly = this.GetType().Assembly;
            var config = new DependencyTreeConfig(assembly);
            var fullTypeName = "Lively.Diagrams.Tests.FullNamePlantUmlTests+ExampleTypeReferencingOtherAssemblies";

            var tree = new DependencyTree(config);
            var depTree = tree.GetDependencies(fullTypeName);
            var diagram = FullNamePlantUml.Create(new[] { depTree }, new[] { "Lively.Diagrams.Tests", "Lively.Diagrams" });

            var expected = @"@startuml

class Lively.Diagrams.Tests.FullNamePlantUmlTests_ExampleTypeReferencingOtherAssemblies {
}
class Lively.Diagrams.FullNamePlantUml {
}

Lively.Diagrams.Tests.FullNamePlantUmlTests_ExampleTypeReferencingOtherAssemblies ---> Lively.Diagrams.FullNamePlantUml

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

        public class ExampleTypeReferencingOtherAssemblies
        {
            public ExampleTypeReferencingOtherAssemblies(FullNamePlantUml fullNamePlantUml) { }
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTransient<ExampleInterface, ExampleImplementation>();
                services.AddTransient<ExampleInterfaceWithMethods, ExampleImplementationWithMethods>();
            }
        }
    }
}
