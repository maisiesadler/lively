using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DepTree.Resolvers
{
    public class StartupInterfaceResolver : IInterfaceResolver
    {
        private readonly FakeServiceCollection _fakeServiceCollection;

        public StartupInterfaceResolver(StartupInterfaceResolverConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (config.Assembly == null) throw new ArgumentNullException(nameof(config.Assembly));

            var startupType = config.Assembly.GetTypes().FirstOrDefault(x => x.Name == config.StartupName);
            if (startupType == null)
            {
                throw new Exception("Could not find Startup in assembly");
            }

            var startup = CreateInstance(startupType, config.Configuration);
            MethodInfo method = startupType.GetMethod("ConfigureServices");
            _fakeServiceCollection = new FakeServiceCollection();
            method.Invoke(startup, new[] { _fakeServiceCollection });

            SkipAssemblies = config.SkipAssemblies;
        }

        private object CreateInstance(Type startupType, IConfiguration configuration)
        {
            if (configuration == null)
            {
                return Activator.CreateInstance(startupType);
            }

            return Activator.CreateInstance(startupType, configuration);
        }

        public HashSet<string> SkipAssemblies { get; }

        public Type Resolve(Type t)
        {
            if (SkipAssemblies?.Contains(t.FullName) == true) return null;

            return _fakeServiceCollection.GetImplementation(t);
        }
    }
}
