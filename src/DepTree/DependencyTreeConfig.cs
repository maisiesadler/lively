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
        public IInterfaceResolver InterfaceResolver { get; }
        public string StartupName { get; }

        public DependencyTreeConfig(Assembly assembly, HashSet<string> skipAssemblies = null, string startupName = "Startup")
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            InterfaceResolver = new StartupInterfaceResolver(assembly, skipAssemblies, startupName);
        }

        public DependencyTreeConfig(Assembly assembly, IConfiguration configuration, HashSet<string> skipAssemblies = null, string startupName = "Startup")
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            InterfaceResolver = new StartupInterfaceResolver(assembly, configuration, skipAssemblies, startupName);
        }
    }
}
