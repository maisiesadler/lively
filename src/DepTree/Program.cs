using System;
using System.Collections.Generic;
using System.Reflection;

namespace DepTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var ass = Assembly.LoadFrom("src/Example/bin/Debug/net5.0/Example.dll");

            var dt = GetDependencies(ass, "root", "Example.Class1");

            Print(dt);
        }

        private static void Print(DependencyTreeNode node, string indent = "")
        {
            System.Console.WriteLine($"{indent}{node.Name} - {node.Type.FullName}");
            var childindent = indent + "  ";
            foreach (var c in node.Children)
            {
                Print(c, childindent);
            }
        }

        private static DependencyTreeNode GetDependencies(Assembly assembly, string name, string typeName, int depth = 0)
        {
            var type = assembly.GetType(typeName);

            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (ctors.Length != 1)
            {
                throw new Exception($":( there are {ctors.Length} ctors");
            }

            var c = ctors[0];

            var children = new List<DependencyTreeNode>();
            foreach (var p in c.GetParameters())
            {
                var child = GetDependencies(assembly, p.Name, p.ParameterType.FullName, depth + 1);
                children.Add(child);
            }

            return new DependencyTreeNode(name, type, children);
        }
    }

    public class DependencyTreeNode
    {
        public string Name { get; }
        public Type Type { get; }
        public IList<DependencyTreeNode> Children { get; }

        public DependencyTreeNode(string name, Type type, IList<DependencyTreeNode> children)
            => (Name, Type, Children) = (name, type, children);
    }
}
