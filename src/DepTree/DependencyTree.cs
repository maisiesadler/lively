using System;
using System.Collections.Generic;
using System.Reflection;

namespace DepTree
{
    public static class DependencyTree
    {
        public static DependencyTreeNode Create(DependencyTreeConfig config, string typeName, string name = "root")
             => GetDependencies(config, typeName, name);

        private static DependencyTreeNode GetDependencies(DependencyTreeConfig config, string typeName, string name, int depth = 0)
        {
            var type = config.Assembly.GetType(typeName);
            if (type == null)
            {
                return new DependencyTreeNode(name, type, DependencyTreeError.UnknownType);
            }

            if (type.IsInterface)
            {
                var implementation = config.InterfaceResolver.Resolve(type);

                if (implementation == null)
                {
                    return new DependencyTreeNode(name, type, DependencyTreeError.NoImplementation);
                }

                var (ch, err) = FindChildrenForType(config, implementation, depth);
                if (err != null)
                    return new DependencyTreeNode(name, type, implementation, err.Value);

                return new DependencyTreeNode(name, type, implementation, ch);
            }

            var (children, error) = FindChildrenForType(config, type, depth);
            if (error != null)
                return new DependencyTreeNode(name, type, error.Value);

            return new DependencyTreeNode(name, type, children);
        }

        private static (List<DependencyTreeNode>, DependencyTreeError?) FindChildrenForType(DependencyTreeConfig config, Type type, int depth)
        {
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (ctors.Length != 1)
            {
                return (null, DependencyTreeError.IncorrectConstructors);
            }

            var c = ctors[0];

            var children = new List<DependencyTreeNode>();
            foreach (var p in c.GetParameters())
            {
                var child = GetDependencies(config, p.ParameterType.FullName, p.Name, depth + 1);
                children.Add(child);
            }

            return (children, null);
        }
    }
}
