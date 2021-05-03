using System.Collections.Generic;
using System.Text;

namespace DepTree.Diagrams
{
    public class yUMLmd
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new List<string>();

            var relationships = new Dictionary<string, Dictionary<string, int>>();
            foreach (var node in nodes)
            {
                AddAllNodes(relationships, node);
            }

            foreach (var (nodeName, children) in relationships)
            {
                foreach (var (childname, count) in children)
                {
                    if (count == 1)
                    {
                        builder.Add($"[{nodeName}]->[{childname}]");
                    }
                    else
                    {
                        builder.Add($"[{nodeName}]-{count}>[{childname}]");
                    }
                }
            }

            var img = string.Join(", ", builder);
            return $"![](http://yuml.me/diagram/scruffy/class/{img} \"yUML\")";
        }

        private static void AddAllNodes(Dictionary<string, Dictionary<string, int>> relationships, DependencyTreeNode node)
        {
            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                var childname = child.Type?.Name;
                if (child.Implementation != null) childname += "|" + child.Implementation.Name;
                var nodename = node.Type?.Name;
                if (!relationships.ContainsKey(nodename))
                    relationships[nodename] = new Dictionary<string, int>();

                var nodeRelationships = relationships[nodename];

                if (!nodeRelationships.ContainsKey(childname))
                    nodeRelationships[childname] = 0;

                nodeRelationships[childname]++;

                AddAllNodes(relationships, child);
            }
        }
    }
}
