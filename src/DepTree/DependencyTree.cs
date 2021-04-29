using System;
using System.Collections.Generic;
using System.Reflection;

namespace DepTree
{
    public static class DependencyTree
    {
        public static DependencyTreeNode Create(Assembly assembly, string typeName, string name = "root")
             => GetDependencies(assembly, typeName, name);

        private static DependencyTreeNode GetDependencies(Assembly assembly, string typeName, string name, int depth = 0)
        {
            var type = assembly.GetType(typeName);
            if (type == null)
            {
                return new DependencyTreeNode(name, type, DependencyTreeError.UnknownType);
            }

            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (ctors.Length != 1)
            {
                return new DependencyTreeNode(name, type, DependencyTreeError.IncorrectConstructors);
            }

            var c = ctors[0];

            var children = new List<DependencyTreeNode>();
            foreach (var p in c.GetParameters())
            {
                var child = GetDependencies(assembly, p.ParameterType.FullName, p.Name, depth + 1);
                children.Add(child);
            }

            return new DependencyTreeNode(name, type, children);
        }
    }
}
