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

            foreach (var (nodeName, nodeFullName, methods, children) in flattenedNodes.Relationships())
            {
                AppendClass(builder, nodeName, methods);
                foreach (var (childname, childFullName, childPlusImpl, count) in children)
                {
                    if (count == 1)
                    {
                        builder.AppendLine($"{nodeName} ---> {childname}");
                    }
                    else
                    {
                        builder.AppendLine($"{nodeName} ---> \"{count}\" {childname}");
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
                builder.AppendLine($"interface {interfaceName} {{");
                builder.AppendLine("}");

                AppendClass(builder, implementation.Name, implementation.Methods);

                builder.AppendLine($"{interfaceName} <--- {implementation.Name}");
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
    }
}
