using System;
using System.Collections.Generic;

namespace DepTree
{
    public class DependencyTreeNode
    {
        public string Name { get; }
        public Type Type { get; }
        public Type Implementation { get; }
        public IList<DependencyTreeNode> Children { get; }
        public DependencyTreeError? Error { get; }

        public DependencyTreeNode(string name, Type type, IList<DependencyTreeNode> children)
            => (Name, Type, Children) = (name, type, children);

        public DependencyTreeNode(string name, Type type, DependencyTreeError error)
            => (Name, Type, Error) = (name, type, error);

        public DependencyTreeNode(string name, Type type, Type implementation, IList<DependencyTreeNode> children)
            => (Name, Type, Implementation, Children) = (name, type, implementation, children);

        public DependencyTreeNode(string name, Type type, Type implementation, DependencyTreeError error)
            => (Name, Type, Implementation, Error) = (name, type, implementation, error);
    }
}
