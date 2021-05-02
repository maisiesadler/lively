using System.Collections.Generic;
using System.Text;

namespace DepTree.Diagrams
{
    public class yUML
    {
        public static string Create(params DependencyTreeNode[] nodes)
        {
            var builder = new StringBuilder(@"// {type:class}
// {direction:topDown}
// {generate:true}

");

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
                        builder.AppendLine($"[{nodeName}]->[{childname}]");
                    }
                    else
                    {
                        builder.AppendLine($"[{nodeName}]-{count}>[{childname}]");
                    }
                }
            }

            return builder.ToString();
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
