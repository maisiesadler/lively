using System;
using System.Collections.Generic;
using System.Reflection;
using Lively.Resolvers;
using Microsoft.Extensions.Configuration;

namespace Lively
{
    public class DependencyTreeConfig
    {
        public IReadOnlyList<Assembly> Assemblies { get; }
        public IConfiguration Configuration { get; }
        public string StartupName { get; set; } = "Startup";
        public HashSet<string> SkipTypes { get; set; }
        public Func<DependencyTreeConfig, IInterfaceResolver> CreateInterfaceResolver { get; set; } = StartupInterfaceResolver.Create;

        public DependencyTreeConfig(
            Assembly assembly,
            IConfiguration configuration = null)
            : this(new[] { assembly ?? throw new ArgumentNullException(nameof(assembly)) }, configuration) { }

        public DependencyTreeConfig(
            IReadOnlyList<Assembly> assemblies,
            IConfiguration configuration = null)
        {
            Assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            if (Assemblies.Count == 0) throw new InvalidOperationException();
            Configuration = configuration;
        }

        public StartupInterfaceResolverConfig StartupConfig => new StartupInterfaceResolverConfig
        {
            Assemblies = Assemblies,
            Configuration = Configuration,
            StartupName = StartupName,
            SkipTypes = SkipTypes,
        };
    }
}
