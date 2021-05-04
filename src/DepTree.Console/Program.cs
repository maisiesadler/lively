using System.Collections.Generic;
using System.Reflection;
using DepTree.Diagrams;
using DepTree.Console.Configuration;

namespace DepTree.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var (applicationConfig, ok) = ApplicationConfiguration.Build(args);
            if (!ok)
            {
                System.Console.WriteLine($"Application Config is not valid. {string.Join(", ", applicationConfig.Errors)}.");
                return -1;
            }

            var assembly = Assembly.LoadFrom(applicationConfig.AssemblyLocation);
            var config = new DependencyTreeConfig(assembly, applicationConfig.AssemblyConfiguration, skipAssemblies: applicationConfig.Skip);
            if (applicationConfig.InterfaceResolver == "None")
                config.InterfaceResolverType = Resolvers.InterfaceResolverType.None;

            var nodes = new List<DependencyTreeNode>();
            var tree = new DependencyTree(config);
            foreach (var classname in applicationConfig.Generate)
            {
                var node = tree.GetDependencies(classname);
                nodes.Add(node);
            }

            var diagram = yUMLmd.Create(nodes);
            System.Console.WriteLine(diagram);

            // for debugging
            // Print(nodes[0]);

            return 0;
        }

        private static void Print(DependencyTreeNode node, string indent = "")
        {
            System.Console.WriteLine($"{indent}{node.Name}");
            if (!string.IsNullOrWhiteSpace(node.Type?.FullName))
                System.Console.WriteLine($"{indent} - {node.Type?.FullName}");
            if (!string.IsNullOrWhiteSpace(node.Implementation?.FullName))
                System.Console.WriteLine($"{indent} - Implementation: {node.Implementation?.FullName}");
            if (node.Error != null)
            {
                System.Console.WriteLine($"{indent} Error - {node.Error}");
                return;
            }
            var childindent = "  " + indent;
            foreach (var c in node.Children)
            {
                Print(c, childindent);
            }
        }
    }
}
