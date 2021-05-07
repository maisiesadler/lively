using System;
using System.Collections.Generic;
using System.Reflection;
using DepTree.Resolvers;
using Microsoft.Extensions.Configuration;

namespace DepTree
{
    public class DependencyTreeConfig
    {
        public Assembly Assembly { get; }
        public IConfiguration Configuration { get; }
        public string StartupName { get; set; } = "Startup";
        public HashSet<string> SkipAssemblies { get; set; }
        public InterfaceResolverType InterfaceResolverType { get; set; } = InterfaceResolverType.Startup;

        public DependencyTreeConfig(
            Assembly assembly,
            IConfiguration configuration = null)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            Configuration = configuration;
        }

        public StartupInterfaceResolverConfig StartupConfig => new StartupInterfaceResolverConfig
        {
            Assembly = Assembly,
            Configuration = Configuration,
            StartupName = StartupName,
            SkipAssemblies = SkipAssemblies,
        };
    }
}
