using System;
using System.Collections.Generic;
using System.Reflection;
using DepTree.TypeDescriptions;

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
                var td = new UnknownTypeDescription(typeName);
                return new DependencyTreeNode(name, td, DependencyTreeError.UnknownType);
            }
            var typeDescription = new ConcreteTypeDescription(type);

            if (type.IsInterface)
            {
                var implementation = config.InterfaceResolver.Resolve(type);

                if (implementation == null)
                {
                    return new DependencyTreeNode(name, typeDescription, DependencyTreeError.NoImplementation);
                }

                var implTypeDescription = new ConcreteTypeDescription(implementation);

                var (ch, err) = FindChildrenForType(config, implementation, depth);
                if (err != null)
                    return new DependencyTreeNode(name, typeDescription, implTypeDescription, err.Value);

                return new DependencyTreeNode(name, typeDescription, implTypeDescription, ch);
            }

            var (children, error) = FindChildrenForType(config, type, depth);
            if (error != null)
                return new DependencyTreeNode(name, typeDescription, error.Value);

            return new DependencyTreeNode(name, typeDescription, children);
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
