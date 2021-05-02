using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DepTree.Resolvers
{
    public class StartupInterfaceResolverConfig
    {
        public Assembly Assembly { get; set; }
        public IConfiguration Configuration { get; set; }
        public HashSet<string> SkipAssemblies { get; set; }
        public string StartupName { get; set; } = "Startup";
    }
}
