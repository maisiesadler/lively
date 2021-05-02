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

        public StartupInterfaceResolver(Assembly assembly, HashSet<string> skipAssemblies, string startupName)
        {
            var startupType = assembly.GetTypes().FirstOrDefault(x => x.Name == startupName);
            if (startupType == null)
            {
                throw new Exception("Could not find Startup in assembly");
            }

            var startup = Activator.CreateInstance(startupType);
            MethodInfo method = startupType.GetMethod("ConfigureServices");
            _fakeServiceCollection = new FakeServiceCollection();
            method.Invoke(startup, new[] { _fakeServiceCollection });

            SkipAssemblies = skipAssemblies;
        }

        public StartupInterfaceResolver(Assembly assembly, IConfiguration configuration, HashSet<string> skipAssemblies, string startupName)
        {
            var startupType = assembly.GetTypes().FirstOrDefault(x => x.Name == startupName);
            if (startupType == null)
            {
                throw new Exception("Could not find Startup in assembly");
            }

            var startup = Activator.CreateInstance(startupType, configuration);
            MethodInfo method = startupType.GetMethod("ConfigureServices");
            _fakeServiceCollection = new FakeServiceCollection();
            method.Invoke(startup, new[] { _fakeServiceCollection });

            SkipAssemblies = skipAssemblies;
        }

        public HashSet<string> SkipAssemblies { get; }

        public Type Resolve(Type t)
        {
            if (SkipAssemblies?.Contains(t.FullName) == true) return null;

            return _fakeServiceCollection.GetImplementation(t);
        }
    }
}
