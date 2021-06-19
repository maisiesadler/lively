using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Lively.Resolvers
{
    public class StartupInterfaceResolverConfig
    {
        public IList<Assembly> Assemblies { get; set; }
        public IConfiguration Configuration { get; set; }
        public HashSet<string> SkipTypes { get; set; }
        public string StartupName { get; set; } = "Startup";

        public IEnumerable<Type> AssemblyTypes => Assemblies.SelectMany(a => a.GetTypes());
    }
}
