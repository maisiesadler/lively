using System.Collections.Generic;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    internal record Relationship(string NodeName, string NodeFullName, IReadOnlyList<TypeMethod> Methods, IReadOnlyList<RelationshipChild> Children);
    internal record RelationshipChild(string NodeName, string NodeFullName, string NodePlusImplementation, int Count);

    internal record Implementation(string InterfaceName, string InterfaceFullName, ITypeDescription ImplementationType);

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
                var node= _typeDescriptions[nodeFullName];
                var relChildren = new List<RelationshipChild>();
                foreach (var (childFullName, count) in children)
                {
                    var childName = _typeDescriptions.TryGetValue(childFullName, out var childType)
                        ? childType.Name
                        : childFullName;

                    var nodePlusImpl = _implementations.TryGetValue(childFullName, out var implementation)
                        ? $"{childName}|{implementation.Name}"
                        : childName;

                    relChildren.Add(new RelationshipChild(childName, childFullName, nodePlusImpl, count));
                }

                yield return new Relationship(node.Name, nodeFullName, node.Methods, relChildren);
            }
        }

        public IEnumerable<Implementation> Implementations()
        {
            foreach (var (interfaceFullName, implementationType) in _implementations)
            {
                var interfaceName = _typeDescriptions.TryGetValue(interfaceFullName, out var interfaceType)
                    ? interfaceType.Name
                    : interfaceFullName;
                yield return new Implementation(interfaceName, interfaceFullName, implementationType);
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
