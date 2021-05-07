using System.Collections.Generic;
using System.Text;

namespace DepTree.Diagrams
{
    public class Mermaid
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder("classDiagram");

            var relationships = new Dictionary<string, Dictionary<string, int>>();
            var implementations = new Dictionary<string, string>();
            foreach (var node in nodes)
            {
                AddAllNodes(relationships, implementations, node);
            }

            foreach (var (nodeName, children) in relationships)
            {
                foreach (var (childname, count) in children)
                {
                    if (count == 1)
                    {
                        builder.AppendLine($"  {nodeName} --> {childname}");
                    }
                    else
                    {
                        builder.AppendLine($"  {nodeName} --> \"{count}\" {childname}");
                    }
                }
            }

            foreach (var (@interface, implementation) in implementations)
            {
                builder.AppendLine($"  class {@interface} {{");
                builder.AppendLine($"    {implementation}");
                builder.AppendLine("   }");
            }

            return builder.ToString();
        }

        private static void AddAllNodes(
            Dictionary<string, Dictionary<string, int>> relationships,
            Dictionary<string, string> implementations,
            DependencyTreeNode node)
        {
            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                var childname = child.Type?.Name;
                if (child.Implementation != null)
                {
                    implementations.TryAdd(childname, child.Implementation.Name);
                }

                var nodename = node.Type?.Name;
                if (!relationships.ContainsKey(nodename))
                    relationships[nodename] = new Dictionary<string, int>();

                var nodeRelationships = relationships[nodename];

                if (!nodeRelationships.ContainsKey(childname))
                    nodeRelationships[childname] = 0;

                nodeRelationships[childname]++;

                AddAllNodes(relationships, implementations, child);
            }
        }
    }
}
