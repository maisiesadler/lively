using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using DepTree.Diagrams;
using Microsoft.Extensions.Configuration;

namespace DepTree.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var expectedArgs = "Expected <appconfig-location> <assembly-location> <assembly-config-location (optional)>.";
            if (args.Length != 2 && args.Length != 3)
            {
                System.Console.WriteLine($"Incorrect number of args. {expectedArgs}");
                return -1;
            }

            var configLocation = args[0];
            if (!File.Exists(configLocation))
            {
                System.Console.WriteLine($"Application Config location '{configLocation}' does not exist'. {expectedArgs}");
                return -1;
            }

            var (applicationConfig, ok) = ReadConfig(configLocation);
            if (!ok)
            {
                System.Console.WriteLine($"Application Config at '{configLocation}' not valid'. {expectedArgs}");
                return -1;
            }

            var assemblyLocation = args[1];
            if (!File.Exists(assemblyLocation))
            {
                System.Console.WriteLine($"Assembly location '{assemblyLocation}' does not exist'. {expectedArgs}");
                return -1;
            }

            var iconfiguration = TryBuildConfiguration(args);

            var ass = Assembly.LoadFrom(assemblyLocation);
            var config = new DependencyTreeConfig(ass, iconfiguration, skipAssemblies: applicationConfig.Skip);
            config.InterfaceResolverType = Resolvers.InterfaceResolverType.None;

            var nodes = new List<DependencyTreeNode>();
            var tree = new DependencyTree(config);
            foreach (var classname in applicationConfig.Generate)
            {
                var node = tree.GetDependencies(classname);
                nodes.Add(node);
            }

            var diagram = yUMLmd.Create(nodes);
            File.WriteAllText("DependencyTree.md", diagram);

            // for debugging
            // Print(nodes[0]);

            return 0;
        }

        private static IConfiguration TryBuildConfiguration(string[] args)
        {
            if (args.Length > 2
              && args[2] is string configLocation
              && File.Exists(configLocation))
            {
                var cfgBuilder = new ConfigurationBuilder();
                cfgBuilder.AddJsonFile(configLocation, optional: false, reloadOnChange: false);
                return cfgBuilder.Build();
            }

            return null;
        }

        private static (ApplicationConfig, bool) ReadConfig(string configLocation)
        {
            try
            {
                var configContents = File.ReadAllText(configLocation);
                return (JsonSerializer.Deserialize<ApplicationConfig>(configContents), true);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Could not read config. Exception: {ex.Message}.");
                return (null, false);
            }
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
