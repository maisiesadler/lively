using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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

            if (type.IsInterface)
            {
                var implementation = FindImplementation(assembly, type);

                if (implementation == null)
                {
                    return new DependencyTreeNode(name, type, DependencyTreeError.NoImplementation);
                }

                var (ch, err) = FindChildrenForType(assembly, implementation, depth);
                if (err != null)
                    return new DependencyTreeNode(name, type, implementation, err.Value);

                return new DependencyTreeNode(name, type, implementation, ch);
            }

            var (children, error) = FindChildrenForType(assembly, type, depth);
            if (error != null)
                return new DependencyTreeNode(name, type, error.Value);

            return new DependencyTreeNode(name, type, children);
        }

        private static Type FindImplementation(Assembly assembly, Type type)
        {
            var startupType = assembly.GetTypes().FirstOrDefault(x => x.Name == "Startup");
            if (startupType == null)
            {
                return null;
            }

            var startup = Activator.CreateInstance(startupType);
            MethodInfo method = startupType.GetMethod("ConfigureServices");
            var serviceCollection = new ServiceCollection();
            method.Invoke(startup, new[] { serviceCollection });

            var sp = serviceCollection.BuildServiceProvider();
            var s = sp.GetService(type);

            if (s == null)
            {
                throw new Exception($"Could not find implementation of {type}");
            }

            return s.GetType();
        }

        private static (List<DependencyTreeNode>, DependencyTreeError?) FindChildrenForType(Assembly assembly, Type type, int depth)
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
                var child = GetDependencies(assembly, p.ParameterType.FullName, p.Name, depth + 1);
                children.Add(child);
            }

            return (children, null);
        }
    }
}
