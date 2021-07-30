using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    public class PlantUml
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("@startuml");
            builder.AppendLine();

            var flattenedNodes = FlattenedNodes.Create(nodes);

            foreach (var (nodeType, implementationType, children) in flattenedNodes.Relationships())
            {
                AppendClass(builder, nodeType.Name, nodeType.Methods);
                foreach (var (childType, childImplementationType, count) in children)
                {
                    AppendDependentRelationship(builder, nodeType.Name, childType.Name, count);
                }
            }

            var implementations = flattenedNodes.Implementations().ToList();

            if (implementations.Count > 0)
            {
                builder.AppendLine();
            }

            foreach (var (interfaceType, implementationType) in implementations)
            {
                AppendInterfaceImplementation(builder, interfaceType.Name, implementationType.Name, implementationType.Methods);
            }

            builder.AppendLine();
            builder.AppendLine("@enduml");

            return builder.ToString();
        }

        private static void AppendDependentRelationship(StringBuilder builder, string parentName, string childName, int count)
        {
            parentName = NormalisePlantUml(parentName);
            childName = NormalisePlantUml(childName);

            if (count == 1)
            {
                builder.AppendLine($"{parentName} ---> {childName}");
            }
            else
            {
                builder.AppendLine($"{parentName} ---> \"{count}\" {childName}");
            }
        }

        private static void AppendInterfaceImplementation(StringBuilder builder, string interfaceName, string implementationName, IReadOnlyList<TypeMethod> implementationMethods)
        {
            interfaceName = NormalisePlantUml(interfaceName);
            implementationName = NormalisePlantUml(implementationName);

            builder.AppendLine($"interface {interfaceName} {{");
            builder.AppendLine("}");

            AppendClass(builder, implementationName, implementationMethods);

            builder.AppendLine($"{interfaceName} <--- {implementationName}");
        }

        private static void AppendClass(StringBuilder builder, string nodeName, IReadOnlyList<TypeMethod> methods)
        {
            builder.AppendLine("class " + nodeName + " {");
            foreach (var method in methods)
                builder.AppendLine($"  +{method.Name}()");
            builder.AppendLine("}");
        }

        private static string NormalisePlantUml(string fullName)
            => fullName
                .Replace('+', '.')
                .Replace("`", "");
    }
}
