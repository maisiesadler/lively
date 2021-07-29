using System.Collections.Generic;
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

            var (relationships, types, implementations) = FlattenedNodes.Create(nodes);

            foreach (var (nodeFullName, children) in relationships)
            {
                var node = types[nodeFullName];
                AppendClass(builder, node);
                foreach (var (childname, count) in children)
                {
                    if (count == 1)
                    {
                        builder.AppendLine($"{nodeFullName} ---> {childname}");
                    }
                    else
                    {
                        builder.AppendLine($"{nodeFullName} ---> \"{count}\" {childname}");
                    }
                }
            }

            if (implementations.Count > 0)
            {
                builder.AppendLine();
            }

            foreach (var (interfaceFullName, implementation) in implementations)
            {
                builder.AppendLine($"interface {interfaceFullName} {{");
                builder.AppendLine($"  {implementation.Name}");
                builder.AppendLine("}");

                AppendClass(builder, implementation);

                builder.AppendLine($"{implementation.FullName} ---> {interfaceFullName}");
            }

            builder.AppendLine();
            builder.AppendLine("@enduml");

            return builder.ToString();
        }

        private static void AppendClass(StringBuilder builder, ITypeDescription typeDescription)
        {
            builder.AppendLine("class " + typeDescription.FullName + " {");
            foreach (var method in typeDescription.Methods)
                builder.AppendLine($"  +{method.Name}()");
            builder.AppendLine("}");
        }
    }
}
