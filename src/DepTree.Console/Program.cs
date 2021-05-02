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

            var applicationConfig = JsonSerializer.Deserialize<ApplicationConfig>(File.ReadAllText(configLocation));

            var assemblyLocation = args[1];
            if (!File.Exists(assemblyLocation))
            {
                System.Console.WriteLine($"Assembly location '{assemblyLocation}' does not exist'. {expectedArgs}");
                return -1;
            }

            var assemblyConfigLocation = args[2];
            var iconfiguration = BuildConfiguration(assemblyConfigLocation);

            var ass = Assembly.LoadFrom(assemblyLocation);
            var config = new DependencyTreeConfig(ass, iconfiguration, skipAssemblies: applicationConfig.Skip);

            var nodes = new List<DependencyTreeNode>();
            foreach (var classname in applicationConfig.Generate)
            {
                var dt = DependencyTree.Create(config, "PropertyService.Api.Infrastructure.Controllers.Legacy.AddressService.AddressServiceController");
                nodes.Add(dt);
            }

            var diagram = yUML.Create(nodes);
            System.Console.WriteLine(diagram);
            return 0;
        }

        private static IConfiguration BuildConfiguration(string assemblyConfigLocation)
        {
            if (assemblyConfigLocation != null && File.Exists(assemblyConfigLocation))
            {
                var cfgBuilder = new ConfigurationBuilder();
                cfgBuilder.AddJsonFile(assemblyConfigLocation, optional: false, reloadOnChange: false);
                return cfgBuilder.Build();
            }

            return null;
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
