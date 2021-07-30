using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    public class FullNamePlantUml
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("@startuml");
            builder.AppendLine();

            var flattenedNodes = FlattenedNodes.Create(nodes);

            foreach (var (nodeType, implementationType, children) in flattenedNodes.Relationships())
            {
                var nodeFullName = NormalisePlantUml(nodeType.FullName);
                AppendClass(builder, nodeFullName, nodeType.Methods);
                foreach (var (childType, childImplementationType, count) in children)
                {
                    var childFullName = NormalisePlantUml(childType.FullName);
                    if (count == 1)
                    {
                        builder.AppendLine($"{nodeFullName} ---> {childFullName}");
                    }
                    else
                    {
                        builder.AppendLine($"{nodeFullName} ---> \"{count}\" {childFullName}");
                    }
                }
            }

            var implementations = flattenedNodes.Implementations().ToList();

            if (implementations.Count > 0)
            {
                builder.AppendLine();
            }

            foreach (var (interfaceType, implementationType) in implementations)
            {
                var interfaceFullName = NormalisePlantUml(interfaceType.FullName);
                builder.AppendLine($"interface {interfaceFullName} {{");
                builder.AppendLine("}");

                var implementationFullName = NormalisePlantUml(implementationType.FullName);
                AppendClass(builder, implementationFullName, implementationType.Methods);

                builder.AppendLine($"{interfaceFullName} <--- {implementationFullName}");
            }

            builder.AppendLine();
            builder.AppendLine("@enduml");

            return builder.ToString();
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
