using System.Collections.Generic;
using System.Reflection;
using Lively.Diagrams;
using Lively.Console.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lively.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var envProvider = new EnvironmentVariableProvider();
            var (applicationConfig, ok) = ApplicationConfiguration.Build(envProvider, args);
            if (!ok)
            {
                System.Console.WriteLine($"Application Config is not valid. {string.Join(", ", applicationConfig.Errors)}.");
                return -1;
            }

            var (assembliesOk, assemblies) = TryBuildAssemblies(applicationConfig);
            if (!assembliesOk)
            {
                System.Console.WriteLine($"Could not load assemblies from {applicationConfig.AssemblyLocation}");
                return -1;
            }

            var config = new DependencyTreeConfig(assemblies, applicationConfig.AssemblyConfiguration)
            {
                CreateInterfaceResolver = applicationConfig.CreateInterfaceResolver,
                SkipTypes = applicationConfig.Skip,
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
            else if (applicationConfig.OutputFormat == OutputFormatType.Mermaid)
            {
                var diagram = Mermaid.Create(nodes);
                System.Console.WriteLine(diagram);
            }
            else if (applicationConfig.OutputFormat == OutputFormatType.MermaidMd)
            {
                var diagram = MermaidMd.Create(nodes);
                System.Console.WriteLine(diagram);
            }
            else if (applicationConfig.OutputFormat == OutputFormatType.PlantUml)
            {
                var diagram = PlantUml.Create(nodes);
                System.Console.WriteLine(diagram);
            }
            else if (applicationConfig.OutputFormat == OutputFormatType.FullNamePlantUml)
            {
                var diagram = FullNamePlantUml.Create(nodes);
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

        private static (bool, Assembly[]) TryBuildAssemblies(ApplicationConfiguration config)
        {
            if (string.IsNullOrEmpty(config.AssemblyLocation))
                return (false, null);

            if (File.Exists(config.AssemblyLocation))
            {
                var assembly = Assembly.LoadFrom(config.AssemblyLocation);
                return (true, new[] { assembly });
            }
            else if (Directory.Exists(config.AssemblyLocation))
            {
                var assemblies = DependencyTreeConfigExtensions.GetAllAssembliesInDirectory(
                    config.AssemblyLocation,
                    filename =>
                    {
                        if (string.IsNullOrWhiteSpace(config.AssemblyPatternMatch))
                            return true;

                        var split = filename.Split('/');
                        var assemblyName = split[split.Length - 1];
                        return Regex.IsMatch(assemblyName, config.AssemblyPatternMatch);
                    }).ToArray();
                return (true, assemblies);
            }

            return (false, null);
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
