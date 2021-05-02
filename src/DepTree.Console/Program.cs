using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DepTree.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var ass = Assembly.LoadFrom("/Users/maisiesadler/comparison/property-service/src/PropertyService.Api/bin/Release/net5.0/osx.10.12-x64/PropertyService.Api.dll");
            var configLocation = "/Users/maisiesadler/comparison/property-service/src/PropertyService.Test.Integration/testsettings.json";

            var cfgBuilder = new ConfigurationBuilder();
            cfgBuilder.AddJsonFile(configLocation, optional: false, reloadOnChange: false);
            var iconfiguration = cfgBuilder.Build();
            var skipAssemblies = new HashSet<string> { "Serilog.IDiagnosticContext" };
            var config = new DependencyTreeConfig(ass, iconfiguration, skipAssemblies);
            var dt = DependencyTree.Create(config, "PropertyService.Api.Infrastructure.Controllers.Legacy.AddressService.AddressServiceController");

            Print(dt);
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
