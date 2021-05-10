using System.Collections.Generic;

namespace DepTree.Diagrams
{
    internal class FlattenedNodes
    {
        private readonly Dictionary<string, Dictionary<string, int>> _relationships = new Dictionary<string, Dictionary<string, int>>();
        private readonly Dictionary<string, string> _implementations = new Dictionary<string, string>();

        private FlattenedNodes() { }

        public void Deconstruct(
            out IReadOnlyDictionary<string, Dictionary<string, int>> relationships,
            out IReadOnlyDictionary<string, string> implementations)
        {
            relationships = _relationships;
            implementations = _implementations;
        }

        public static FlattenedNodes Create(IList<DependencyTreeNode> nodes)
        {
            var flattenedNodes = new FlattenedNodes();

            foreach (var node in nodes)
            {
                flattenedNodes.AddDependencyTreeNode(node);
            }

            return flattenedNodes;
        }

        private void AddDependencyTreeNode(DependencyTreeNode node)
        {
            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                var childname = child.Type?.Name;
                if (child.Implementation != null)
                {
                    _implementations.TryAdd(childname, child.Implementation.Name);
                }

                var nodename = node.Type?.Name;
                if (!_relationships.ContainsKey(nodename))
                    _relationships[nodename] = new Dictionary<string, int>();

                var nodeRelationships = _relationships[nodename];

                if (!nodeRelationships.ContainsKey(childname))
                    nodeRelationships[childname] = 0;

                nodeRelationships[childname]++;

                AddDependencyTreeNode(child);
            }
        }
    }
}
