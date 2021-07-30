using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lively.Diagrams
{
    public class Mermaid
    {
        private static Regex _invalidMermaidChars = new Regex("`");

        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("classDiagram");

            var flattenedNodes = FlattenedNodes.Create(nodes);

            foreach (var (nodeType, implementationType, children) in flattenedNodes.Relationships())
            {
                var nodeName = _invalidMermaidChars.Replace(nodeType.Name, "");
                foreach (var (childType, childImplementationType, count) in children)
                {
                    var childName = _invalidMermaidChars.Replace(childType.Name, "");
                    if (count == 1)
                    {
                        builder.AppendLine($"  {nodeName} --> {childName}");
                    }
                    else
                    {
                        builder.AppendLine($"  {nodeName} --> \"{count}\" {childName}");
                    }
                }
            }

            foreach (var (interfaceType, implementationType) in flattenedNodes.Implementations())
            {
                builder.AppendLine($"  class {interfaceType.Name} {{");
                builder.AppendLine($"    {implementationType.Name}");
                builder.AppendLine("  }");
            }

            return builder.ToString();
        }
    }
}
