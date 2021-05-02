using System.Collections.Generic;
using DepTree.TypeDescriptions;

namespace DepTree
{
    public class DependencyTreeNode
    {
        public string Name { get; }
        public ITypeDescription Type { get; }
        public ITypeDescription Implementation { get; }
        public IList<DependencyTreeNode> Children { get; }
        public DependencyTreeError? Error { get; }

        public DependencyTreeNode(string name, ITypeDescription type, IList<DependencyTreeNode> children)
            => (Name, Type, Children) = (name, type, children);

        public DependencyTreeNode(string name, ITypeDescription type, DependencyTreeError error)
            => (Name, Type, Error) = (name, type, error);

        public DependencyTreeNode(string name, ITypeDescription type, ITypeDescription implementation, IList<DependencyTreeNode> children)
            => (Name, Type, Implementation, Children) = (name, type, implementation, children);

        public DependencyTreeNode(string name, ITypeDescription type, ITypeDescription implementation, DependencyTreeError error)
            => (Name, Type, Implementation, Error) = (name, type, implementation, error);
    }
}
