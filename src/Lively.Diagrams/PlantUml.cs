using System.Collections.Generic;
using System.Text;

namespace Lively.Diagrams
{
    public class PlantUml
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("@startuml");
            builder.AppendLine();

            var (relationships, implementations) = FlattenedNodes.Create(nodes);

            foreach (var (nodeName, children) in relationships)
            {
                builder.AppendLine($"class {nodeName}");
                foreach (var (childname, count) in children)
                {
                    if (count == 1)
                    {
                        builder.AppendLine($"{nodeName} ---> {childname}");
                    }
                    else
                    {
                        builder.AppendLine($"{nodeName} ---> \"2\" {childname}");
                    }
                }
            }

            if (implementations.Count > 0)
            {
                builder.AppendLine();
            }

            foreach (var (@interface, implementation) in implementations)
            {
                builder.AppendLine($"interface {@interface} {{");
                builder.AppendLine($"  {implementation}");
                builder.AppendLine("}");
            }

            builder.AppendLine();
            builder.AppendLine("@enduml");

            return builder.ToString();
        }
    }
}
