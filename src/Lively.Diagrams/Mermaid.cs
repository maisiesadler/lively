using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Lively.TypeDescriptions;

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

            foreach (var (nodeType, implementationType) in flattenedNodes.TypesAndImplementations())
            {
                if (implementationType != null)
                    AppendImplementation(builder, nodeType, implementationType);
            }

            foreach (var (nodeType, childType, count) in flattenedNodes.Relationships())
            {
                var implementationType = flattenedNodes.Implementation(nodeType);
                if (implementationType != null)
                    AppendImplementation(builder, nodeType, implementationType);

                var nodeName = _invalidMermaidChars.Replace(nodeType.Name, "");
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

            return builder.ToString();
        }

        private static void AppendImplementation(StringBuilder builder, ITypeDescription nodeType, ITypeDescription implementationType)
        {
            builder.AppendLine($"  class {nodeType.Name} {{");
            builder.AppendLine($"    {implementationType.Name}");
            builder.AppendLine("  }");
        }
    }
}
