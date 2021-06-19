using System;
using System.Collections.Generic;
using System.Reflection;
using Lively.Resolvers;
using Lively.TypeDescriptions;

namespace Lively
{
    public class DependencyTree
    {
        private readonly int _maxDepth = 100;

        public IList<Assembly> Assemblies { get; }
        public IInterfaceResolver InterfaceResolver { get; }
        public HashSet<string> SkipTypes { get; set; }

        public DependencyTree(DependencyTreeConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            Assemblies = config.Assemblies ?? throw new ArgumentNullException(nameof(config.Assemblies));
            SkipTypes = config.SkipTypes;
            InterfaceResolver = config.CreateInterfaceResolver == null
                ? NoInterfaceResolver.Create()
                : config.CreateInterfaceResolver(config);
        }

        public DependencyTreeNode GetDependencies(string typeName, string name = "root")
        {
            return GetDependencies(typeName, name, 0);
        }

        private DependencyTreeNode GetDependencies(string typeName, string name, int depth)
        {
            if (!TryFindType(typeName, out var type))
            {
                var td = new UnknownTypeDescription(typeName);
                return new DependencyTreeNode(name, td, DependencyTreeError.UnknownType);
            }
            var typeDescription = new ConcreteTypeDescription(type);

            if (depth > _maxDepth)
            {
                return new DependencyTreeNode(name, typeDescription, DependencyTreeError.TooManyLayers);
            }

            if (type.IsInterface)
            {
                var implementation = InterfaceResolver.Resolve(type);

                if (implementation == null)
                {
                    return new DependencyTreeNode(name, typeDescription, DependencyTreeError.NoImplementation);
                }

                var implTypeDescription = new ConcreteTypeDescription(implementation);

                var (ch, err) = FindChildrenForType(implementation, depth);
                if (err != null)
                    return new DependencyTreeNode(name, typeDescription, implTypeDescription, err.Value);

                return new DependencyTreeNode(name, typeDescription, implTypeDescription, ch);
            }

            var (children, error) = FindChildrenForType(type, depth);
            if (error != null)
                return new DependencyTreeNode(name, typeDescription, error.Value);

            return new DependencyTreeNode(name, typeDescription, children);
        }

        private (List<DependencyTreeNode>, DependencyTreeError?) FindChildrenForType(Type type, int depth)
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
                var nonGenericType = NonGenericTypeName(p.ParameterType);
                if (SkipTypes?.Contains(nonGenericType.FullName) == true) continue;

                var child = GetDependencies(p.ParameterType.FullName, p.Name, depth + 1);
                children.Add(child);
            }

            return (children, null);
        }

        private bool TryFindType(string typeName, out Type type)
        {
            foreach (var assembly in Assemblies)
            {
                type = assembly.GetType(typeName);
                if (type != null) return true;
            }

            type = null;
            return false;
        }

        private static Type NonGenericTypeName(Type type)
        {
            return type.IsGenericType
                ? type.GetGenericTypeDefinition()
                : type;
        }
    }
}
