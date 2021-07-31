using System;
using System.Collections.Generic;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    internal record Relationship(ITypeDescription Type, ITypeDescription ChildType, int Count);

    internal record TypeAndImplementation(ITypeDescription NodeType, ITypeDescription ImplementationType);

    internal class FlattenedNodes
    {
        private readonly Dictionary<string, Dictionary<string, int>> _relationships = new Dictionary<string, Dictionary<string, int>>();
        private readonly Dictionary<string, ITypeDescription> _typeDescriptions = new Dictionary<string, ITypeDescription>();
        private readonly Dictionary<string, ITypeDescription> _implementations = new Dictionary<string, ITypeDescription>();

        private FlattenedNodes() { }

        public IEnumerable<Relationship> Relationships()
        {
            foreach (var (nodeFullName, children) in _relationships)
            {
                var node = _typeDescriptions[nodeFullName];

                if (_implementations.TryGetValue(nodeFullName, out var nodeImplementation))
                    node = nodeImplementation;

                foreach (var (childFullName, count) in children)
                {
                    if (!_typeDescriptions.TryGetValue(childFullName, out var childType))
                        throw new InvalidOperationException($"Could not find child type {childFullName}");

                    yield return new Relationship(node, childType, count);
                }
            }
        }

        public ITypeDescription Implementation(ITypeDescription type)
        {
            _implementations.TryGetValue(type.FullName, out var nodeImplementation);
            return nodeImplementation;
        }

        public IEnumerable<TypeAndImplementation> TypesAndImplementations()
        {
            foreach (var (nodeFullName, nodeType) in _typeDescriptions)
            {
                _implementations.TryGetValue(nodeFullName, out var nodeImplementation);
                yield return new TypeAndImplementation(nodeType, nodeImplementation);
            }
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
            var nodeFullName = node.Type.FullName;
            _typeDescriptions.TryAdd(nodeFullName, node.Type);
            if (node.Implementation != null)
            {
                _implementations.TryAdd(nodeFullName, node.Implementation);
            }

            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                var childname = child.Type?.FullName;

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
