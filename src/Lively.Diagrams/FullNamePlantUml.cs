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

            foreach (var (nodeName, nodeFullName, methods, children) in flattenedNodes.Relationships())
            {
                var _nodeFullName = NormalisePlantUml(nodeFullName);
                AppendClass(builder, _nodeFullName, methods);
                foreach (var (childname, childFullName, childPlusImpl, count) in children)
                {
                    var _childFullName = NormalisePlantUml(childFullName);
                    if (count == 1)
                    {
                        builder.AppendLine($"{_nodeFullName} ---> {_childFullName}");
                    }
                    else
                    {
                        builder.AppendLine($"{_nodeFullName} ---> \"{count}\" {_childFullName}");
                    }
                }
            }

            var implementations = flattenedNodes.Implementations().ToList();

            if (implementations.Count > 0)
            {
                builder.AppendLine();
            }

            foreach (var (interfaceName, interfaceFullName, implementation) in implementations)
            {
                var _interfaceFullName = NormalisePlantUml(interfaceFullName);
                builder.AppendLine($"interface {_interfaceFullName} {{");
                builder.AppendLine("}");

                var implementationFullName = NormalisePlantUml(implementation.FullName);
                AppendClass(builder, implementationFullName, implementation.Methods);

                builder.AppendLine($"{_interfaceFullName} <--- {implementationFullName}");
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
