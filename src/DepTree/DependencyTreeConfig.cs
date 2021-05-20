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
        public HashSet<string> SkipTypes { get; set; }
        public Func<DependencyTreeConfig, IInterfaceResolver> CreateInterfaceResolver { get; set; } = StartupInterfaceResolver.Create;

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
            SkipTypes = SkipTypes,
        };
    }
}
