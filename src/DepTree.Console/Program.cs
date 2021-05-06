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
            // System.Console.WriteLine(string.Join(", ", args));
            var envProvider = new EnvironmentVariableProvider();
            var (applicationConfig, ok) = ApplicationConfiguration.Build(envProvider, args);
            if (!ok)
            {
                System.Console.WriteLine($"Application Config is not valid. {string.Join(", ", applicationConfig.Errors)}.");
                return -1;
            }

            // System.Console.WriteLine(JsonSerializer.Serialize(applicationConfig));

            var assembly = Assembly.LoadFrom(applicationConfig.AssemblyLocation);
            var config = new DependencyTreeConfig(assembly, applicationConfig.AssemblyConfiguration)
            {
                InterfaceResolverType = applicationConfig.InterfaceResolverType,
                SkipAssemblies = applicationConfig.Skip,
                StartupName = applicationConfig.StartupName,
            };

            var nodes = new List<DependencyTreeNode>();
            var tree = new DependencyTree(config);
            foreach (var classname in applicationConfig.Generate)
            {
                var node = tree.GetDependencies(classname);
                nodes.Add(node);
            }

            if (applicationConfig.OutputFormat == OutputFormatType.YumlMd)
            {
                var diagram = yUMLmd.Create(nodes);
                System.Console.WriteLine(diagram);
            }
            else if (applicationConfig.OutputFormat == OutputFormatType.Yuml)
            {
                var diagram = yUML.Create(nodes);
                System.Console.WriteLine(diagram);
            }
            else if (applicationConfig.OutputFormat == OutputFormatType.Debug)
            {
                System.Console.WriteLine($"Got {nodes?.Count} nodes");
                foreach (var node in nodes)
                {
                    Print(node);
                }
            }
            else
            {
                System.Console.WriteLine($"OutputFormat '{applicationConfig.OutputFormat}' unknown");
            }

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
