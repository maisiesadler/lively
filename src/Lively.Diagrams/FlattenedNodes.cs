using System.Collections.Generic;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    internal class FlattenedNodes
    {
        private readonly Dictionary<string, Dictionary<string, int>> _relationships = new Dictionary<string, Dictionary<string, int>>();
        private readonly Dictionary<string, ITypeDescription> _typeDescriptions = new Dictionary<string, ITypeDescription>();
        private readonly Dictionary<string, ITypeDescription> _implementations = new Dictionary<string, ITypeDescription>();

        private FlattenedNodes() { }

        public void Deconstruct(
            out IReadOnlyDictionary<string, Dictionary<string, int>> relationships,
            out IReadOnlyDictionary<string, ITypeDescription> typeDescriptions,
            out IReadOnlyDictionary<string, ITypeDescription> implementations)
        {
            relationships = _relationships;
            typeDescriptions = _typeDescriptions;
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
                var childname = child.Type?.FullName;

                _typeDescriptions.TryAdd(childname, child.Type);
                if (child.Implementation != null)
                {
                    _implementations.TryAdd(childname, child.Implementation);
                }

                var nodename = node.Type?.FullName;
                _typeDescriptions.TryAdd(nodename, node.Type);

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
